using MarvelousSoftware.QueryLanguage.Parsing.Expressions;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract;

namespace MarvelousSoftware.QueryLanguage.Parsing
{
    public abstract class ExpressionVisitor
    {
        public virtual void Visit(BinaryExpression binaryExpression) { }

        public virtual void Visit(CompareExpression compareExpression) { }

        public virtual void Visit(StatementExpression statementExpression) { }

        public void Visit(ExpressionBase expression)
        {
            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                Visit(binaryExpression);
                return;
            }

            var compareExpression = expression as CompareExpression;
            if (compareExpression != null)
            {
                Visit(compareExpression);
                return;
            }

            Visit((StatementExpression) expression);
        }
    }
}