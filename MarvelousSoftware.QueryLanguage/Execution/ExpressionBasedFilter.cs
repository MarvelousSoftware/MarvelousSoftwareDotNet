using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MarvelousSoftware.Common.Utils;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions;
using BinaryExpression = MarvelousSoftware.QueryLanguage.Parsing.Expressions.BinaryExpression;
using ExpressionVisitor = MarvelousSoftware.QueryLanguage.Parsing.ExpressionVisitor;

namespace MarvelousSoftware.QueryLanguage.Execution
{
    public class ExpressionBasedFilter<T> : ExpressionVisitor
    {
        private readonly Stack<Expression> _stack = new Stack<Expression>();

        private readonly ParameterExpression _paramExpression = Expression.Parameter(typeof(T), "x");

        public IQueryable<T> GetFilteredElements(IQueryable<T> input)
        {
            if (_stack.Count != 1)
            {
                throw new QueryLanguageException("Something went wrong while creating a filter.");
            }

            var result = _stack.Pop();

            var filter = Expression.Lambda<Func<T, bool>>(result, _paramExpression);
            return input.Where(filter);
        }

        public override void Visit(BinaryExpression binaryExpression)
        {
            var right = _stack.Pop();
            var left = _stack.Pop();
            Expression result;
            switch (binaryExpression.LogicalOperator.KeywordType)
            {
                case KeywordType.And:
                    result = Expression.AndAlso(left, right);
                    break;
                case KeywordType.Or:
                    result = Expression.OrElse(left, right);
                    break;
                default:
                    throw new QueryLanguageException("Unknown logical operator."); 
            }

            _stack.Push(result);
        }

        public override void Visit(StatementExpression statementExpression)
        {
            // creates following expressions:
            // {column} == null
            // {column} != null            

            var columnInfo = statementExpression.Column.ColumnInfo;

            Expression compare;
            switch (statementExpression.Statement.KeywordType)
            {
                case KeywordType.Empty:
                    if (statementExpression.Column.ColumnInfo.IsNullable == false)
                    {
                        compare = Expression.Constant(false);
                        break;
                    }

                    compare = ExpressionUtils.CheckForNulls(
                        _paramExpression,
                        columnInfo.MemberName,
                        expectNull: true,
                        checkLast: true);

                    if (columnInfo.ColumnType == ColumnType.String)
                    {
                        var member = ExpressionUtils.GetNestedPropertyOrField(_paramExpression, columnInfo.MemberName);
                        compare = Expression.OrElse(compare, Expression.Equal(member, Expression.Constant(string.Empty)));
                    }
                    break;
                case KeywordType.NotEmpty:
                    if (statementExpression.Column.ColumnInfo.IsNullable == false)
                    {
                        compare = Expression.Constant(true);
                        break;
                    }

                    compare = ExpressionUtils.CheckForNulls(
                        _paramExpression,
                        columnInfo.MemberName,
                        expectNull: false,
                        checkLast: true);

                    if (columnInfo.ColumnType == ColumnType.String)
                    {
                        var member = ExpressionUtils.GetNestedPropertyOrField(_paramExpression, columnInfo.MemberName);
                        var nullcheck = ExpressionUtils.CheckForNulls(_paramExpression, columnInfo.MemberName);
                        var notEmpty = Expression.AndAlso(nullcheck, Expression.NotEqual(member, Expression.Constant(string.Empty)));
                        compare = Expression.AndAlso(compare, notEmpty);
                    }
                    break;

                case KeywordType.IsTrue:
                    Visit(new CompareExpression(statementExpression.Column, 
                        new CompareOperatorToken(new Keyword("", KeywordType.Equal, TokenType.CompareOperator)), 
                        new LiteralToken(true)));
                    return;
                case KeywordType.IsFalse:
                    Visit(new CompareExpression(statementExpression.Column,
                        new CompareOperatorToken(new Keyword("", KeywordType.Equal, TokenType.CompareOperator)),
                        new LiteralToken(false)));
                    return;
                default:
                    var msg = $"Keyword {statementExpression.Statement.KeywordType} is not supported.";
                    throw new QueryLanguageException(msg);
            }

            _stack.Push(compare);
        }

        public override void Visit(CompareExpression compareExpression)
        {
            var expression = CompareExpressionTranslator.Execute(compareExpression, _paramExpression);

            if (expression == null)
            {
                return;
            }

            _stack.Push(expression);
        }
    }
}