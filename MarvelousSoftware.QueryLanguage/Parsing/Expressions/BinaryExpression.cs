using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract;
using MarvelousSoftware.QueryLanguage.Parsing.Models;

namespace MarvelousSoftware.QueryLanguage.Parsing.Expressions
{
    /// <summary>
    /// Represents comparision of 2 nodes with logical operator.
    /// Example: `Name = "Damian" OR Name = "John"`.
    /// In this example `Name = "Damian"` would be left expression, 
    /// `Name = "John"` would be right expression and 
    /// OR would be logical operator.
    /// </summary>
    public sealed class BinaryExpression : ExpressionBase
    {
        public override ExpressionType ExpressionType => ExpressionType.BinaryExpression;

        /// <summary>
        /// Left side of the expression.
        /// </summary>
        public readonly ExpressionBase Left;

        /// <summary>
        /// Right side of the expression.
        /// </summary>
        public readonly ExpressionBase Right;

        /// <summary>
        /// Logical operator which combines <see cref="Left"/> and <see cref="Right"/> expressions.
        /// </summary>
        public readonly LogicalOperatorToken LogicalOperator;

        public BinaryExpression(ExpressionBase left, LogicalOperatorToken logicalOperator, ExpressionBase right)
        {
            Left = left;
            Right = right;
            LogicalOperator = logicalOperator;
        }

        public override void Visit(ExpressionVisitor visitor)
        {
            Left.Visit(visitor);
            Right.Visit(visitor);
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return "(" + Left + " " + LogicalOperator + " " + Right + ")";
        }
    }
}