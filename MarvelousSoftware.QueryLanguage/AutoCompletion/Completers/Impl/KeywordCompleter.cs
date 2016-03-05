using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion.Completers.Impl
{
    public sealed class KeywordCompleter<T> : EnumerableCompleterBase<T>
    {
        public override short Order => 0;
        public override CompletionGroup CompletionGroup => CompletionGroup.Keywords;

        public KeywordCompleter(LanguageConfig<T> config) : base(config)
        {
        }

        protected override CompletionStrategy CompletionStrategy => Config.AutoCompleteOptions.KeywordCompletionStrategy;

        public override IEnumerable<string> GetCompletionsImpl(CompletionInfo completionInfo)
        {
            switch (completionInfo.TokenType)
            {
                case TokenType.ParenOpen:
                    return new List<string>
                    {
                        Config.Syntax.Config.ParenOpen.ToString()
                    };
                case TokenType.ParenClose:
                    return new List<string>
                    {
                        Config.Syntax.Config.ParenClose.ToString()
                    };
                case TokenType.CompareOperator:
                    if (completionInfo.ColumnInfo == null)
                    {
                        return new string[0];
                    }
                    return Config.Syntax.Config.Keywords
                        .Where(x => x.TokenType == completionInfo.TokenType)
                        .Where(completionInfo.ColumnInfo.IsCompareOperatorAllowed)
                        .Select(x => x.Syntax);
                case TokenType.Statement:
                    if (completionInfo.ColumnInfo == null)
                    {
                        return new string[0];
                    }
                    return Config.Syntax.Config.Keywords
                        .Where(x => x.TokenType == completionInfo.TokenType)
                        .Where(completionInfo.ColumnInfo.IsStatementAllowed)
                        .Select(x => x.Syntax);
                default:
                    return Config.Syntax.Config.Keywords
                        .Where(x => x.TokenType == completionInfo.TokenType)
                        .Select(x => x.Syntax);
            }
        }

        public override bool ShouldProcess(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.CompareOperator:
                case TokenType.LogicalOperator:
                case TokenType.ParenClose:
                case TokenType.ParenOpen:
                case TokenType.Statement:
                    return true;
                default:
                    return false;
            }
        }
    }
}