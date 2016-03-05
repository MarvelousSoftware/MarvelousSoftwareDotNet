using System.Linq;
using MarvelousSoftware.Grid.DataSource;
using MarvelousSoftware.QueryLanguage;
using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.Grid.QueryLanguage
{
    public static class ToDataSourceResultConfigExtensions
    {
        public static ToDataSourceResultConfig<T> FilterWithQueryLanguage<T>(this ToDataSourceResultConfig<T> dataSourceConfig, LanguageConfig<T> languageConfig)
        {
            var result = dataSourceConfig.Data.FilterWithQueryLanguage(languageConfig);

            if (result.Errors.Any())
            {
                dataSourceConfig.DataSourceResult["queryLanguage"] = new
                {
                    errors = result.Errors
                };
            }
            else
            {
                dataSourceConfig.DataSourceResult["queryLanguage"] = new { };
            }

            dataSourceConfig.Data = result.Data;
            return dataSourceConfig;
        }
    }
}