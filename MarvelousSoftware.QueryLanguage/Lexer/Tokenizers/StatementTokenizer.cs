using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokenizers
{
    internal sealed class StatementTokenizer<T> : KeywordTokenizerBase<T, StatementToken>
    {
        public override bool ShouldTokenizeAfter(TokenType? token) =>
            token == TokenType.Column;

        public override StatementToken GetToken(Keyword keyword) => new StatementToken(keyword);

        public override IEnumerable<Keyword> GetPossibleKeywords(LexerRuntimeInfo<T> info)
        {
            return info.Lang.Syntax.Statements;
        }

        public override Error OnNotFoundError(string word)
        {
            return new Error($"\"{word}\" is not a statement.", ErrorId.StatementNotFound, ErrorType.Critical, word);
        }
    }
}