using System.Runtime.Serialization;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion
{
    public class AutoCompletionRow
    {
        public string Text { get; set; }
        public CompletionGroup Group { get; set; }

        [IgnoreDataMember]
        public string Value { get; set; }
    }
}