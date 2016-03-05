using MarvelousSoftware.QueryLanguage.Lexer;

namespace MarvelousSoftware.QueryLanguage.Config
{
    /// <summary>
    /// Contains info about small, immutable query language elements like operators or methods.
    /// </summary>
    public class Keyword
    {
        public Keyword(string syntax, KeywordType keywordType, TokenType tokenType)
        {
            Syntax = syntax;
            KeywordType = keywordType;
            TokenType = tokenType;
        }

        /// <summary>
        /// A string which will be searched in the query in order to match a keyword.
        /// </summary>
        public string Syntax { get; }

        /// <summary>
        /// A type of a keyword.
        /// </summary>
        public KeywordType KeywordType { get; }

        /// <summary>
        /// An expression which is associated with keyword.
        /// </summary>
        public TokenType TokenType { get; }

        public override string ToString()
        {
            return KeywordType.ToString();
        }
    }
}
