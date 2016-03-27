using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokenizers
{
    internal sealed class StatementTokenizer<T> : KeywordTokenizerBase<T, StatementToken>
    {
        public override bool ShouldTokenizeAfter(TokenType? token) =>
            token == TokenType.Column;

        public override StatementToken GetToken(Keyword keyword) => new StatementToken(keyword);

        public override IEnumerable<Keyword> GetPossibleKeywords(LexerRuntimeInfo<T> info)
        {
            return info.Lang.SyntaxConfig.Statements;
        }

        public override Error OnNotFoundError(string word)
        {
            return new Error($"\"{word}\" is not a statement.", ErrorId.StatementNotFound, ErrorType.Critical, word);
        }
    }
}