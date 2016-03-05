using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokens
{
    public class ParenCloseToken : TokenBase
    {
        public override TokenType TokenType => TokenType.ParenClose;

        public override string ToString()
        {
            return ")";
        }
    }
}
