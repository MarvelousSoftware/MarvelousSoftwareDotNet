using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion.Completers.Impl
{
    public sealed class FunctionCompleter<T> : EnumerableCompleterBase<T>
    {
        public override short Order => 5;
        public override CompletionGroup CompletionGroup => CompletionGroup.Keywords;

        protected override CompletionStrategy CompletionStrategy => Config.AutoCompleteOptions.FunctionCompletionStrategy;

        public FunctionCompleter(LanguageConfig<T> config) : base(config)
        {
        }

        public override IEnumerable<string> GetCompletionsImpl(CompletionInfo completionInfo)
        {
            if (completionInfo.ColumnInfo == null)
            {
                return Enumerable.Empty<string>();
            }



            return Config.FunctionDefinitions.Definitions
                .Where(x => x.ColumnType == completionInfo.ColumnInfo.ColumnType)
                .Select(x => x.Name + "()");
        }

        public override bool ShouldProcess(TokenType tokenType) => tokenType == TokenType.Function;
    }
}