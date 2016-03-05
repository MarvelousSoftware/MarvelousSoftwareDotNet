using MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokens
{
    /// <summary>
    /// Contains data regarding whitespace places in the query.
    /// </summary>
    public class WhitespaceToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Whitespace;
        
        // TODO: some Trivia indicator

        public override string ToString()
        {
            return " ";
        }
    }
}
