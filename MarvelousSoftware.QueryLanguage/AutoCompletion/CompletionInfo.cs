using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion
{
    public class CompletionInfo
    {
        public TokenType TokenType { get; set; }
        public string ComparePart { get; set; }
        public ColumnInfo ColumnInfo { get; set; }
        public int BatchSize { get; set; }
    }
}