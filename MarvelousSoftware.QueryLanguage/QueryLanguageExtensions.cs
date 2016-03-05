using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;
using MarvelousSoftware.Core;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage
{
    public static class QueryLanguageExtensions
    {
        public static QueryLanguageFilterResult<T> FilterWithQueryLanguage<T>(this IQueryable<T> dataSource, LanguageConfig<T> config)
        {
            var request = RequestUtils.GetRequestWrapper();

            var query = GetQuery(request);
            if (query == null)
            {
                return new QueryLanguageFilterResult<T>()
                {
                    Data = dataSource
                };
            }

            var language = new QueryLanguage<T>(config);
            var result = language.Filter(dataSource, query);

            return new QueryLanguageFilterResult<T>()
            {
                Data = result.Elements,
                Errors = result.Errors.GetSingleMessageIfPossible()
            };
        }

        private static string GetQuery(IRequestWrapper request)
        {
            if (!request.Has("marvelousParams") && !request.Has("query"))
            {
                return null;
            }

            if (request.Has("query"))
            {
                return request["query"];
            }

            var decodedParams = Encoding.UTF8.GetString(Convert.FromBase64String(request["marvelousParams"]));
            var requestParams = new JavaScriptSerializer().Deserialize<QueryLanguageParams>(decodedParams);

            return requestParams.Query;
        }
    }

    public class QueryLanguageParams
    {
        [DataMember(Name = "query")]
        public string Query { get; set; }
    }
}
