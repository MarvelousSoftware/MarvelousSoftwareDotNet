using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract
{
    public abstract class KeywordTokenizerBase<T, TKeywordToken> : ITokenizer<T> where TKeywordToken : KeywordTokenBase
    {
        public abstract bool ShouldTokenizeAfter(TokenType? token);

        public TokenizationResult Tokenize(LexerRuntimeInfo<T> info)
        {
            var result = new TokenizationResult();

            var reader = new QueryStringReader(info.Query, info.CurrentPosition);
            var keyword = reader.ReadTillEndOfKeyword(GetPossibleKeywords(info), info.LanguageSpecialChars, info.Lang.Syntax.Config.KeywordCaseSensitive);

            var token = GetToken(info.Lang.GetKeyword(keyword));
            token.SetPosition(info.CurrentPosition, reader.ReadLength);

            result.ParsedToken = token;

            if (token.Keyword == null)
            {
                result.Parsed = false;

                var word = new QueryStringReader(info.Query, info.CurrentPosition).ReadTillEndOfWord();
                result.Errors.Add(OnNotFoundError(word));
                return result;
            }
            
            OnTokenized(ref result, info);

            return result;
        }

        /// <summary>
        /// Should create instance of <see cref="TKeywordToken"/> using provided parameter.
        /// </summary>
        public abstract TKeywordToken GetToken(Keyword keyword);

        /// <summary>
        /// Should provide possible keywords.
        /// </summary>
        public abstract IEnumerable<Keyword> GetPossibleKeywords(LexerRuntimeInfo<T> info);

        /// <summary>
        /// Allows to react in case if keyword is not found.
        /// </summary>
        public abstract Error OnNotFoundError(string word);

        /// <summary>
        /// Allows to change result of tokenization.
        /// </summary>
        public virtual void OnTokenized(ref TokenizationResult result, LexerRuntimeInfo<T> info)
        {
        }
    }
}