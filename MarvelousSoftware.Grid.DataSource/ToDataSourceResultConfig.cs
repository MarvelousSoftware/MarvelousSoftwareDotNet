using System.Linq;

namespace MarvelousSoftware.Grid.DataSource
{
    public class ToDataSourceResultConfig<T>
    {
        internal IQueryable<T> Data { get; set; }
        internal DataSourceResult<T> DataSourceResult { get; set; }

        public ToDataSourceResultConfig(IQueryable<T> data, DataSourceResult<T> dataSourceResult)
        {
            Data = data;
            DataSourceResult = dataSourceResult;
        }
    }
}