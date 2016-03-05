using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using MarvelousSoftware.Examples.Samples;
using MarvelousSoftware.QueryLanguage;
using MarvelousSoftware.QueryLanguage.AutoCompletion;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.Examples.API.Controllers
{
    public class CitiesWithQueryLanguageController : ApiController
    {
        private static readonly IQueryable<City> _cities = Cities.Get(DateTime.Now).AsQueryable();

        private static readonly LanguageConfig<City> _languageConfig = new LanguageConfig<City>()
            .AddColumn("City", x => x.Name) // columns mapping: public name -> internal property
            .AddColumn(x => x.Country)
            .AddColumn(x => x.Population)
            .AddColumn(x => x.TotalArea)
            .AddColumn("Density", x => x.PopulationDensity)
            .AddColumn(x => x.Created)
            .AddColumn(x => x.DeathPenalty)
            .AutoCompletion(c => c.ValueDataSource = _cities); // source of auto completions 

        private static readonly QueryLanguage<City> _queryLanguage = new QueryLanguage<City>(_languageConfig);

        [HttpGet]
        [Route("~/api/query-language/cities")]
        [EnableCors("*", "*", "*")]
        public QueryLanguageFilterResult<City> Get()
        {
            // returns cities ordered by creation date and filtered with provided query (if any)
            // uses given language configuration
            // FilterWithQueryLanguage is just an extension method from MarvelousSoftware.QueryLanguage package
            var result = _cities.OrderByDescending(x => x.Created).FilterWithQueryLanguage(_languageConfig);

            // for sake of simplicity pagination is not implemented, therefore only 10 elements are returned
            result.Data = result.Data.Take(10);

            return result;
        }

        [HttpGet]
        [Route("~/api/query-language/cities/auto-completion")]
        [EnableCors("*", "*", "*")]
        public AutoCompletionResult QueryAutoCompletion()
        {
            // gets auto completions using provided query from web request
            return _queryLanguage.AutoComplete();
        } 
    }
}