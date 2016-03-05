using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokens
{
    /// <summary>
    /// Token for logical operator keywords.
    /// </summary>
    public class LogicalOperatorToken : KeywordTokenBase
    {
        public override TokenType TokenType => TokenType.LogicalOperator;

        public LogicalOperatorToken(Keyword keyword) : base(keyword)
        {
        }
    }
}
