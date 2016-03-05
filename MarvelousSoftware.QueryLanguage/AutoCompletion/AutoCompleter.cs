using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.Common.Utils;
using MarvelousSoftware.QueryLanguage.AutoCompletion.Completers;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion
{
    public class AutoCompleter<T>
    {
        private readonly LanguageConfig<T> _config;
        private readonly CompleterBase<T>[] _completers;

        public AutoCompleter(LanguageConfig<T> config)
        {
            _config = config;
            _completers = TypeUtils.CreateAllOf<CompleterBase<T>>(
                typeof(CompleterBase), config)
                .OrderBy(x => x.Order)
                .ToArray();
        }

        public AutoCompletionResult Run(string query, int caretPosition, int skip)
        {
            if (caretPosition > query.Length)
            {
                throw new QueryLanguageException($"Caret position ({caretPosition}) is out of range of `{query}` query.");
            }

            var lexer = new Lexer<T>(_config);
            var lexerResult = lexer.Run(query, new LexerConfig()
            {
                AllowUnknownToken = true
            });

            if (lexerResult.Errors.Any())
            {
                return new AutoCompletionResult
                {
                    Errors = lexerResult.Errors
                };
            }

            var currentPositionInfo = GetCurrentPositionInfo(query, caretPosition, lexerResult.Tokens);
            var nexts = GetNextTo(currentPositionInfo.PrevToken);
            return GetCompletions(nexts, currentPositionInfo);
        }

        private AutoCompletionResult GetCompletions(IEnumerable<TokenType> nexts, PositionInfo positionInfo)
        {
            // TODO: completion should be a complex object with text to display and text to change
            // that would be helpful e.g. in case of dates

            var completions = new List<AutoCompletionRow>();
            var result = new AutoCompletionResult();

            if (positionInfo.CurrentToken != null)
            {
                if (positionInfo.CurrentToken.TokenType == TokenType.Whitespace)
                {
                    result.StartPosition = positionInfo.CaretPosition;
                    result.Length = 0;
                }
                else
                {
                    result.StartPosition = positionInfo.CurrentToken.StartPosition;
                    result.Length = positionInfo.CurrentToken.EndPosition - positionInfo.CurrentToken.StartPosition;
                }
            }

            var total = 0;
            var remainingBatchSize = _config.AutoCompleteOptions.PageSize;

            foreach (var next in nexts)
            {
                foreach (var completer in _completers)
                {
                    if (!completer.ShouldProcess(next))
                    {
                        continue;
                    }

                    var current = completer.GetCompletions(new CompletionInfo()
                    {
                        TokenType = next,
                        ColumnInfo = positionInfo.LastColumn,
                        BatchSize = remainingBatchSize,
                        ComparePart = positionInfo.TextToCompare
                    });
                    if (current == null)
                    {
                        continue;
                    }

                    var currentCompletions = current.Completions.Select(x => new AutoCompletionRow()
                    {
                        Value = x.Value,
                        Text = x.GetText(_config.Syntax.Config.StringLiteralIdentifier),
                        Group = completer.CompletionGroup
                    }).ToArray();

                    total += currentCompletions.Length;
                    remainingBatchSize = _config.AutoCompleteOptions.PageSize - total;
                    completions.AddRange(currentCompletions);

                    result.IsNextPageAvailable = result.IsNextPageAvailable || current.IsNextPageAvailable;
                }
            }

            result.Completions = completions.OrderBy(x => x.Group).ThenBy(x => x.Value);
            return result;
        }

        private PositionInfo GetCurrentPositionInfo(string query, int caretPosition, IEnumerable<TokenBase> tokens)
        {
            var info = new PositionInfo()
            {
                CaretPosition = caretPosition
            };

            foreach (var token in tokens)
            {
                info.CurrentToken = token;

                if (token.TokenType == TokenType.Column)
                {
                    info.LastColumnToken = token;
                }

                if (token.StartPosition <= caretPosition && token.EndPosition >= caretPosition)
                {
                    if (token.EndPosition != caretPosition)
                    {
                        break;
                    }

                    if (token.TokenType != TokenType.Whitespace)
                    {
                        break;
                    }
                }

                if (info.CurrentToken.StartPosition < caretPosition && info.CurrentToken.TokenType != TokenType.Whitespace)
                {
                    info.PrevToken = info.CurrentToken;
                }
            }

            if (info.LastColumnToken != null)
            {
                info.LastColumn = _config.GetColumnInfo(info.LastColumnToken.GetText(query));
            }

            if (info.CurrentToken != null && info.CurrentToken.StartPosition != caretPosition // not at start of token
                && info.CurrentToken.TokenType != TokenType.Whitespace) // and not whitespace 
            {
                info.TextToCompare = info.CurrentToken
                    .GetSliceOfText(query, caretPosition - info.CurrentToken.StartPosition);
            }

            return info;
        }

        private IEnumerable<TokenType> GetNextTo(TokenBase token)
        {
            if (token == null)
                return new[] {TokenType.Column, TokenType.ParenOpen};
            return _config.TokenizersProvider.GetTokenTypesNextTo(token.TokenType);
        }

        private class PositionInfo
        {
            public TokenBase CurrentToken;
            public TokenBase PrevToken;
            public TokenBase LastColumnToken;
            public ColumnInfo LastColumn;
            public string TextToCompare = "";
            public int CaretPosition;
        }
    }
}