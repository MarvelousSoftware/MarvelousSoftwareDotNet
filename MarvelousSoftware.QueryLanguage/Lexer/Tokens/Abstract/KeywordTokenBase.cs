using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract
{
    /// <summary>
    /// Base class for all keyword based tokens.
    /// </summary>
    public abstract class KeywordTokenBase : TokenBase
    {
        /// <summary>
        /// Compare operator type.
        /// </summary>
        public readonly Keyword Keyword;

        /// <summary>
        /// Type of a keyword. Just a shortcut for 'Keyword.KeywordType'.
        /// Returns null if Keyword is not defined.
        /// </summary>
        public KeywordType KeywordType => Keyword.KeywordType;

        protected KeywordTokenBase(Keyword keyword)
        {
            Keyword = keyword;
        }

        public override string ToString()
        {
            return Keyword.Syntax;
        }
    }
}