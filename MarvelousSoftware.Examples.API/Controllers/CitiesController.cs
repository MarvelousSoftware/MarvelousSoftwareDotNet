using System;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using MarvelousSoftware.Examples.Samples;
using MarvelousSoftware.Grid.DataSource;
using MarvelousSoftware.Grid.QueryLanguage;
using MarvelousSoftware.QueryLanguage;
using MarvelousSoftware.QueryLanguage.AutoCompletion;
using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.Examples.API.Controllers
{
    public class CitiesController : ApiController
    {
        private static readonly IQueryable<City> _cities = Cities.Get(DateTime.Now).AsQueryable();

        private static readonly LanguageConfig<City> _languageConfig = new LanguageConfig<City>()
            .AddColumn("City", x => x.Name)
            .AddColumn(x => x.Country)
            .AddColumn(x => x.Population)
            .AddColumn(x => x.TotalArea)
            .AddColumn("Density", x => x.PopulationDensity)
            .AddColumn(x => x.Created)
            .AddColumn(x => x.DeathPenalty)
            .AutoCompletion(c => c.ValueDataSource = _cities);

        private static readonly QueryLanguage<City> _queryLanguage = new QueryLanguage<City>(_languageConfig);

        [HttpGet]
        [EnableCors("*", "*", "*")]
        public DataSourceResult<City> Get()
        {
            Thread.Sleep(400);
            
            return _cities.OrderByDescending(x => x.Created).ToDataSourceResult(config =>
            {
                config.FilterWithQueryLanguage(_languageConfig);
            });
        }

        [HttpGet]
        [Route("~/api/cities/auto-completion")]
        [EnableCors("*", "*", "*")]
        public AutoCompletionResult QueryAutoCompletion() => _queryLanguage.AutoComplete();
    }
}