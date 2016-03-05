using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokenizers
{
    internal sealed class LogicalOperatorTokenizer<T> : KeywordTokenizerBase<T, LogicalOperatorToken>
    {
        public override bool ShouldTokenizeAfter(TokenType? token)
            => token == TokenType.Literal || token == TokenType.Function || token == TokenType.Statement || token == TokenType.ParenClose;
        
        public override LogicalOperatorToken GetToken(Keyword keyword) => new LogicalOperatorToken(keyword);

        public override IEnumerable<Keyword> GetPossibleKeywords(LexerRuntimeInfo<T> info)
        {
            return info.Lang.Syntax.LogicalOperators;
        }

        public override Error OnNotFoundError(string word)
        {
            return new Error($"\"{word}\" is not a logical operator.", ErrorId.LogicalOperatorNotFound, ErrorType.Critical, word);
        }
    }
}