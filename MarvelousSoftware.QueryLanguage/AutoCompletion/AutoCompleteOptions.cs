using System.Linq;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion
{
    public sealed class AutoCompleteOptions<T>
    {
        /// <summary>
        /// If true then case will be ignored while limiting completion results.
        /// </summary>
        public bool IgnoreCase = true;

        /// <summary>
        /// Strategy used on keywords completion.
        /// </summary>
        public CompletionStrategy KeywordCompletionStrategy = CompletionStrategy.StartsWith;

        /// <summary>
        /// Strategy used on columns completion.
        /// </summary>
        public CompletionStrategy ColumnCompletionStrategy = CompletionStrategy.StartsWith;

        /// <summary>
        /// Strategy used on functions completion.
        /// </summary>
        public CompletionStrategy FunctionCompletionStrategy = CompletionStrategy.StartsWith;

        /// <summary>
        /// Strategy used on values completion.
        /// </summary>
        public CompletionStrategy ValueCompletionStrategy = CompletionStrategy.StartsWith;

        /// <summary>
        /// Maximum number of allowed completions to be displayed on a single page.
        /// </summary>
        public int PageSize = 15;

        /// <summary>
        /// Data source for value completions.
        /// </summary>
        public IQueryable<T> ValueDataSource { get; set; }

        internal bool HasValueDataSource => ValueDataSource != null;
    }
}