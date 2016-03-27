using System;
using System.Globalization;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Execution;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract;

namespace MarvelousSoftware.QueryLanguage.SimplifiedFiltering
{
    public class SimplifiedFilterer
    {
        /// <summary>
        /// Filters data using provided model using always AND logical operator.
        /// </summary>
        public IQueryable<T> Filter<T>(SimplifiedFilteringModel filteringModel, IQueryable<T> data)
        {
            var languageConfig = new LanguageConfig<T>();
            var andKeyword = languageConfig.SyntaxConfig.LogicalOperators.First(x => x.KeywordType == KeywordType.And); // TODO: keyword factory?
            ExpressionBase root = null;

            if (filteringModel.ColumnsFilters == null || filteringModel.ColumnsFilters.Count == 0)
            {
                throw new ArgumentException("ColumnsFilters missing.", nameof(filteringModel));
            }

            foreach (var columnFilters in filteringModel.ColumnsFilters)
            {
                var columnName = columnFilters.Key;
                languageConfig.AddColumn(columnName, columnName);
                var column = languageConfig.Columns.First(x => x.MemberName == columnName);
                var filters = columnFilters.Value;

                foreach (var filter in filters)
                {
                    ExpressionBase expression;

                    switch (filter.CompareOperator)
                    {
                        case KeywordType.Empty:
                        case KeywordType.NotEmpty:
                        case KeywordType.IsTrue:
                        case KeywordType.IsFalse:
                            var statementKeyword = languageConfig.SyntaxConfig.Statements.First(x => x.KeywordType == filter.CompareOperator);
                            expression = new StatementExpression(new ColumnToken(column), new StatementToken(statementKeyword));
                            break;
                        default:
                            object value;
                            if (!TryParseValue(filter.Value, filteringModel, column, out value))
                            {
                                return data;
                            }

                            var compareOperator = languageConfig.SyntaxConfig.CompareOperators.First(x => x.KeywordType == filter.CompareOperator);
                            expression = new CompareExpression(new ColumnToken(column), new CompareOperatorToken(compareOperator), new LiteralToken(value));
                            break;
                    }

                    if (root == null)
                    {
                        root = expression;
                        continue;
                    }
                    root = new BinaryExpression(root, new LogicalOperatorToken(andKeyword), expression);
                }
            }

            if (root == null)
            {
                throw new InvalidOperationException("Something went wrong.");
            }

            var expressionBasedFilter = new ExpressionBasedFilter<T>();
            root.Visit(expressionBasedFilter);
            return expressionBasedFilter.GetFilteredElements(data);
        }

        private static bool TryParseValue(string value, SimplifiedFilteringModel filteringModel, ColumnInfo column, out object output)
        {
            switch (column.ColumnType)
            {
                case ColumnType.String:
                    output = value;
                    return true;
                case ColumnType.Integer:
                    value = value.Replace(" ", "");
                    return LiteralParserHelper.TryParseInteger(value, column.SystemType, out output);
                case ColumnType.Float:
                    value = value.Replace(" ", "");
                    var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                    culture.NumberFormat.NumberDecimalSeparator = filteringModel.NumberDecimalSeparator;
                    return LiteralParserHelper.TryParseFloat(value, column.SystemType, culture, out output);
                case ColumnType.Boolean:
                    if (filteringModel.TrueValues.Contains(value.ToLower()))
                    {
                        output = true;
                        return true;
                    }
                    if (filteringModel.FalseValues.Contains(value.ToLower()))
                    {
                        output = false;
                        return true;
                    }
                    output = null;
                    return false;
                case ColumnType.DateTime:
                    DateTime date;
                    var parsed = LiteralParserHelper.TryParseDate(value, filteringModel.DateFormats,
                        CultureInfo.CurrentCulture, out date);
                    output = date;
                    return parsed;
                default:
                    throw new InvalidOperationException($"Type of '${column.MemberName}' is not supported.");
            }
        }
    }
}