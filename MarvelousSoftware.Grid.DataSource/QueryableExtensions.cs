using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MarvelousSoftware.Core;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using MarvelousSoftware.Core.Utils;
using MarvelousSoftware.QueryLanguage.SimplifiedFiltering;

namespace MarvelousSoftware.Grid.DataSource
{
    // TODO: write unit tests for each method

    public static class QueryableExtensions
    {
        public static DataSourceResult<T> ToDataSourceResult<T>(this IQueryable<T> input, Action<ToDataSourceResultConfig<T>> configAction = null)
        {
            var dataSourceResult = new DataSourceResult<T>();

            var config = new ToDataSourceResultConfig<T>(input, dataSourceResult);
            configAction?.Invoke(config);

            if (dataSourceResult.Errors.Any())
            {
                dataSourceResult.Total = input.Count();
                return dataSourceResult;
            }

            dataSourceResult.Data = config.Data;
            dataSourceResult.Total = dataSourceResult.Data.Count();

            var request = RequestUtils.GetRequestWrapper();

            if (!request.Has("marvelousParams"))
            {
                return dataSourceResult;
            }

            var decodedParams = Encoding.UTF8.GetString(Convert.FromBase64String(request["marvelousParams"]));
            var requestParams = new JavaScriptSerializer().Deserialize<DataSourceParams>(decodedParams);

            return HandleDataSourceParams(requestParams, dataSourceResult.Data, dataSourceResult);
        }

        private static DataSourceResult<T> HandleDataSourceParams<T>(DataSourceParams requestParams, IQueryable<T> data, DataSourceResult<T> dataSourceResult)
        {
            var columns = new List<Column<T>>();
            if (requestParams.Fields != null)
            {
                columns.AddRange(requestParams.Fields.Select(field => new Column<T>(field)));
            }

            if (requestParams.Filtering != null && requestParams.Filtering.Any())
            {
                data = FilterData(requestParams, data);
                dataSourceResult.Total = data.Count();
            }

            if (requestParams.SortBy != null && requestParams.SortBy.Any())
            {
                var columnsToSort = new List<Column<T>>();
                foreach (var sort in requestParams.SortBy)
                {
                    var column = columns.FirstOrDefault(x => x.Member == sort.Member);
                    if (column == null)
                    {
                        column = new Column<T>(sort.Member);
                        columns.Add(column);
                    }

                    column.SetSortDirection(sort.Direction);
                    columnsToSort.Add(column);
                }
                data = SortData(columnsToSort, data);
            }

            if (requestParams.Page.HasValue && requestParams.PageSize.HasValue)
            {
                data = data
                    .Skip((requestParams.Page.Value - 1)*requestParams.PageSize.Value)
                    .Take(requestParams.PageSize.Value);
            }

            if (requestParams.GetOnlyVisibleColumns.HasValue)
            {
                dataSourceResult.Data = GetOnlyVisible(columns, data);
            }
            else
            {
                dataSourceResult.Data = data;
            }
            return dataSourceResult;
        }

        private static IQueryable<T> FilterData<T>(DataSourceParams requestParams, IQueryable<T> data)
        {
            var filterer = new SimplifiedFilterer();
            var columnsFilters = new Dictionary<string, IEnumerable<SimpleColumnFilter>>();

            foreach (var filterInfo in requestParams.Filtering)
            {
                columnsFilters.Add(filterInfo.Key, filterInfo.Value.Select(x => new SimpleColumnFilter()
                {
                    CompareOperator = x.CompareOperator,
                    Value = x.Value
                }));
            }

            var model = new SimplifiedFilteringModel()
            {
                ColumnsFilters = columnsFilters,
                DateFormats = new[] { "d/M/yyyy H:m", "d/M/yyyy" },
                FalseValues = new[] { "false", "no" },
                TrueValues = new[] { "true", "yes" },
                NumberDecimalSeparator = ","
            };
            return filterer.Filter(model, data);
        }

        private static IOrderedQueryable<T> SortData<T>(IEnumerable<Column<T>> columnsToSort, IQueryable<T> data)
        {
            IOrderedQueryable<T> result = null;

            IOrderedQueryable<T> ordered = null;
            foreach (var column in columnsToSort)
            {
                var paramExpression = Expression.Parameter(typeof (T), "x");
                var memberExpression = ExpressionUtils.GetNestedPropertyOrField(paramExpression, column.Member);
                var boxingExpression = Expression.Convert(memberExpression, typeof(object));
                var finalExpression = Expression.Lambda<Func<T, object>>(boxingExpression, paramExpression);

                switch (column.SortDirection)
                {
                    case SortDirection.Ascending:
                        result = ordered?.ThenBy(finalExpression) ?? data.OrderBy(finalExpression);
                        ordered = result;
                        break;
                    case SortDirection.Descending:
                        result = ordered?.ThenByDescending(finalExpression) ?? data.OrderByDescending(finalExpression);
                        ordered = result;
                        break;
                }
            }

            return result;
        }

        private static IQueryable<T> GetOnlyVisible<T>(IEnumerable<Column<T>> allColumns, IQueryable<T> data)
        {
            var columns = allColumns.ToArray();
            var type = typeof(T);

            var propertyMembers = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var fieldMembers = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            if (propertyMembers.Length + fieldMembers.Length == columns.Length)
            {
                // all fields are visible, therefore there is no need for any kind of transformation
                return data;
            }

            var constructor = type.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
            {
                throw new DataSourceException($"In order to get only visible column {type.FullName} has to provide parameterless constructor.");
            }

            var param = Expression.Parameter(type);
            var newObj = Expression.New(constructor);
            var bindings = new MemberBinding[columns.Length];
            for (var i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                bindings[i] = Expression.Bind(column.MemberInfo, Expression.PropertyOrField(param, column.Member));
            }
            var memberInit = Expression.MemberInit(newObj, bindings);
            var final = Expression.Lambda<Func<T, T>>(memberInit, param);

            return data.Select(final);
        }

        private class Column<T>
        {
            public string Member { get; private set; }
            public MemberInfo MemberInfo { get; private set; }
            public SortDirection SortDirection { get; private set; }

            public Column(string member)
            {
                Member = member;

                MemberInfo = typeof(T).GetMember(Member, MemberTypes.Field | MemberTypes.Property,
                    BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();
                var exists = MemberInfo != null;
                if (!exists)
                {
                    throw new DataSourceException($"Property or field '{Member}' doesn't exist in the '${typeof(T).FullName}' type.");
                }
            }

            public void SetSortDirection(string direction)
            {
                switch (direction)
                {
                    case "asc":
                        SortDirection = SortDirection.Ascending;
                        break;
                    case "desc":
                        SortDirection = SortDirection.Descending;
                        break;
                    default:
                        var msg = $"'{direction}' direction is not supported.";
                        throw new DataSourceException(msg);
                }
            }
        }

        private enum SortDirection
        {
            None,
            Ascending,
            Descending
        }
    }
}

