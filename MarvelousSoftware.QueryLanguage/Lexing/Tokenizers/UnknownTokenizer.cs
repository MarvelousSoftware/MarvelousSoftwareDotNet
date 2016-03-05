using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokenizers
{
    public class UnknownTokenizer<T> : ITokenizer<T>
    {
        public bool ShouldTokenizeAfter(TokenType? token)
        {
            return true;
        }

        public TokenizationResult Tokenize(LexerRuntimeInfo<T> info)
        {
            var reader = new QueryStringReader(info.Query, info.CurrentPosition)
            {
                ResetLengthOnEachRead = false
            };
            var text = reader.ReadTill(char.IsWhiteSpace);
            text += reader.ReadTillEndOfWhitespace();

            var token = new UnknownToken(text);
            token.SetPosition(info.CurrentPosition, reader.ReadLength);

            return new TokenizationResult
            {
                ParsedToken = token
            };
        }
    }
}