using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing
{
    public class TokenizersProvider<T> : ITokenizersProvider<T>
    {
        public IReadOnlyDictionary<TokenType, ITokenizer<T>> Tokenizers { get; private set; } = new Dictionary<TokenType, ITokenizer<T>>()
        {
            {TokenType.Whitespace, new WhitespaceTokenizer<T>()},
            {TokenType.CompareOperator, new CompareOperatorTokenizer<T>()},
            {TokenType.Column, new ColumnTokenizer<T>()},
            {TokenType.Function, new FunctionTokenizer<T>()},
            {TokenType.Literal, new LiteralTokenizer<T>()},
            {TokenType.LogicalOperator, new LogicalOperatorTokenizer<T>()},
            {TokenType.ParenOpen, new ParenOpenTokenizer<T>()},
            {TokenType.ParenClose, new ParenCloseTokenizer<T>()},
            {TokenType.Statement, new StatementTokenizer<T>()}
        };

        public ITokenizer<T> Get(TokenType tokenType)
        {
            return Tokenizers[tokenType];
        }

        public IEnumerable<TokenType> GetTokenTypesNextTo(TokenType? tokenType)
        {
            return Tokenizers
                .Where(x => x.Value.ShouldTokenizeAfter(tokenType))
                .Select(x => x.Key);
        }

        public void AddAfter(TokenType tokenType, ITokenizer<T> tokenizer)
        {
            var result = new Dictionary<TokenType, ITokenizer<T>>();
            foreach (var t in Tokenizers)
            {
                result.Add(t.Key, t.Value);

                if (t.Key == tokenType)
                {
                    result.Add(tokenType, tokenizer);
                }
            }

            Tokenizers = result;
        }

        public void AddBefore(TokenType tokenType, ITokenizer<T> tokenizer)
        {
            var result = new Dictionary<TokenType, ITokenizer<T>>();
            foreach (var t in Tokenizers)
            {
                if (t.Key == tokenType)
                {
                    result.Add(tokenType, tokenizer);
                }
                result.Add(t.Key, t.Value);
            }

            Tokenizers = result;
        }

        public void Replace(TokenType tokenType, ITokenizer<T> tokenizer)
        {
            var result = new Dictionary<TokenType, ITokenizer<T>>();
            foreach (var t in Tokenizers)
            {
                if (t.Key == tokenType)
                {
                    result.Add(tokenType, tokenizer);
                    continue;
                }
                result.Add(t.Key, t.Value);
            }
            Tokenizers = result;
        }

        // TODO: allow to add custom tokenizers (TokenType cannot be an enum?)
    }
}
