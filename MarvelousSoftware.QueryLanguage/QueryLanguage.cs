using System.Linq;
using MarvelousSoftware.Core;
using MarvelousSoftware.QueryLanguage.AutoCompletion;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Execution;
using MarvelousSoftware.QueryLanguage.Lexer;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Parsing;

namespace MarvelousSoftware.QueryLanguage
{
    /// <summary>
    /// Main service for using query builder language. Uses only publicly available types and therefore 
    /// each functionality can be archived without this class. The purpouse is to make easier
    /// usage of the library by supplying common use cases.
    /// </summary>
    public class QueryLanguage<T>
    {
        /// <summary>
        /// Language configuration.
        /// </summary>
        public readonly LanguageConfig<T> Config;

        private readonly Lexer<T> _lexer;

        /// <summary>
        /// Creates instance of <see cref="QueryLanguage{T}"/> using <see cref="config"/>.
        /// </summary>
        public QueryLanguage(LanguageConfig<T> config)
        {
            Config = config;
            _lexer = new Lexer<T>(Config);
        }

        /// <summary>
        /// Filters given <see cref="input"/> using given <see cref="query"/>.
        /// </summary>
        /// <param name="input">Queryable object which will be filtered.</param>
        /// <param name="query">Query used for filtration.</param>
        /// <returns>Filtered input.</returns>
        public FilterResult<T> Filter(IQueryable<T> input, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new FilterResult<T>()
                {
                    Elements = input
                };
            }

            var tokenizationResult = _lexer.Run(query);
            if (tokenizationResult.Errors.Any())
            {
                return new FilterResult<T>()
                {
                    Errors = tokenizationResult.Errors
                };
            }

            var filter = new ExpressionBasedFilter<T>();
            var parsingResult = new Parser()
                .AddVisitor(filter)
                .Parse(tokenizationResult.Tokens.ToArray());

            if (parsingResult.Errors.Any())
            {
                return new FilterResult<T>()
                {
                    Errors = parsingResult.Errors
                };
            }

            return new FilterResult<T>()
            {
                Elements = filter.GetFilteredElements(input)
            };
        }

        /// <summary>
        /// Gets auto completions using provided query.
        /// </summary>
        /// <param name="query">Query used for auto completion.</param>
        /// <param name="caretPosition">Caret position on the query.</param>
        /// <param name="skip">Number of elements to skip. Used for pagination. Page size is defined in the language config.</param>
        /// <param name="config">Language configuration.</param>
        public AutoCompletionResult AutoComplete(string query, int caretPosition, int skip, LanguageConfig<T> config)
        {
            var autoCompleter = new AutoCompleter<T>(config);
            return autoCompleter.Run(query, caretPosition, skip);
        }

        /// <summary>
        /// Gets auto completions using provided query from web request.
        /// </summary>
        public AutoCompletionResult AutoComplete()
        {
            var autoCompleter = new AutoCompleter<T>(Config);
            var request = RequestUtils.GetRequestWrapper();
            if (!request.Has("query") || !request.Has("caretPosition") || !request.Has("skip"))
            {
                throw new QueryLanguageException("One of the paremeters is missing in the web request.");
            }

            int caretPosition;
            if (!int.TryParse(request["caretPosition"], out caretPosition))
            {
                throw new QueryLanguageException("'caretPosition' should be an integer.");
            }

            int skip;
            if (!int.TryParse(request["skip"], out skip))
            {
                throw new QueryLanguageException("'skip' should be an integer.");
            }

            return autoCompleter.Run(request["query"], caretPosition, skip);
        }
    }
}