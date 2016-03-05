using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests.LexerParsers
{
    [TestFixture]
    public class WhitespaceTokenizerTests
    {
        private static ITokenizer<object> Lexer => new WhitespaceTokenizer<object>();

        [Test]
        public void WhitespaceLexer_CanParseEmptyQuery()
        {
            var query = "     \t \r ";
            const int skip = 0;
            var result = Lexer.Tokenize(GetRuntimeInfo(query, skip));

            Assert.IsTrue(result.Parsed);
            Assert.AreEqual(result.ParsedToken.TokenType, TokenType.Whitespace);
            Assert.AreEqual(result.ParsedToken.StartPosition, 0);
            Assert.AreEqual(result.ParsedToken.Length, query.Length);
        }

        [Test]
        public void WhitespaceLexer_CanSkipCharsInQuery()
        {
            const string query = "     \t \r ";
            const int skip = 2;
            var result = Lexer.Tokenize(GetRuntimeInfo(query, skip));

            Assert.IsTrue(result.Parsed);
            Assert.AreEqual(result.ParsedToken.TokenType, TokenType.Whitespace);
            Assert.AreEqual(result.ParsedToken.StartPosition, skip);
            Assert.AreEqual(result.ParsedToken.Length, query.Length - skip);
        }

        [Test]
        public void WhitespaceLexer_CanParseNotEmptyQuery()
        {
            const string query = "Name     = 'Damian'";
            const int start = 4;
            const int length = 5;

            var result = Lexer.Tokenize(GetRuntimeInfo(query, start));

            Assert.IsTrue(result.Parsed);
            Assert.AreEqual(result.ParsedToken.TokenType, TokenType.Whitespace);
            Assert.AreEqual(result.ParsedToken.StartPosition, start);
            Assert.AreEqual(result.ParsedToken.Length, length);
        }

        [Test]
        public void WhitespaceLexer_CanParseNotWhitespace()
        {
            const string query = "Name     = 'Damian'";
            const int start = 1;

            var result = Lexer.Tokenize(GetRuntimeInfo(query, start));

            Assert.IsFalse(result.Parsed);
        }

        private static LexerRuntimeInfo<object> GetRuntimeInfo(string query, int position)
        {
            return new LexerRuntimeInfo<object>(query, new LanguageConfig<object>()) {CurrentPosition = position};
        }
    }
}
