using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract;
using MarvelousSoftware.QueryLanguage.Parsing.Models;

namespace MarvelousSoftware.QueryLanguage.Parsing.Expressions
{
    /// <summary>
    /// Represents a compare expression, for instance: `Name is empty`, where is empty is a Statement and Name is a Column. 
    /// </summary>
    public sealed class StatementExpression : ExpressionBase
    {
        public override ExpressionType ExpressionType => ExpressionType.StatementExpression;

        /// <summary>
        /// Column token.
        /// </summary>
        public readonly ColumnToken Column;

        /// <summary>
        /// Statement token.
        /// </summary>
        public readonly StatementToken Statement;

        public StatementExpression(ColumnToken column, StatementToken statement)
        {
            Column = column;
            Statement = statement;
        }

        public override void Visit(ExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return Column + " " + Statement;
        }
    }
}