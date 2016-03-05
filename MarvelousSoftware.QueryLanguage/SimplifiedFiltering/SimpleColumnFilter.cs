using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.QueryLanguage.SimplifiedFiltering
{
    public class SimpleColumnFilter
    {
        public KeywordType CompareOperator { get; set; }
        public string Value { get; set; }
    }
}