using MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokens
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
