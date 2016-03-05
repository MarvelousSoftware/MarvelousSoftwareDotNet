using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokenizers
{
    internal sealed class ColumnTokenizer<T> : ITokenizer<T>
    {
        public bool ShouldTokenizeAfter(TokenType? token)
        {
            if (token == null)
            {
                return true;
            }

            switch (token)
            {
                case TokenType.ParenOpen:
                case TokenType.LogicalOperator:
                    return true;
            }

            return false;
        }

        public TokenizationResult Tokenize(LexerRuntimeInfo<T> info)
        {
            var result = new TokenizationResult();

            var reader = new QueryStringReader(info.Query, info.CurrentPosition);
            var columnName = reader.ReadTillIvalidCharOrWhitespace(info.LanguageSpecialChars);

            var token = new ColumnToken(info.Lang.GetColumnInfo(columnName));
            token.SetPosition(info.CurrentPosition, reader.ReadLength);

            result.ParsedToken = token;

            if (token.ColumnInfo == null)
            {
                result.Parsed = false;
                result.Errors.Add(new Error($"\"{columnName}\" is not a column.", ErrorId.ColumnNotFound, ErrorType.Critical, columnName));
                return result;
            }

            info.LastColumn = token.ColumnInfo;

            return result;
        }
    }
}
