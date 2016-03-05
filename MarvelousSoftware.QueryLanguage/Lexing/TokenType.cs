#pragma warning disable 1591
namespace MarvelousSoftware.QueryLanguage.Lexing
{
    public enum TokenType
    {
        Unknown,
        Whitespace,
        Column,
        CompareOperator,
        LogicalOperator,
        Literal,
        Function,
        Statement,
        ParenOpen,
        ParenClose
    }
}
