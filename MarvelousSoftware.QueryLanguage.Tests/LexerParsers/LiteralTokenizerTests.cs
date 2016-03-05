using System;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.Lexer.Tokenizers;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests.LexerParsers
{
    [TestFixture]
    public class LiteralTokenizerTests
    {
        private static LiteralTokenizer<Person> Tokenizer => new LiteralTokenizer<Person>();

        [Test]
        public void LiteralLexer_CanParseString()
        {
            var value = "Some String Value";
            var query = $"\"{value}\"";

            var result = GetLexer("Name = " + query, typeof(string), null, true);

            CheckCorrectness(result, query);
            var token = CheckCorrectness(result, query);
            token.Value.Should().Be(value);
            token.Type.Should().Be(typeof (string));
        }

        [Test]
        public void LiteralLexer_CanParseStringWithoutQuotes()
        {
            var value = "Some";

            var result = GetLexer($"Name = {value} and", typeof(string), exact: true, allowedToBeGreedy: true);

            CheckCorrectness(result, value);
            var token = CheckCorrectness(result, value);
            token.Value.Should().Be(value);
            token.Type.Should().Be(typeof(string));
        }

        [Test]
        public void LiteralLexer_CanHandleReservedChar()
        {
            var value = "So(me";

            var result = GetLexer($"Name = {value} and", typeof(string), exact: true, allowedToBeGreedy: true);

            result.Parsed.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().Contain(x => x.Id == ErrorId.InvalidLiteral);
        }

        [Test]
        public void LiteralLexer_CanParseNotClosedString()
        {
            var value = "Some String Value and event longer text";
            var query = $"\"{value}";

            var result = GetLexer("Name = " + query, typeof(string), null, true);

            CheckCorrectness(result, query);
            var token = CheckCorrectness(result, query);
            token.Value.Should().Be(value);
            token.Type.Should().Be(typeof(string));
        }

        [Test]
        public void LiteralLexer_CanConfigureStringIdentifier()
        {
            var value = "some string";
            var query = $"'{value}'";

            var result = GetLexer("Name = " + query, typeof(string), x => x.StringLiteralIdentifier = '\'', true);

            CheckCorrectness(result, query);
            var token = CheckCorrectness(result, query);
            token.Value.Should().Be(value);
            token.Type.Should().Be(typeof(string));
        }

        [Test]
        public void LiteralLexer_CanParseNull()
        {
            var query = "null";
            var result = GetLexer(query, typeof(string));

            var token = CheckCorrectness(result, query);
            token.Value.Should().BeNull();
            token.Type.Should().BeNull();
        }

        [Test]
        public void LiteralLexer_CanParseDate()
        {
            var query = "5/10/2015";
            var result = GetLexer(query, typeof(DateTime), x => x.DateTimeFormats = new[] { "d/M/yyyy" });

            var token = CheckCorrectness(result, query);
            token.Value.Should().Be(new DateTime(2015, 10, 5));
            token.Type.Should().Be(typeof(DateTime));
        }

        [Test]
        public void LiteralLexer_CanParseDateWithConfiguratedFormat()
        {
            var query = "2015 - 5/10 10:28";
            var result = GetLexer(query, typeof(DateTime), x => x.DateTimeFormats = new[] { "yyyy - d/M HH:mm" });

            var token = CheckCorrectness(result, query);
            token.Value.Should().Be(new DateTime(2015, 10, 5, 10, 28, 0));
            token.Type.Should().Be(typeof(DateTime));
        }

        [Test]
        public void LiteralLexer_CanParseShort()
        {
            var query = short.MaxValue.ToString();
            var result = GetLexer(query, typeof(short));

            var token = CheckCorrectness(result, query);
            token.Value.Should().Be(short.MaxValue);
            token.Value.Should().BeOfType<short>();
            token.Type.Should().Be(typeof(short));
        }

        [Test]
        public void LiteralLexer_CanParseInt()
        {
            var query = int.MaxValue.ToString();
            var result = GetLexer(query, typeof(int));

            var token = CheckCorrectness(result, query);
            token.Value.Should().Be(int.MaxValue);
            token.Value.Should().BeOfType<int>();
            token.Type.Should().Be(typeof(int));
        }

        [Test]
        public void LiteralLexer_CanParseLong()
        {
            var query = long.MaxValue.ToString();
            var result = GetLexer(query, typeof(long));

            var token = CheckCorrectness(result, query);
            token.Value.Should().Be(long.MaxValue);
            token.Value.Should().BeOfType<long>();
            token.Type.Should().Be(typeof(long));
        }

        [Test]
        public void LiteralLexer_CanParseDecimal()
        {
            var query = "1000.65178";
            var result = GetLexer(query, typeof(decimal));

            var token = CheckCorrectness(result, query);
            token.Value.Should().Be((decimal)1000.65178);
            token.Value.Should().BeOfType<decimal>();
            token.Type.Should().Be(typeof(decimal));
        }

        [Test]
        public void LiteralLexer_CanHandleInvalidValue()
        {
            var query = "InvalidValue";
            var result = GetLexer(query, typeof(string));

            result.Parsed.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().Contain(x => x.Id == ErrorId.InvalidLiteral);
        }

        [Test]
        public void LiteralLexer_CanParseIntRightBeforeParenClose()
        {
            var query = int.MaxValue.ToString();
            var result = GetLexer(query + ")", typeof(int));

            var token = CheckCorrectness(result, query);
            token.Value.Should().Be(int.MaxValue);
            token.Value.Should().BeOfType<int>();
            token.Type.Should().Be(typeof(int));
        }

        [Test]
        public void LiteralLexer_CanParseDecimalRightBeforeParenClose()
        {
            var query = "1000.65178";
            var result = GetLexer(query + ")", typeof(decimal));

            var token = CheckCorrectness(result, query);
            token.Value.Should().Be((decimal)1000.65178);
            token.Value.Should().BeOfType<decimal>();
            token.Type.Should().Be(typeof(decimal));
        }

        private static LiteralToken CheckCorrectness(TokenizationResult result, string query)
        {
            result.Errors.Should().BeEmpty();
            result.Parsed.Should().BeTrue();
            result.ParsedToken.StartPosition.Should().Be(7);
            result.ParsedToken.Length.Should().Be(query.Length);

            result.ParsedToken.TokenType.Should().Be(TokenType.Literal);
            result.ParsedToken.Should().BeOfType<LiteralToken>();

            var token = (LiteralToken)result.ParsedToken;

            return token;
        }

        private static TokenizationResult GetLexer(string query, Type memberType, Action<DefaultSyntaxConfig> config = null, 
            bool exact = false, bool allowedToBeGreedy = false)
        {
            if (exact == false)
            {
                query = $"Name = {query} ";
            }

            var defaultConfig = new LanguageConfig<Person>()
                .AddColumn("Name", x => x.FirstName) 
                .AddColumn("LastName", x => x.LastName);

            config?.Invoke((DefaultSyntaxConfig)defaultConfig.Syntax.Config);

            var result = Tokenizer.Tokenize(new LexerRuntimeInfo<Person>(query, defaultConfig)
            {
                CurrentPosition = 7,
                LastColumn = new ColumnInfo("DoesNotMatter", "DoesNotMatter", memberType),
                AllowedToBeGreedy = allowedToBeGreedy
            });

            return result;
        }
    }
}