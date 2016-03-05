namespace MarvelousSoftware.QueryLanguage.Lexer
{
    /// <summary>
    /// Allows to parse a query.
    /// </summary>
    public interface ILexer
    {
        /// <summary>
        /// Parses given query.
        /// </summary>
        LexerResult Run(string query, LexerConfig lexerConfig = null);
    }
}
