using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Lexing;

// todo: missing features:
// 1. In method: `Name IN('a','b','c')`, `Labels IN('a', 'c')` 

namespace MarvelousSoftware.QueryLanguage.Config
{
    /// <summary>
    /// Provides default implementation of <see cref="ISyntaxConfig"/>.
    /// </summary>
    public class DefaultSyntaxConfig : ISyntaxConfig
    {
        public DefaultSyntaxConfig()
        {
            Keywords = new List<Keyword>()
                {
                    new Keyword("=",            KeywordType.Equal,              TokenType.CompareOperator),
                    new Keyword("!=",           KeywordType.NotEqual,           TokenType.CompareOperator),
                    new Keyword(">",            KeywordType.GreaterThan,        TokenType.CompareOperator),
                    new Keyword("<",            KeywordType.LessThan,           TokenType.CompareOperator),
                    new Keyword(">=",           KeywordType.GreaterThanOrEqual, TokenType.CompareOperator),
                    new Keyword("<=",           KeywordType.LessThanOrEqual,    TokenType.CompareOperator),
                    new Keyword("starts with",  KeywordType.StartsWith,         TokenType.CompareOperator),
                    new Keyword("ends with",    KeywordType.EndsWith,           TokenType.CompareOperator),
                    new Keyword("contains",     KeywordType.Contains,           TokenType.CompareOperator),
                    new Keyword("is empty",     KeywordType.Empty,              TokenType.Statement),
                    new Keyword("is not empty", KeywordType.NotEmpty,           TokenType.Statement),
                    new Keyword("is true",      KeywordType.IsTrue,             TokenType.Statement),
                    new Keyword("is false",     KeywordType.IsFalse,            TokenType.Statement),
                    new Keyword("and",          KeywordType.And,                TokenType.LogicalOperator),
                    new Keyword("or",           KeywordType.Or,                 TokenType.LogicalOperator)
                };
        }

        public IEnumerable<Keyword> Keywords { get; set; }
        public bool KeywordCaseSensitive { get; set; } = false;

        public IEnumerable<string> DateTimeFormats { get; set; } = new[] {"d/M/yyyy H:m", "d/M/yyyy"};
        public bool ColumnNameCaseSensitive { get; set; } = false;

        public string NullConstant { get; set; } = "null";
        public bool NullConstantCaseSensitive { get; set; } = false;

        public char NumberDecimalSeparator { get; set; } = '.';

        public char ParenOpen { get; set; } = '(';
        public char ParenClose { get; set; } = ')';

        public char ParamsOpen { get; set; } = '(';
        public char ParamsClose { get; set; } = ')';

        public char StringLiteralIdentifier { get; set; } = '"';
    }
}