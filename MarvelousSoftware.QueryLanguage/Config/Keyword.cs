using System;
using MarvelousSoftware.QueryLanguage.Lexing;

namespace MarvelousSoftware.QueryLanguage.Config
{
    /// <summary>
    /// Contains info about small, immutable query language elements like operators or methods.
    /// </summary>
    public class Keyword
    {
        public Keyword(string syntax, KeywordType keywordType)
        {
            Syntax = syntax;
            KeywordType = keywordType;

            switch (keywordType)
            {
                case KeywordType.Equal:
                case KeywordType.NotEqual:
                case KeywordType.GreaterThan:
                case KeywordType.GreaterThanOrEqual:
                case KeywordType.LessThan:
                case KeywordType.LessThanOrEqual:
                case KeywordType.StartsWith:
                case KeywordType.EndsWith:
                case KeywordType.Contains:
                    TokenType = TokenType.CompareOperator;
                    break;

                case KeywordType.Empty:
                case KeywordType.NotEmpty:
                case KeywordType.IsTrue:
                case KeywordType.IsFalse:
                    TokenType = TokenType.Statement;
                    break;

                case KeywordType.And:
                case KeywordType.Or:
                    TokenType = TokenType.LogicalOperator;
                    break;

                default:
                    throw new NotSupportedException();
            }
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
