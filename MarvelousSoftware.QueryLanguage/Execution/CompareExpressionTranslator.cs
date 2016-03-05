using System;
using System.Linq.Expressions;
using MarvelousSoftware.Common.Utils;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions;

namespace MarvelousSoftware.QueryLanguage.Execution
{
    internal class CompareExpressionTranslator
    {
        /// <summary>
        /// Translates MQL compare expression to .net Expression using provided parameter.
        /// </summary>
        public static Expression Execute(CompareExpression compareExpression, Expression paramExpression)
        {
            var expression = HandleString(compareExpression, paramExpression)
                ?? HandleNumberAndDateTime(compareExpression, paramExpression)
                ?? HandleBoolean(compareExpression, paramExpression);

            if (expression == null)
            {
                throw new QueryLanguageException($"Compare expression '{compareExpression.CompareOperator}' doesn't support '{compareExpression.Column.ColumnInfo.SystemType}' type.");
            }

            return expression;
        }

        private static Expression HandleString(CompareExpression compareExpression, Expression paramExpression)
        {
            var columnInfo = compareExpression.Column.ColumnInfo;

            if (columnInfo.ColumnType != ColumnType.String)
            {
                return null;
            }

            var value = compareExpression.Value.Evaluate();
            var member = ExpressionUtils.GetNestedPropertyOrField(paramExpression, columnInfo.MemberName);

            Func<Expression, Expression, System.Linq.Expressions.BinaryExpression> compareMethod = null;
            bool expectNull = false;
            string methodToBeCalled = null;
            switch (compareExpression.CompareOperator.KeywordType)
            {
                case KeywordType.Equal:
                    compareMethod = Expression.Equal;
                    break;
                case KeywordType.NotEqual:
                    compareMethod = Expression.NotEqual;
                    if (value != null)
                    {
                        // eadge case: FirstName != "Damian"
                        // it should return also null values
                        expectNull = true;
                    }
                    break;
                case KeywordType.Contains:
                    methodToBeCalled = "Contains";
                    break;
                case KeywordType.StartsWith:
                    methodToBeCalled = "StartsWith";
                    break;
                case KeywordType.EndsWith:
                    methodToBeCalled = "EndsWith";
                    break;
                default:
                    return null;
            }

            Expression final;
            System.Linq.Expressions.BinaryExpression nullCheck;
            if (compareMethod != null)
            {
                if (value == null)
                {
                    return compareMethod(member, Expression.Constant(null));
                }

                nullCheck = ExpressionUtils.CheckForNulls(paramExpression, columnInfo.MemberName, expectNull: expectNull);
                final = compareMethod(member, Expression.Constant(value));

                if (expectNull)
                {
                    return Expression.OrElse(nullCheck, final);
                }
                return Expression.AndAlso(nullCheck, final);
            }

            if (methodToBeCalled == null)
            {
                throw new QueryLanguageException("Failed to create comparision.");
            }

            final = Expression.Call(
                member,
                columnInfo.SystemType.GetMethod(methodToBeCalled, new[] { typeof(string) }),
                Expression.Constant(value));

            nullCheck = ExpressionUtils.CheckForNulls(paramExpression, columnInfo.MemberName);
            return Expression.AndAlso(nullCheck, final);
        }

        private static Expression HandleNumberAndDateTime(CompareExpression compareExpression, Expression paramExpression)
        {
            var columnInfo = compareExpression.Column.ColumnInfo;

            if (columnInfo.ColumnType != ColumnType.Float && columnInfo.ColumnType != ColumnType.Integer
                && columnInfo.ColumnType != ColumnType.DateTime)
            {
                return null;
            }

            var value = compareExpression.Value.Evaluate();
            var member = ExpressionUtils.GetNestedPropertyOrField(paramExpression, columnInfo.MemberName);
            Func<Expression, Expression, System.Linq.Expressions.BinaryExpression> compareMethod;

            bool expectNull = false;
            switch (compareExpression.CompareOperator.KeywordType)
            {
                case KeywordType.Equal:
                    compareMethod = Expression.Equal;
                    break;
                case KeywordType.NotEqual:
                    compareMethod = Expression.NotEqual;
                    if (value != null)
                    {
                        // edge case: Salary != 5000
                        // it should return also null values
                        expectNull = true;
                    }
                    break;
                case KeywordType.GreaterThan:
                    compareMethod = Expression.GreaterThan;
                    break;
                case KeywordType.GreaterThanOrEqual:
                    compareMethod = Expression.GreaterThanOrEqual;
                    break;
                case KeywordType.LessThan:
                    compareMethod = Expression.LessThan;
                    break;
                case KeywordType.LessThanOrEqual:
                    compareMethod = Expression.LessThanOrEqual;
                    break;
                default:
                    return null;
            }

            if (columnInfo.IsNullable == false)
            {
                return compareMethod(member, Expression.Constant(value));
            }

            if (value == null)
            {
                return compareMethod(member, Expression.Constant(null));
            }

            var nullCheck = ExpressionUtils.CheckForNulls(paramExpression, columnInfo.MemberName, expectNull: expectNull);
            var final = compareMethod(
                Expression.Convert(member, value.GetType()),
                Expression.Convert(Expression.Constant(value), value.GetType()));

            if (expectNull)
            {
                return Expression.OrElse(nullCheck, final);
            }
            return Expression.AndAlso(nullCheck, final);
        }

        private static Expression HandleBoolean(CompareExpression compareExpression, Expression paramExpression)
        {
            var columnInfo = compareExpression.Column.ColumnInfo;

            if (columnInfo.ColumnType != ColumnType.Boolean)
            {
                return null;
            }

            var member = ExpressionUtils.GetNestedPropertyOrField(paramExpression, columnInfo.MemberName);
            var expected = compareExpression.Value.Evaluate();

            Func<Expression, Expression, System.Linq.Expressions.BinaryExpression> compareMethod;
            switch (compareExpression.CompareOperator.KeywordType)
            {
                case KeywordType.Equal:
                    compareMethod = Expression.Equal;
                    break;
                case KeywordType.NotEqual:
                    compareMethod = Expression.NotEqual;
                    break;
                default:
                    return null;
            }

            if (columnInfo.IsNullable)
            {
                var compare = ExpressionUtils.CheckForNulls(
                    paramExpression,
                    columnInfo.MemberName,
                    expectNull: false,
                    checkLast: true);
                return Expression.AndAlso(compare, compareMethod(member, Expression.Constant(expected, columnInfo.SystemType)));
            }

            return compareMethod(member, Expression.Constant(expected, columnInfo.SystemType));
        }
    }
}