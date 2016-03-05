using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokens
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
