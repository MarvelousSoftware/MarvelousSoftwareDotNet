using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer.Tokenizers;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexer
{
    /// <summary>
    /// Allows to parse a query from string to a list of tokens.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    public class Lexer<T> : ILexer
    {
        /// <summary>
        /// Creates <see cref="Lexer{T}"/> instance using provided language configuration.
        /// </summary>
        /// <param name="languageConfig">Language related configuration.</param>
        public Lexer(LanguageConfig<T> languageConfig)
        {
            _lang = languageConfig;
        }

        private readonly LanguageConfig<T> _lang;

        public LexerResult Run(string query, LexerConfig lexerConfig = null)
        {
            lexerConfig = lexerConfig ?? new LexerConfig();
            var result = new LexerResult();
            var unknownLexer = new UnknownTokenizer<T>();

            var info = new LexerRuntimeInfo<T>(query, _lang);
            var expectedTokenTypes = new List<TokenType>(){TokenType.Whitespace, TokenType.ParenOpen, TokenType.Column};
            
            while (info.IsInRange())
            {
                var anyLexerParsed = false;
                var errors = new ErrorList();
                
                foreach (var expectedType in expectedTokenTypes)
                {
                    var lexer = _lang.TokenizersProvider.Get(expectedType);

                    var parseResult = lexer.Tokenize(info);

                    if (parseResult.Errors.Any())
                    {
                        errors.AddRange(parseResult.Errors);
                        if (parseResult.Terminate)
                        {
                            break;
                        }

                        continue;
                    }

                    if (parseResult.Terminate)
                    {
                        break;
                    }

                    if (parseResult.Parsed == false)
                    {
                        continue;
                    }

                    result.Tokens.Add(parseResult.ParsedToken);
                    info.CurrentPosition = parseResult.ParsedToken.EndPosition;
                    anyLexerParsed = true;

                    // in case of whitespace next expected expressions has not changed
                    if (expectedType == TokenType.Whitespace)
                    {
                        break;
                    }

                    expectedTokenTypes = _lang.TokenizersProvider.GetTokenTypesNextTo(parseResult.ParsedToken.TokenType).ToList();
                    break;
                }

                if (!anyLexerParsed && lexerConfig.AllowUnknownToken == false)
                {
                    result.Errors.AddRange(errors);
                }

                if (!anyLexerParsed)
                {
                    var parsedToken = unknownLexer.Tokenize(info).ParsedToken;

                    // If for some reason even unknownLexer is not able to
                    // parse the token then stop lexer execution.
                    // Without this break it may result in never ending while.
                    // If all parsers works correctly it should never happen though.
                    // This line is just for caution.
                    if (parsedToken.Length == 0)
                    {
                        result.Errors.Add("Unexpected error.", ErrorId.UnexpectedError, ErrorType.Critical);
                        break;
                    }

                    var lastToken = result.Tokens.LastOfType<UnknownToken>(2);
                    if (lastToken != null)
                    {
                        // The idea is to have only one unknown token till 
                        // parser is able to resume normal execution.
                        parsedToken = lastToken.Merge(query, parsedToken.As<UnknownToken>());
                        result.Tokens.RemoveAt(result.Tokens.Count - 1);
                    }

                    result.Tokens.Add(parsedToken);
                    info.CurrentPosition = parsedToken.EndPosition;
                    var nexts = new List<TokenType>();
                    foreach (var tokenType in expectedTokenTypes)
                    {
                        nexts.Add(tokenType);
                        nexts.AddRange(_lang.TokenizersProvider.GetTokenTypesNextTo(tokenType));
                    }
                    expectedTokenTypes = nexts.Distinct().ToList();

                    // greediness is disabled to make sure that string won't be swallowed by
                    // literal lexer
                    info.AllowedToBeGreedy = false;
                    continue;
                }

                info.AllowedToBeGreedy = true;
            }

            var unknownTokens = result.Tokens.Where(x => x.TokenType == TokenType.Unknown).ToList();
            if (lexerConfig.AllowUnknownToken == false && result.Errors.Any() == false && unknownTokens.Any())
            {
                // since no errors (like invalid column name) and yet some tokens are unknown
                // then generic one is being added
                foreach (var unknownToken in unknownTokens)
                {
                    var msg = $"Unknown expression: {unknownToken}";
                    result.Errors.Add(msg, ErrorId.UnknownExpression, ErrorType.Critical);
                }
            }

            return result;
        }
    }

    public class LexerConfig
    {
        public bool AllowUnknownToken { get; set; }
    }
}
