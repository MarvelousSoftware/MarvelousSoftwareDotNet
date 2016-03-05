using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion.Completers
{
    public abstract class CompleterBase<T> : CompleterBase
    {
        public abstract short Order { get; }
        public abstract CompletionGroup CompletionGroup { get; }

        protected readonly LanguageConfig<T> Config;

        protected CompleterBase(LanguageConfig<T> config)
        {
            Config = config;
        }

        public abstract CompletionResult GetCompletions(CompletionInfo completionInfo);

        public abstract bool ShouldProcess(TokenType tokenType);
    }

    public abstract class CompleterBase
    {
    }
}