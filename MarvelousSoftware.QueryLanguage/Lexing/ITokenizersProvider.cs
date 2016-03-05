using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing
{
    /// <summary>
    /// Holds all tokenizers and provides methods to get them.
    /// </summary>
    public interface ITokenizersProvider<T>
    {
        /// <summary>
        /// Gets lexer by <see cref="tokenType"/>.
        /// </summary>
        ITokenizer<T> Get(TokenType tokenType);

        /// <summary>
        /// Gets all token types which could be specied after given <see cref="tokenType"/>.
        /// </summary>
        IEnumerable<TokenType> GetTokenTypesNextTo(TokenType? tokenType);

        void AddAfter(TokenType tokenType, ITokenizer<T> tokenizer);
        void AddBefore(TokenType tokenType, ITokenizer<T> tokenizer);
        void Replace(TokenType tokenType, ITokenizer<T> tokenizer);
    }
}
