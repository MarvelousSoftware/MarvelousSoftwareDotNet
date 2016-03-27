using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests.LexerParsers
{
    [TestFixture]
    public class ColumnTokenizerTests
    {
        private static readonly LanguageConfig<Person> DefaultConfig = new LanguageConfig<Person>()
            .AddColumn("Name", x => x.FirstName)
            .AddColumn("LastName", x => x.LastName);

        private static ColumnTokenizer<Person> Tokenizer => new ColumnTokenizer<Person>();

        [Test]
        public void ColumnLexer_CanParseColumnAtStart()
        {
            var query = "Name";
            var columnName = query;
            var result = Tokenizer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig));
            var column = result.ParsedToken as ColumnToken;

            Assert.IsTrue(result.Parsed);
            Assert.AreEqual(result.ParsedToken.TokenType, TokenType.Column);
            Assert.AreEqual(column.StartPosition, 0);
            Assert.AreEqual(column.Length, columnName.Length);
            Assert.AreEqual(column.ColumnInfo.ColumnName, columnName);
            Assert.AreEqual(column.ColumnInfo.MemberName, "FirstName");
            Assert.AreEqual(column.ColumnInfo.SystemType, typeof(Person).GetProperty("FirstName").PropertyType);
        }

        [Test]
        public void ColumnLexer_CanParseWithSkip()
        {
            var query = "some kind Name of test";
            var columnName = "Name";
            var skip = 10;
            var result = Tokenizer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig) { CurrentPosition = skip });
            var column = result.ParsedToken as ColumnToken;

            Assert.IsTrue(result.Parsed);
            Assert.AreEqual(result.ParsedToken.TokenType, TokenType.Column);
            Assert.AreEqual(column.StartPosition, skip);
            Assert.AreEqual(column.Length, columnName.Length);
            Assert.AreEqual(column.ColumnInfo.ColumnName, columnName);
            Assert.AreEqual(column.ColumnInfo.MemberName, "FirstName");
            Assert.AreEqual(column.ColumnInfo.SystemType, typeof(Person).GetProperty("FirstName").PropertyType);
        }

        [Test]
        public void ColumnLexer_CanConfigurateCaseSensivity()
        {
            var config = new LanguageConfig<Person>().Syntax(x => x.ColumnNameCaseSensitive = false).AddColumn("Name", x => x.FirstName);
            var parser = new ColumnTokenizer<Person>();

            var query = "nAmE some test";
            var columnName = "Name";
            var result = parser.Tokenize(new LexerRuntimeInfo<Person>(query, config));
            var column = result.ParsedToken as ColumnToken;

            Assert.IsTrue(result.Parsed);
            Assert.AreEqual(result.ParsedToken.TokenType, TokenType.Column);
            Assert.AreEqual(column.StartPosition, 0);
            Assert.AreEqual(column.Length, columnName.Length);
            Assert.AreEqual(column.ColumnInfo.ColumnName, columnName);
            Assert.AreEqual(column.ColumnInfo.MemberName, "FirstName");
            Assert.AreEqual(column.ColumnInfo.SystemType, typeof(Person).GetProperty("FirstName").PropertyType);
        }

        [Test]
        public void ColumnLexer_ErrorWhenColumnNotFound()
        {
            var query = "some kind of test query";
            var columnName = "some";
            var result = Tokenizer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig));
            var column = result.ParsedToken as ColumnToken;

            Assert.IsFalse(result.Parsed);
            Assert.AreEqual(result.ParsedToken.TokenType, TokenType.Column);
            Assert.AreEqual(column.StartPosition, 0);
            Assert.AreEqual(column.Length, columnName.Length);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.IsTrue(result.Errors[0].Id == ErrorId.ColumnNotFound);
            Assert.IsTrue(result.Errors[0].Type == ErrorType.Critical);
        }

        [Test]
        public void ColumnLexer_EndsOnInvalidChar()
        {
            var query = "some!kind of test query";
            var columnName = "some";
            var result = Tokenizer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig));
            var column = result.ParsedToken as ColumnToken;

            Assert.IsFalse(result.Parsed);
            Assert.AreEqual(result.ParsedToken.TokenType, TokenType.Column);
            Assert.AreEqual(column.StartPosition, 0);
            Assert.AreEqual(column.Length, columnName.Length);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.IsTrue(result.Errors[0].Id == ErrorId.ColumnNotFound);
            Assert.IsTrue(result.Errors[0].Type == ErrorType.Critical);
        }
    }
}
