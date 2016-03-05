using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokens
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
