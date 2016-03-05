using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MarvelousSoftware.Common.Utils
{
    public static class ExpressionUtils
    {
        public static string GetNameOf<TSource, TField>(Expression<Func<TSource, TField>> field)
        {
            return string.Join(".", PathExpressionVisitor.GetPath(field));
        }

        /// <summary>
        /// Gets access to nested property or field.
        /// </summary>
        /// <param name="expression">Root of the expression.</param>
        /// <param name="propertyOrFieldPath">Path to property, or field.</param>
        /// <returns>Generated expression.</returns>
        public static MemberExpression GetNestedPropertyOrField(Expression expression, string propertyOrFieldPath)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (string.IsNullOrEmpty(propertyOrFieldPath))
                throw new ArgumentNullException(nameof(propertyOrFieldPath));

            var parts = propertyOrFieldPath.Split('.');
            if (parts.Length == 1)
            {
                return Expression.PropertyOrField(expression, propertyOrFieldPath);
            }

            var newExpression = Expression.PropertyOrField(expression, parts[0]);
            var rest = string.Join(".", parts.Skip(1));
            return GetNestedPropertyOrField(newExpression, rest);
        }


        /// <summary>
        /// Checks wheter given properties or fields are not nulls.
        /// </summary>
        /// <example>
        /// Result of (x, "Person.Contact") invokation:
        /// 'x => x.Person != null && x.Person.Contact != null'
        /// </example>
        /// <param name="propertyOwner">Owner of the property.</param>
        /// <param name="propertyOrFieldPath">Full property or field path.</param>
        /// <param name="checkLast">If false then last property or field from propertyOrFieldPath won't be checked.</param>
        /// <param name="expectNull">
        /// If true then instead of 'x => x.Person != null && x.Person.Contact != null' it will create
        /// 'x => x.Person == null || x.Person.Contact == null'.
        /// </param>
        /// <returns>New expression with check for nulls.</returns>
        public static BinaryExpression CheckForNulls(Expression propertyOwner, string propertyOrFieldPath, bool checkLast = true,
            bool expectNull = false)
        {
            if (string.IsNullOrEmpty(propertyOrFieldPath))
                throw new ArgumentNullException(nameof(propertyOrFieldPath));

            int i = 0;
            BinaryExpression result = null;
            while (true)
            {
                // propertyOrFieldName -> Person.Contact.Something

                var parts = propertyOrFieldPath.Split('.'); // e.g. [Person,Contact,Something]
                var toCheck = string.Join(".", parts.Take(i + 1)); // e.g. Person.Contact
                var remaining = parts.Count() - i;
                var accessorExpression = GetNestedPropertyOrField(propertyOwner, toCheck); // e.g. x => x.Person.Contact

                if (remaining == 0)
                {
                    return result;
                }

                if (remaining == 1 && checkLast == false)
                {
                    return result;
                }

                BinaryExpression nullCheck;
                if (expectNull)
                {
                    nullCheck = Expression.Equal(accessorExpression, Expression.Constant(null));
                }
                else
                {
                    nullCheck = Expression.NotEqual(accessorExpression, Expression.Constant(null));
                }

                if (expectNull)
                {
                    result = result == null ? nullCheck : Expression.OrElse(result, nullCheck);
                }
                else
                {
                    result = result == null ? nullCheck : Expression.AndAlso(result, nullCheck);
                }

                if (remaining == 0)
                {
                    return result;
                }

                i++;
            }
        }

        private class PathExpressionVisitor : ExpressionVisitor
        {
            public static string[] GetPath<TSource, TResult>(Expression<Func<TSource, TResult>> expression)
            {
                var visitor = new PathExpressionVisitor();
                visitor.Visit(expression.Body);
                return Enumerable.Reverse(visitor._path).ToArray();
            }
            private readonly List<string> _path = new List<string>();
            protected override Expression VisitMember(MemberExpression node)
            {
                _path.Add(node.Member.Name);
                return base.VisitMember(node);
            }
        }
    }
}
