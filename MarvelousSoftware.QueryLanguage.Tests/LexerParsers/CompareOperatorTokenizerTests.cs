using System;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests.LexerParsers
{
    [TestFixture]
    public class CompareOperatorTokenizerTests
    {
        private static readonly LanguageConfig<Person> DefaultConfig = new LanguageConfig<Person>()
            .AddColumn("Name", x => x.FirstName)
            .AddColumn("LastName", x => x.LastName);

        private static ITokenizer<Person> Lexer => new CompareOperatorTokenizer<Person>();

        [Test]
        public void CompareOperatorLexer_CanParseAllOperators()
        {
            SimpleCanParse(KeywordType.Equal, typeof(int));
            SimpleCanParse(KeywordType.NotEqual, typeof(int));
            SimpleCanParse(KeywordType.GreaterThan, typeof(int));
            SimpleCanParse(KeywordType.GreaterThanOrEqual, typeof(int));
            SimpleCanParse(KeywordType.LessThan, typeof(int));
            SimpleCanParse(KeywordType.LessThanOrEqual, typeof(int));
            SimpleCanParse(KeywordType.StartsWith, typeof(string));
            SimpleCanParse(KeywordType.EndsWith, typeof(string));
            SimpleCanParse(KeywordType.Contains, typeof(string));
        }

        [Test]
        public void CompareOperatorLexer_CanConfigureToBeCaseInsensitive()
        {
            var syntax = new DefaultSyntaxConfig { KeywordCaseSensitive = false };
            var config = new LanguageConfig<Person>(syntax);

            SimpleCanParse(KeywordType.StartsWith, typeof(string), "STARTS WITH", config);
            SimpleCanParse(KeywordType.EndsWith, typeof(string), "ENDS WITH", config);
            SimpleCanParse(KeywordType.Contains, typeof(string), "CONTAINS", config);
        }

        [Test]
        public void CompareOperatorLexer_CanParseWithoutSpacesInCaseOfSpecialChar()
        {
            var query = "Name='Test'";

            var result = Lexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig)
            {
                CurrentPosition = 4,
                LastColumn = new ColumnInfo("Name", "FirstName", typeof(string))
            });
            var exp = (CompareOperatorToken)result.ParsedToken;

            result.Parsed.Should().BeTrue();
            result.ParsedToken.TokenType.Should().Be(TokenType.CompareOperator);
            exp.StartPosition.Should().Be(4);
            exp.Length.Should().Be(1);
            exp.KeywordType.Should().Be(KeywordType.Equal);
        }

        [Test]
        public void CompareOperatorLexer_ErrorWhenNoSpaceAfter()
        {
            TestForError("Name starts withTest", 5, ErrorId.CompareOperatorNotFound, ErrorType.Critical);
        }

        [Test]
        public void CompareOperatorLexer_ErrorWhenNoSpaceBefore()
        {
            TestForError("Namestarts with 'Test'", 4, ErrorId.CompareOperatorNotFound, ErrorType.Critical);
        }

        [Test]
        public void CompareOperatorLexer_ErrorWhenOperatorNotFound()
        {
            TestForError("Name isSomethingSimilarTo 'Test'", 5, ErrorId.CompareOperatorNotFound, ErrorType.Critical);
        }

        [Test]
        public void CompareOperatorLexer_ErrorWhenOperatorUsedWithWrongColumnType()
        {
            TestForError(" > 1", 1, ErrorId.NotSupportedCompareOperator, ErrorType.Critical, typeof(string));
            TestForError(" >= 1", 1, ErrorId.NotSupportedCompareOperator, ErrorType.Critical, typeof(string));
            TestForError(" < 1", 1, ErrorId.NotSupportedCompareOperator, ErrorType.Critical, typeof(string));
            TestForError(" <= 1", 1, ErrorId.NotSupportedCompareOperator, ErrorType.Critical, typeof(string));
            TestForError(" starts with 1", 1, ErrorId.NotSupportedCompareOperator, ErrorType.Critical, typeof(int));
            TestForError(" ends with 1", 1, ErrorId.NotSupportedCompareOperator, ErrorType.Critical, typeof(DateTime));
            TestForError(" contains 1", 1, ErrorId.NotSupportedCompareOperator, ErrorType.Critical, typeof(decimal));
        }

        private void TestForError(string query, int position, ErrorId errorId, ErrorType errorType, Type columnType = null)
        {
            var result = Lexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig)
            {
                CurrentPosition = position,
                LastColumn = new ColumnInfo("Name", "FirstName", columnType ?? typeof(string))
            });
            var exp = (CompareOperatorToken)result.ParsedToken;

            Assert.IsFalse(result.Parsed);
            Assert.AreEqual(TokenType.CompareOperator, result.ParsedToken.TokenType);
            Assert.AreEqual(position, exp.StartPosition);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(errorId, result.Errors[0].Id);
            Assert.AreEqual(errorType, result.Errors[0].Type);
        }

        private void SimpleCanParse(KeywordType keywordType, Type columnType, string compareOperator = null, LanguageConfig<Person> config = null)
        {
            var cfg = config ?? DefaultConfig;
            var @operator = compareOperator ?? cfg.Syntax.CompareOperators.First(x => x.KeywordType == keywordType).Syntax;
            var query = "Name " + @operator + " 'Test'";

            var result = Lexer.Tokenize(new LexerRuntimeInfo<Person>(query, cfg)
            {
                CurrentPosition = 5,
                LastColumn = new ColumnInfo("Name", "FirstName", columnType)
            });
            var exp = (CompareOperatorToken)result.ParsedToken;

            Assert.IsTrue(result.Parsed, query);
            Assert.AreEqual(result.ParsedToken.TokenType, TokenType.CompareOperator, query);

            Assert.AreEqual(exp.StartPosition, 5, query);
            Assert.AreEqual(exp.Length, @operator.Length, query);
            Assert.AreEqual(exp.KeywordType, keywordType, query);
        }
    }
}
