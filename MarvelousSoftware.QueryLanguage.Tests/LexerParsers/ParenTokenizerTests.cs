using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.Lexer.Tokenizers;
using MarvelousSoftware.QueryLanguage.Lexer.Tokenizers.Abstract;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests.LexerParsers
{
    [TestFixture]
    public class ParenTokenizerTests
    {
        private static readonly LanguageConfig<Person> DefaultConfig = new LanguageConfig<Person>()
            .AddColumn("Name", x => x.FirstName)
            .AddColumn("LastName", x => x.LastName);

        private static ITokenizer<Person> ParenOpenLexer
        {
            get { return new ParenOpenTokenizer<Person>(); }
        }

        private static ITokenizer<Person> ParenCloseLexer
        {
            get { return new ParenCloseTokenizer<Person>(); }
        }

        [Test]
        public void ParenLexer_CanParseOpenParenAtTheStart()
        {
            var query = "(";

            var result = ParenOpenLexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig) { CurrentPosition = 0 });
            var token = (ParenOpenToken)result.ParsedToken;

            result.Parsed.Should().BeTrue();
            result.ParsedToken.TokenType.Should().Be(TokenType.ParenOpen);
            token.StartPosition.Should().Be(0);
            token.Length.Should().Be(1);
        }

        [Test]
        public void ParenLexer_CanParseCloseParenAtTheEnd()
        {
            var query = "(S = 1)";
            //                 ^

            var result = ParenCloseLexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig) { CurrentPosition = 6, OpenedParensCount = 1 });
            var token = (ParenCloseToken)result.ParsedToken;

            result.Parsed.Should().BeTrue();
            result.ParsedToken.TokenType.Should().Be(TokenType.ParenClose);
            token.StartPosition.Should().Be(6);
            token.Length.Should().Be(1);
        }

        [Test]
        public void ParenLexer_CanParseOpenParenInTheMiddle()
        {
            var query = "S = 1 and (S = 3 or S is empty)";
            //                     ^

            var result = ParenOpenLexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig) { CurrentPosition = 10 });
            var token = (ParenOpenToken)result.ParsedToken;

            result.Parsed.Should().BeTrue();
            result.ParsedToken.TokenType.Should().Be(TokenType.ParenOpen);
            token.StartPosition.Should().Be(10);
            token.Length.Should().Be(1);
        }

        [Test]
        public void ParenLexer_CanParseCloseParenInTheMiddle()
        {
            var query = "(S = 1) and";
            //                 ^

            var result = ParenCloseLexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig) { CurrentPosition = 6, OpenedParensCount = 1 });
            var token = (ParenCloseToken)result.ParsedToken;

            result.Parsed.Should().BeTrue();
            result.ParsedToken.TokenType.Should().Be(TokenType.ParenClose);
            token.StartPosition.Should().Be(6);
            token.Length.Should().Be(1);
        }

        [Test]
        public void ParenLexer_ErrorWhenOperatorNotFound()
        {
            TestForError("Some Kind", 5, ErrorId.ParenOpenNotFound, ErrorType.Critical, ParenOpenLexer);
            TestForError("Some Kind", 5, ErrorId.ParenCloseNotFound, ErrorType.Critical, ParenCloseLexer);
        }

        [Test]
        public void ParenLexer_ErrorWhenParenOpenNotFound()
        {
            var query = "S = 1) and";
            //                ^

            var result = ParenCloseLexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig) { CurrentPosition = 5, OpenedParensCount = 0 });

            result.Parsed.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].Id.Should().Be(ErrorId.ParenCloseWithoutParenOpen);
            result.Errors[0].Type.Should().Be(ErrorType.Critical);
        }

        private void TestForError(string query, int position, ErrorId errorId, ErrorType errorType, ITokenizer<Person> lexer)
        {
            var result = lexer.Tokenize(new LexerRuntimeInfo<Person>(query, DefaultConfig) { CurrentPosition = position });

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Id.Should().Be(errorId);
            result.Errors[0].Type.Should().Be(errorType);
        }
    }
}
