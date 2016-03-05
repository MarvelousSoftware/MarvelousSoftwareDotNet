using System;
using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion.Completers
{
    public abstract class EnumerableCompleterBase<T> : CompleterBase<T>
    {
        protected abstract CompletionStrategy CompletionStrategy { get; }

        protected EnumerableCompleterBase(LanguageConfig<T> config) : base(config)
        {
        }

        public abstract IEnumerable<string> GetCompletionsImpl(CompletionInfo completionInfo);

        public override CompletionResult GetCompletions(CompletionInfo completionInfo)
        {
            return LimitCompletionResults(GetCompletionsImpl(completionInfo), completionInfo.ComparePart, completionInfo.BatchSize);
        }

        protected CompletionResult LimitCompletionResults(IEnumerable<string> inputResults, string comparePart, int take)
        {
            if (string.IsNullOrEmpty(comparePart))
            {
                return new CompletionResult(inputResults.Select(x => new SpecificCompleterResult(x)), take);
            }

            IEnumerable<string> results;

            switch (CompletionStrategy)
            {
                case CompletionStrategy.StartsWith:
                    results = LimitUsingStartsWithStrategy(inputResults, comparePart);
                    break;
                case CompletionStrategy.Contains:
                    results = LimitUsingContainsStrategy(inputResults, comparePart);
                    break;
                default:
                    throw new QueryLanguageException($"Completion strategy '{CompletionStrategy}' is not supported.");
            }

            return new CompletionResult(results.Select(x => new SpecificCompleterResult(x)), take);
        }

        private IEnumerable<string> LimitUsingStartsWithStrategy(IEnumerable<string> inputResults, string comparePart)
        {
            return inputResults.Where(x => x.StartsWith(comparePart, Config.AutoCompleteOptions.IgnoreCase, null));
        }

        private IEnumerable<string> LimitUsingContainsStrategy(IEnumerable<string> inputResults, string comparePart)
        {
            var comparison = Config.AutoCompleteOptions.IgnoreCase
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture;

            return inputResults.Where(x => x.IndexOf(comparePart, comparison) >= 0);
        }
    }
}