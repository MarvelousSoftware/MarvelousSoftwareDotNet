namespace MarvelousSoftware.QueryLanguage.Lexer.Tokenizers.Abstract
{
    public interface ITokenizer<T>
    {
        /// <summary>
        /// Returns true if should run.
        /// </summary>
        bool ShouldTokenizeAfter(TokenType? token);

        /// <summary>
        /// Tries to tokenize an expression at <see cref="LexerRuntimeInfo{T}.CurrentPosition"/>.
        /// </summary>
        TokenizationResult Tokenize(LexerRuntimeInfo<T> info);
    }
}
