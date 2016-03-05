using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokens
{
    /// <summary>
    /// Token used in case if any other token cannot be created.
    /// </summary>
    public class UnknownToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Unknown;
        public readonly string Text;

        public UnknownToken(string text)
        {
            Text = text;
        }

        public UnknownToken Merge(string query, UnknownToken token)
        {
            var newText = Text + token.Text;

            var newToken = new UnknownToken(newText);
            newToken.SetPosition(StartPosition, newText.Length);

            return newToken;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
