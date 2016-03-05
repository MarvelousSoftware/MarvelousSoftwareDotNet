using System.Collections.Generic;
using System.Linq;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion.Completers
{
    public class CompletionResult
    {
        public IEnumerable<SpecificCompleterResult> Completions { get; set; }
        public bool IsNextPageAvailable { get; set; }

        public CompletionResult(IEnumerable<SpecificCompleterResult> input, int take)
        {
            var result = input.OrderBy(x => x.Value).Take(take + 1).ToArray();
            if (result.Length == take + 1)
            {
                IsNextPageAvailable = true;
                Completions = result.Take(take);
            }
            else
            {
                Completions = result;
            }
        }
    }
}