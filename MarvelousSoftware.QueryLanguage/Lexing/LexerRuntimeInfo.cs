using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.QueryLanguage.Lexing
{
    /// <summary>
    /// Provides information about current place to tokenize.
    /// </summary>
    public class LexerRuntimeInfo<T>
    {
        public LexerRuntimeInfo(string query, LanguageConfig<T> lang)
        {
            Query = query;
            QueryLength = query.Length;
            Lang = lang;
            LanguageSpecialChars = lang.Syntax.Config.GetLanguageSpecialChars();
        }

        /// <summary>
        /// Query to be handled.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// Length of the <see cref="Query"/>.
        /// </summary>
        public int QueryLength { get; }

        /// <summary>
        /// Position of the cursor in the <see cref="Query"/>.
        /// </summary>
        public int CurrentPosition { get; set; }

        /// <summary>
        /// Char at <see cref="CurrentPosition"/>.
        /// </summary>
        public char CurrentChar => Query[CurrentPosition];

        /// <summary>
        /// Language configuration.
        /// </summary>
        public LanguageConfig<T> Lang { get; private set; }

        /// <summary>
        /// Count of not closed parens.
        /// </summary>
        public int OpenedParensCount = 0;

        /// <summary>
        /// Last tokenized column.
        /// </summary>
        public ColumnInfo LastColumn { get; set; }

        /// <summary>
        /// Language specific special characters.
        /// </summary>
        public char[] LanguageSpecialChars { get; private set; }

        /// <summary>
        /// If true then it is allowed to swallow word even if lexer is not able
        /// to recognize if that's fine. Greediness is disabled while trying to find
        /// the end of unknown token.
        /// </summary>
        public bool AllowedToBeGreedy { get; set; }

        /// <summary>
        /// Checks wheter <see cref="CurrentPosition"/> is in range of <see cref="Query"/>.
        /// </summary>
        public bool IsInRange()
        {
            return QueryLength > CurrentPosition;
        }
    }
}
