using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion.Completers
{
    public abstract class QuerableCompleterBase<T> : CompleterBase<T>
    {
        protected QuerableCompleterBase(LanguageConfig<T> config) : base(config)
        {
        }

        protected abstract CompletionStrategy CompletionStrategy { get; }

        public abstract IQueryable<SpecificCompleterResult> GetCompletionsImpl(CompletionInfo completionInfo);

        public override CompletionResult GetCompletions(CompletionInfo completionInfo)
        {
            var valuesSource = GetCompletionsImpl(completionInfo);

            if (valuesSource == null)
            {
                return null;
            }

            IQueryable<SpecificCompleterResult> result;

            if (string.IsNullOrEmpty(completionInfo.ComparePart))
            {
                result = valuesSource;
            }
            else
            {
                result = Limit(valuesSource, completionInfo.ComparePart);
            }

            return new CompletionResult(result, completionInfo.BatchSize);
        }

        private IQueryable<SpecificCompleterResult> Limit(IQueryable<SpecificCompleterResult> source, string comparePart)
        {
            var param = Expression.Parameter(typeof(SpecificCompleterResult));

            Expression final;

            switch (CompletionStrategy)
            {
                case CompletionStrategy.StartsWith:
                    final = GetStartsWithComparisonExpression(comparePart, param);
                    break;
                case CompletionStrategy.Contains:
                    final = GetContainsComparisonExpression(comparePart, param);
                    break;
                default:
                    throw new NotSupportedException($"Completion strategy '{CompletionStrategy}' is not supported.");
            }

            var lambda = Expression.Lambda<Func<SpecificCompleterResult, bool>>(final, param);

            return source.Where(lambda);
        }

        private Expression GetStartsWithComparisonExpression(string comparePart, ParameterExpression param)
        {
            var startsWithMethod = typeof(string).GetMethod("StartsWith",
                new[] { typeof(string), typeof(bool), typeof(CultureInfo) });

            return Expression.Call(Expression.PropertyOrField(param, "Value"), startsWithMethod,
                Expression.Constant(comparePart),
                Expression.Constant(Config.AutoCompleteOptions.IgnoreCase),
                Expression.Constant(null, typeof(CultureInfo)));
        }

        private Expression GetContainsComparisonExpression(string comparePart, ParameterExpression param)
        {
            var comparison = Config.AutoCompleteOptions.IgnoreCase
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture;

            var indexOfMethod = typeof(string).GetMethod("IndexOf",
                new[] { typeof(string), typeof(StringComparison) });
            var call = Expression.Call(Expression.PropertyOrField(param, "Value"), indexOfMethod,
                Expression.Constant(comparePart),
                Expression.Constant(comparison));

            return Expression.GreaterThanOrEqual(call, Expression.Constant(0));
        }
    }
}