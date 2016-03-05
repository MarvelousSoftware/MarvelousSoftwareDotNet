using System;
using System.Linq;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;
using MarvelousSoftware.QueryLanguage.Lexer.Functions;
using MarvelousSoftware.QueryLanguage.Lexer.Tokenizers;
using MarvelousSoftware.QueryLanguage.Lexer.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests.LexerParsers
{
    public class FunctionTokenizerTests
    {
        private static readonly LanguageConfig<Person> DefaultConfig = new LanguageConfig<Person>()
            .AddColumn("Name", x => x.FirstName);

        private static ITokenizer<Person> Tokenizer => new FunctionTokenizer<Person>();

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            DefaultConfig.FunctionDefinitions.Define(new FunctionDefinition("currentUser", ColumnType.String, () => "john"));
            DefaultConfig.FunctionDefinitions.Define(new FunctionDefinition("foo", ColumnType.String, () => "foobar"));
            DefaultConfig.FunctionDefinitions.Define(new FunctionDefinition("day", ColumnType.Integer, () => 10));
        }

        [Test]
        public void FunctionTokenizer_ShouldTokenizeFunctionWithoutParams()
        {
            var func = "currentUser()";

            var result = GetTokenizer(func);

            result.Errors.Should().BeEmpty();
            result.Parsed.Should().BeTrue();
            result.ParsedToken.Length.Should().Be(func.Length);
            result.ParsedToken.TokenType.Should().Be(TokenType.Function);
            var token = result.ParsedToken.As<FunctionToken>();
            token.Name.Should().Be("currentUser");
            token.Parameters.Should().BeEmpty();
            token.Evaluate().Should().Be("john");
        }

        [Test]
        public void FunctionTokenizer_ShouldReturnErrorIfInvalidName()
        {
            var func = "current User()";

            var result = GetTokenizer(func);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].Id.Should().Be(ErrorId.InvalidFunctionName);
            result.Parsed.Should().BeFalse();
        }

        [Test]
        public void FunctionTokenizer_ShouldReturnErrorIfSpaceBeforeParamOpen()
        {
            var func = "currentUser ()";

            var result = GetTokenizer(func);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].Id.Should().Be(ErrorId.InvalidFunctionName);
            result.Parsed.Should().BeFalse();
        }

        [Test]
        public void FunctionTokenizer_ShouldReturnErrorIfAtTheEndOfQueryAndNotClosed()
        {
            var func = "currentUser(";

            var result = GetTokenizer(func);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].Id.Should().Be(ErrorId.FunctionParamsCloseMissing);
            result.Parsed.Should().BeFalse();
        }

        [Test]
        public void FunctionTokenizer_ShouldReturnErrorIfIncompatibleType()
        {
            var func = "day()";

            var result = GetTokenizer(func);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].Id.Should().Be(ErrorId.FunctionReturnsIncompatibleType);
            result.Parsed.Should().BeFalse();
        }

        [Test]
        public void FunctionTokenizer_ShouldIgnoreStringLiteral()
        {
            var func = "\"currentUser()\"";

            var result = GetTokenizer(func);

            result.Parsed.Should().BeFalse();
            result.Errors.Should().BeEmpty();
        }

        private static TokenizationResult GetTokenizer(string query)
        {
            query = $"Name = {query}";

            var result = Tokenizer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig)
            {
                CurrentPosition = 7,
                LastColumn = new ColumnInfo("Name", "Name", typeof(string))
            });

            return result;
        }

        [Test]
        [Ignore]
        public void FunctionTokenizer_ShouldTokenizeFunctionWithParamsDefined()
        {
            var func = "foo(\"bar,\", 10, 10/10/15)";

            var result = GetTokenizer(func);

            result.Errors.Should().BeEmpty();
            result.Parsed.Should().BeTrue();
            result.ParsedToken.TokenType.Should().Be(TokenType.Function);
            var token = result.ParsedToken.As<FunctionToken>();
            token.Name.Should().Be("foo");
            token.Parameters.Count().Should().Be(3);
            token.Parameters.ElementAt(0).Value.Should().Be("bar,");
            token.Parameters.ElementAt(1).Value.Should().Be(10);
            token.Parameters.ElementAt(2).Value.Should().Be(new DateTime(2015, 10, 10));
        }
    }
}