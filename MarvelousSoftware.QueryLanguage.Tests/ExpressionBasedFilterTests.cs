using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Execution;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using NUnit.Framework;
using BinaryExpression = MarvelousSoftware.QueryLanguage.Parsing.Expressions.BinaryExpression;

namespace MarvelousSoftware.QueryLanguage.Tests
{
    public class ExpressionBasedFilterTests
    {
        public readonly IQueryable<Employee> Data = DataSample.GetEmployees();

        [Test]
        public void ExpressionBasedFilter_ShouldWork()
        {
            var syntax = new DefaultSyntaxConfig();
            var config = new LanguageConfig<Employee>().AddColumn(x => x.Salary);

            var compare1 = new CompareExpression(new ColumnToken(config.Columns.Single()),
                new CompareOperatorToken(syntax.Keywords.First(x => x.KeywordType == KeywordType.GreaterThan)),
                new LiteralToken(8000));

            var compare2 = new CompareExpression(new ColumnToken(config.Columns.Single()),
                new CompareOperatorToken(syntax.Keywords.First(x => x.KeywordType == KeywordType.LessThan)),
                new LiteralToken(18000));

            var root = new BinaryExpression(compare1, 
                new LogicalOperatorToken(config.Syntax.LogicalOperators.First(x => x.KeywordType == KeywordType.And)), 
                compare2);

            Test(root, x => x.Salary.HasValue && x.Salary > 8000 && x.Salary < 18000);
        }

        [Test]
        public void ExpressionBasedFilter_ShouldReturnNoElementsIfFilteringForNullsOnNonNullableColumn()
        {
            var syntax = new DefaultSyntaxConfig();
            var config = new LanguageConfig<Employee>().AddColumn(x => x.FunLevel);

            var root = new StatementExpression(new ColumnToken(config.Columns.Single()),
                new StatementToken(syntax.Keywords.First(x => x.KeywordType == KeywordType.Empty)));

            Test(root, x => false, checkForOutputSize: false);
        }

        [Test]
        public void ExpressionBasedFilter_ShouldReturnAllElementsIfFilteringForNotNullsOnNonNullableColumn()
        {
            var syntax = new DefaultSyntaxConfig();
            var config = new LanguageConfig<Employee>().AddColumn(x => x.FunLevel);

            var root = new StatementExpression(new ColumnToken(config.Columns.Single()),
                new StatementToken(syntax.Keywords.First(x => x.KeywordType == KeywordType.NotEmpty)));

            Test(root, x => true, checkForOutputSize: false);
        }

        private void Test(ExpressionBase expression, Expression<Func<Employee, bool>> predicate, bool checkForOutputSize = true)
        {
            var expected = Data.Where(predicate).ToArray();

            var filter = new ExpressionBasedFilter<Employee>();
            expression.Visit(filter);
            var received = filter.GetFilteredElements(Data).ToArray();

            if (checkForOutputSize)
            {
                expected.Length.Should()
                    .NotBe(Data.Count(), "Test case should not return full collection of elements. May lead to false-positives.")
                    .And.NotBe(0, "Test case should be desined to return at least one element.");
            }

            received.Should().BeEquivalentTo<Employee>(expected);
        }
    }
}