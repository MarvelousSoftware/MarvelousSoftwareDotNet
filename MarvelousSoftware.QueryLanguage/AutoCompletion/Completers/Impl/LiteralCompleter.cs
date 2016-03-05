using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MarvelousSoftware.Common.Utils;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion.Completers.Impl
{
    public class LiteralCompleter<T> : QuerableCompleterBase<T>
    {
        public LiteralCompleter(LanguageConfig<T> config) : base(config)
        {
        }

        public override short Order => 20;
        public override CompletionGroup CompletionGroup => CompletionGroup.Values;
        protected override CompletionStrategy CompletionStrategy => Config.AutoCompleteOptions.ValueCompletionStrategy;

        public override IQueryable<SpecificCompleterResult> GetCompletionsImpl(CompletionInfo completionInfo)
        {
            var source = Config.AutoCompleteOptions.ValueDataSource;

            if (completionInfo.ColumnInfo?.ColumnType != ColumnType.String || source == null)
            {
                return new List<SpecificCompleterResult>().AsQueryable();
            }

            var param = Expression.Parameter(typeof(T));

            var memberAccessExpression = ExpressionUtils.GetNestedPropertyOrField(param, completionInfo.ColumnInfo.MemberName);
            var memberAccessLambda = Expression.Lambda<Func<T, string>>(memberAccessExpression, param);

            var nullChecks = ExpressionUtils.CheckForNulls(param, completionInfo.ColumnInfo.MemberName);
            var nullChecksExpression = Expression.Lambda<Func<T, bool>>(nullChecks, param);

            var valuesSource = source
                .Where(nullChecksExpression)
                .Select(memberAccessLambda)
                .Where(x => x != string.Empty)
                .Distinct()
                .Select(x => new SpecificCompleterResult(x, true));

            return valuesSource;
        }

        public override bool ShouldProcess(TokenType tokenType)
        {
            return tokenType == TokenType.Literal;
        }
    }
}