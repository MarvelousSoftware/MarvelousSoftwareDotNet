using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokens
{
    /// <summary>
    /// Token for keywords such as 'Equals' or 'StartsWith'.
    /// </summary>
    public class CompareOperatorToken : KeywordTokenBase
    {
        public override TokenType TokenType => TokenType.CompareOperator;

        public CompareOperatorToken(Keyword keyword) : base(keyword)
        {
        }
    }
}
