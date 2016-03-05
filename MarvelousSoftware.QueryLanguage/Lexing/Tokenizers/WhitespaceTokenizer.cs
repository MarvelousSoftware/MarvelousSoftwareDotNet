using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokenizers
{
    internal sealed class WhitespaceTokenizer<T> : ITokenizer<T>
    {
        public bool ShouldTokenizeAfter(TokenType? token)
        {
            return true;
        }

        public TokenizationResult Tokenize(LexerRuntimeInfo<T> info)
        {
            var result = new TokenizationResult();

            if (string.IsNullOrWhiteSpace(info.CurrentChar.ToString()) == false)
            {
                result.Parsed = false;
                return result;
            }

            var reader = new QueryStringReader(info.Query, info.CurrentPosition);
            reader.ReadTillEndOfWhitespace();

            result.ParsedToken = new WhitespaceToken();
            result.ParsedToken.SetPosition(info.CurrentPosition, reader.ReadLength);

            return result;
        }
    }
}
