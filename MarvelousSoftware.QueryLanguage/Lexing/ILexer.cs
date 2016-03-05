namespace MarvelousSoftware.QueryLanguage.Lexing
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
