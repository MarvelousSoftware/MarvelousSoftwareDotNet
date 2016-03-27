using System.Collections.Generic;

// todo: missing features:
// 1. In method: `Name IN('a','b','c')`, `Labels IN('a', 'c')` 

namespace MarvelousSoftware.QueryLanguage.Config
{
    /// <summary>
    /// Provides default implementation of <see cref="ISyntaxConfig"/>.
    /// </summary>
    public class SyntaxConfig : ISyntaxConfig
    {
        public SyntaxConfig()
        {
            _keywords = new List<Keyword>()
                {
                    new Keyword("=",            KeywordType.Equal),
                    new Keyword("!=",           KeywordType.NotEqual),
                    new Keyword(">",            KeywordType.GreaterThan),
                    new Keyword("<",            KeywordType.LessThan),
                    new Keyword(">=",           KeywordType.GreaterThanOrEqual),
                    new Keyword("<=",           KeywordType.LessThanOrEqual),
                    new Keyword("starts with",  KeywordType.StartsWith),
                    new Keyword("ends with",    KeywordType.EndsWith),
                    new Keyword("contains",     KeywordType.Contains),
                    new Keyword("is empty",     KeywordType.Empty),
                    new Keyword("is not empty", KeywordType.NotEmpty),
                    new Keyword("is true",      KeywordType.IsTrue),
                    new Keyword("is false",     KeywordType.IsFalse),
                    new Keyword("and",          KeywordType.And),
                    new Keyword("or",           KeywordType.Or)
                };
        }

        private readonly List<Keyword> _keywords;
        public IEnumerable<Keyword> Keywords => _keywords;
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

        public SyntaxConfig ReplaceKeyword(Keyword keyword)
        {
            var index = _keywords.FindIndex(x => x.KeywordType == keyword.KeywordType);
            _keywords[index] = keyword;
            return this;
        }
    }
}