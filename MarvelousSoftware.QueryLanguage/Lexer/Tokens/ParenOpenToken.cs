using MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokens
{
    public class ParenOpenToken : TokenBase
    {
        public override TokenType TokenType => TokenType.ParenOpen;

        public override string ToString()
        {
            return "(";
        }
    }
}
