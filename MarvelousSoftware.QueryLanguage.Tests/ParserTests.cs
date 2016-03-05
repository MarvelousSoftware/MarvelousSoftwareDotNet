using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Parsing;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Lexing.Functions;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;
using MarvelousSoftware.QueryLanguage.Models;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parser_CanCreateCorrectAST()
        {
            var query = "(S = 1 AND S != 2) OR S is empty";

            var result = new Parser().Parse(GetTokens(query));

            result.Root.Should().BeOfType<BinaryExpression>();
            var root = result.Root.As<BinaryExpression>();
            root.LogicalOperator.Should().Match<LogicalOperatorToken>(x => x.KeywordType == KeywordType.Or);
            root.Right.Should().Match<StatementExpression>(x => x.Column.ColumnInfo.ColumnName == "S" 
                && x.Statement.KeywordType == KeywordType.Empty);
            root.Left.Should().Match<BinaryExpression>(x => x.LogicalOperator.KeywordType == KeywordType.And);
            var left = root.Left.As<BinaryExpression>();
            left.Left.Should().Match<CompareExpression>(x => x.Column.ColumnInfo.ColumnName == "S" &&
                x.CompareOperator.KeywordType == KeywordType.Equal &&
                (int)x.Value.Evaluate() == 1);
            left.Right.Should().Match<CompareExpression>(x => x.Column.ColumnInfo.ColumnName == "S" &&
                x.CompareOperator.KeywordType == KeywordType.NotEqual &&
                (int)x.Value.Evaluate() == 2);
        }

        [Test]
        public void Parser_CanVisitInCorrectOrder()
        {
            var query = "((S = 1 OR S = 2) AND S != 5) OR S is empty";

            var visitor = new TestVisitor();
            new Parser().AddVisitor(visitor).Parse(GetTokens(query));
            var exprs = visitor.Expressions;

            exprs[0].Should().BeOfType<CompareExpression>();
            exprs[1].Should().BeOfType<CompareExpression>();
            exprs[2].Should().BeOfType<BinaryExpression>();
            exprs[3].Should().BeOfType<CompareExpression>();
            exprs[4].Should().BeOfType<BinaryExpression>();
            exprs[5].Should().BeOfType<StatementExpression>();
            exprs[6].Should().BeOfType<BinaryExpression>();
        }

        [Test]
        public void Parser_CanParseFunction()
        {
            var query = "Name = currentUser()";

            var visitor = new TestVisitor();
            new Parser().AddVisitor(visitor).Parse(GetTokens(query));
            var exprs = visitor.Expressions;

            exprs[0].Should().BeOfType<CompareExpression>().And.Match<CompareExpression>(x => (string)x.Value.Evaluate() == "john");
        }

        private static TokenBase[] GetTokens(string query)
        {
            var config = new LanguageConfig<Person>()
                .AddColumn("S", x => x.S)
                .AddColumn("Name", x => x.FirstName);

            config.FunctionDefinitions.Define(new FunctionDefinition("currentUser", ColumnType.String, () => "john"));

            var lexer = new Lexer<Person>(config);
            var tokens = lexer.Run(query).Tokens.ToArray();
            return tokens;
        }
    }

    public class TestVisitor : ExpressionVisitor
    {
        public readonly List<ExpressionBase> Expressions = new List<ExpressionBase>();

        public override void Visit(BinaryExpression binaryExpression)
        {
            Expressions.Add(binaryExpression);
        }

        public override void Visit(StatementExpression statementExpression)
        {
            Expressions.Add(statementExpression);
        }

        public override void Visit(CompareExpression compareExpression)
        {
            Expressions.Add(compareExpression);
        }
    }
}