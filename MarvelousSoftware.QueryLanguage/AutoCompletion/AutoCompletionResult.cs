using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion
{
    public class AutoCompletionResult
    {
        public int StartPosition { get; internal set; }
        public int Length { get; internal set; }
        public IEnumerable<AutoCompletionRow> Completions { get; internal set; }
        public bool IsNextPageAvailable { get; internal set; }
        public ErrorList Errors { get; internal set; }

        public bool HasErrors => Errors != null && Errors.Any();

        public IEnumerable<string> GetTexts(CompletionGroup @group)
        {
            return Completions?.Where(x => x.Group == group).Select(x => x.Text);
        } 
    }
}