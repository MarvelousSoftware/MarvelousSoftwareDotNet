using MarvelousSoftware.QueryLanguage.Parsing.Models;

namespace MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract
{
    /// <summary>
    /// Base class for all expressions.
    /// </summary>
    public abstract class ExpressionBase
    {
        /// <summary>
        /// Type of an expression.
        /// </summary>
        public abstract ExpressionType ExpressionType { get; }

        public abstract void Visit(ExpressionVisitor visitor);
    }
}