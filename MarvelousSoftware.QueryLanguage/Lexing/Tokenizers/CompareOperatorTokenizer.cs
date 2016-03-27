using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokenizers
{
    internal sealed class CompareOperatorTokenizer<T> : KeywordTokenizerBase<T, CompareOperatorToken>
    {
        public override bool ShouldTokenizeAfter(TokenType? token) => token == TokenType.Column;

        public override CompareOperatorToken GetToken(Keyword keyword) => new CompareOperatorToken(keyword);

        public override IEnumerable<Keyword> GetPossibleKeywords(LexerRuntimeInfo<T> info)
        {
            return info.Lang.SyntaxConfig.CompareOperators;
        }

        public override Error OnNotFoundError(string word)
        {
            return new Error($"\"{word}\" is not a compare operator.", ErrorId.CompareOperatorNotFound, ErrorType.Critical, word);
        }

        public override void OnTokenized(ref TokenizationResult result, LexerRuntimeInfo<T> info)
        {
            var token = (CompareOperatorToken) result.ParsedToken;

            var isAllowed = info.LastColumn?.IsCompareOperatorAllowed(token.Keyword);
            if (isAllowed == false)
            {
                var msg = $"Column '{info.LastColumn.ColumnName}' doesn't support '{token.Keyword.Syntax}' compare operator.";
                result.Errors.Add(msg, ErrorId.NotSupportedCompareOperator, ErrorType.Critical);
                result.Parsed = false;
            }
        }
    }
}
