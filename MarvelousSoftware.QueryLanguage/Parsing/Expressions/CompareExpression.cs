using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract;
using MarvelousSoftware.QueryLanguage.Parsing.Models;

namespace MarvelousSoftware.QueryLanguage.Parsing.Expressions
{
    /// <summary>
    /// Represents a compare expression, for instance: `Name = "Damian"`. 
    /// </summary>
    public sealed class CompareExpression : ExpressionBase
    {
        public override ExpressionType ExpressionType => ExpressionType.CompareExpression;

        /// <summary>
        /// Column token.
        /// </summary>
        public readonly ColumnToken Column;

        /// <summary>
        /// Compare operator token.
        /// </summary>
        public readonly CompareOperatorToken CompareOperator;

        /// <summary>
        /// Value used for comparison.
        /// </summary>
        public readonly IEvaluableToken Value;

        public CompareExpression(ColumnToken column, CompareOperatorToken compareOperator, IEvaluableToken value)
        {
            Column = column;
            CompareOperator = compareOperator;
            Value = value;
        }

        public override void Visit(ExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return Column + " " + CompareOperator + " " + Value;
        }
    }
}