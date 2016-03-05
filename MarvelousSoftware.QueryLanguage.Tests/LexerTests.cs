using System;
using System.Diagnostics;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Lexing.Functions;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests
{
    [TestFixture]
    public class LexerTests
    {
        private static readonly LanguageConfig<Person> DefaultConfig = new LanguageConfig<Person>()
           .AddColumn("Name", x => x.FirstName)
           .AddColumn("LastName", x => x.LastName)
           .AddColumn("S", x => x.S);

        private static ILexer Lexer => new Lexer<Person>(DefaultConfig);

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            DefaultConfig.FunctionDefinitions.Define(new FunctionDefinition("currentUser", ColumnType.String, () => "john"));
        }

        [Test]
        public void Lexer_CanParseAllTokenTypes()
        {
            // todo: statements
            var query = " Name   starts with \"D\"  or (S=2)";

            var result = Lexer.Run(query);

            result.Errors.Should().BeEmpty();
            result.Tokens.Should().HaveCount(14);
            result.Tokens[0].Should().BeOfType<WhitespaceToken>();
            result.Tokens[1].Should().BeOfType<ColumnToken>().And.Match<ColumnToken>(x => x.ColumnInfo.ColumnName == "Name");
            result.Tokens[2].Should().BeOfType<WhitespaceToken>();
            result.Tokens[3].Should().BeOfType<CompareOperatorToken>()
                .And.Match<CompareOperatorToken>(x => x.Keyword.KeywordType == KeywordType.StartsWith);
            result.Tokens[4].Should().BeOfType<WhitespaceToken>();
            result.Tokens[5].Should().BeOfType<LiteralToken>()
                .And.Match<LiteralToken>(x => (string)x.Value == "D");
            result.Tokens[6].Should().BeOfType<WhitespaceToken>();

            result.Tokens[7].Should().BeOfType<LogicalOperatorToken>()
                .And.Match<LogicalOperatorToken>(x => x.Keyword.KeywordType == KeywordType.Or);

            result.Tokens[9].Should().BeOfType<ParenOpenToken>();

            result.Tokens[13].Should().BeOfType<ParenCloseToken>();
        }

        [Test]
        public void Lexer_CanHandleInvalidQuery()
        {
            var query = "NotExistingColumn = \"Test\"";

            var result = Lexer.Run(query);

            result.Errors[0].Should().Match<Error>(x => x.Id == ErrorId.ParenOpenNotFound);
            result.Errors[1].Should().Match<Error>(x => x.Id == ErrorId.ColumnNotFound);
        }

        [Test]
        public void Lexer_CanHandleNotOpenedParen()
        {
            var query = "Name starts with \"D\")";

            var result = Lexer.Run(query);

            result.Errors.Count.Should().BeGreaterThan(0);
            result.Errors.Any(x => x.Id == ErrorId.ParenCloseWithoutParenOpen).Should().BeTrue();
        }

        [Test]
        public void Lexer_CanHandleUnkownToken()
        {
            var column = "FirstName ";
            var query = $"{column}is empty";

            var result = Lexer.Run(query, new LexerConfig()
            {
                AllowUnknownToken = true
            });

            result.Errors.Any().Should().BeFalse();
            result.Tokens[0].TokenType.Should().Be(TokenType.Unknown);
            var token = result.Tokens[0].As<UnknownToken>();
            token.Text.Should().Be(column);
        }

        [Test]
        public void Lexer_CanHandleFunctionToken()
        {
            var query = "Name = currentUser()";

            var result = Lexer.Run(query);

            result.Errors.Should().BeEmpty();
            result.Tokens.Should().HaveCount(5);
            result.Tokens[4].Should().BeOfType<FunctionToken>().And.Match<FunctionToken>(x => x.Name == "currentUser");
        }

        [Ignore]
        [Test]
        public void Lexer_TestPerformance()
        {
            var query = "Name starts with \"D\" or (S=2)";

            // warm up
            Lexer.Run(query);

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            Lexer.Run(query);
            stopWatch.Stop();

            Console.WriteLine(stopWatch.Elapsed);
            Console.WriteLine(stopWatch.ElapsedMilliseconds + "ms");
            Console.WriteLine(stopWatch.ElapsedTicks + " ticks");
        }
    }
}