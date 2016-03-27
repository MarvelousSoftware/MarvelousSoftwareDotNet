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
    public class LogicalOperatorTokenizerTests
    {
        private static readonly LanguageConfig<Person> DefaultConfig = new LanguageConfig<Person>()
            .AddColumn("Name", x => x.FirstName)
            .AddColumn("LastName", x => x.LastName);

        private static ITokenizer<Person> Lexer => new LogicalOperatorTokenizer<Person>();

        [Test]
        public void LogicalOperatorLexer_CanParseAllOperators()
        {
            foreach (var logicalOperator in DefaultConfig.SyntaxConfig.LogicalOperators)
                SimpleCanParse(logicalOperator.Syntax, logicalOperator.KeywordType);
        }

        [Test]
        public void LogicalOperatorLexer_CanConfigureToBeCaseInsensitive()
        {
            var config = new LanguageConfig<Person>().Syntax(x => x.KeywordCaseSensitive = false);

            foreach (var logicalOperator in DefaultConfig.SyntaxConfig.LogicalOperators)
                SimpleCanParse(logicalOperator.Syntax.ToUpper(), logicalOperator.KeywordType, config);
        }

        [Test]
        public void LogicalOperatorLexer_ErrorWhenOperatorNotFound()
        {
            TestForError("Name = 'Test' AndOrNotAnd", 14, TokenType.LogicalOperator, ErrorId.LogicalOperatorNotFound, ErrorType.Critical);
        }

        private void TestForError(string query, int position, TokenType tokenType, ErrorId errorId, ErrorType errorType)
        {
            var result = Lexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig) { CurrentPosition = position });
            var token = (LogicalOperatorToken)result.ParsedToken;

            token.StartPosition.Should().Be(token.StartPosition);
            result.Parsed.Should().BeFalse();
            result.ParsedToken.TokenType.Should().Be(tokenType);
            result.Errors.Should().HaveCount(1);
            result.Errors[0].Id.Should().Be(errorId);
            result.Errors[0].Type.Should().Be(errorType);
        }

        private void SimpleCanParse(string logicalOperator, KeywordType keywordType, LanguageConfig<Person> config = null)
        {
            var query = "Name = \"Test\" "+logicalOperator+" LastName";
            //                           ^

            var result = Lexer.Tokenize(new LexerRuntimeInfo<Person>(query, config ?? DefaultConfig){CurrentPosition = 14});
            var token = (LogicalOperatorToken)result.ParsedToken;

            result.Parsed.Should().BeTrue();
            result.ParsedToken.TokenType.Should().Be(TokenType.LogicalOperator);
            token.StartPosition.Should().Be(14);
            token.Length.Should().Be(logicalOperator.Length);
            token.KeywordType.Should().Be(keywordType);
        }
    }
}
