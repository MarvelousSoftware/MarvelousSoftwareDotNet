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
    public class StatementTokenizerTests
    {
        private static readonly LanguageConfig<Person> DefaultConfig = new LanguageConfig<Person>()
            .AddColumn("Name", x => x.FirstName)
            .AddColumn("LastName", x => x.LastName);

        private static ITokenizer<Person> Lexer => new StatementTokenizer<Person>();

        [Test]
        public void StatementLexer_CanParseAllStatements()
        {
            foreach (var statement in DefaultConfig.SyntaxConfig.Statements)
                SimpleCanParse(statement.Syntax, statement.KeywordType);
        }

        [Test]
        public void StatementLexer_CanConfigureToBeCaseInsensitive()
        {
            var config = new LanguageConfig<Person>().Syntax(x => x.KeywordCaseSensitive = false);

            foreach (var statement in DefaultConfig.SyntaxConfig.Statements)
                SimpleCanParse(statement.Syntax.ToUpper(), statement.KeywordType, config);
        }

        [Test]
        public void StatementLexer_ErrorWhenNotFound()
        {
            TestForError("Name is or isnt 'Test'", 5, TokenType.Statement, ErrorId.StatementNotFound, ErrorType.Critical);
        }

        private void TestForError(string query, int position, TokenType tokenType, ErrorId errorId, ErrorType errorType)
        {
            var result = Lexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig) { CurrentPosition = position });
            var token = (StatementToken)result.ParsedToken;

            token.StartPosition.Should().Be(token.StartPosition);
            result.Parsed.Should().BeFalse();
            result.ParsedToken.TokenType.Should().Be(tokenType);
            result.Errors.Should().HaveCount(1);
            result.Errors[0].Id.Should().Be(errorId);
            result.Errors[0].Type.Should().Be(errorType);
        }

        private void SimpleCanParse(string statement, KeywordType keywordType, LanguageConfig<Person> config = null)
        {
            var query = "Name "+statement+" and";
            //                ^           

            var result = Lexer.Tokenize(new LexerRuntimeInfo<Person>(query, config ?? DefaultConfig){CurrentPosition = 5});
            var token = (StatementToken)result.ParsedToken;

            result.Parsed.Should().BeTrue();
            result.ParsedToken.TokenType.Should().Be(TokenType.Statement);
            token.StartPosition.Should().Be(5);
            token.Length.Should().Be(statement.Length);
            token.KeywordType.Should().Be(keywordType);
        }
    }
}
