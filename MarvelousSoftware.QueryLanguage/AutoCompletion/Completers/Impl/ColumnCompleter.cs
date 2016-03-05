using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion.Completers.Impl
{
    public sealed class ColumnCompleter<T> : EnumerableCompleterBase<T>
    {
        public override short Order => 10;
        public override CompletionGroup CompletionGroup => CompletionGroup.Columns;

        public ColumnCompleter(LanguageConfig<T> config) : base(config)
        {
        }

        protected override CompletionStrategy CompletionStrategy => Config.AutoCompleteOptions.ColumnCompletionStrategy;

        public override IEnumerable<string> GetCompletionsImpl(CompletionInfo completionInfo)
        {
            return Config.Columns.Select(x => x.ColumnName);
        }

        public override bool ShouldProcess(TokenType tokenType) => tokenType == TokenType.Column;
    }
}