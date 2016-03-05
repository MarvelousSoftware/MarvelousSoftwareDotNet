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
    public class MqlWithPredefinedFunctionsController : ApiController
    {
        private static readonly IQueryable<Person> _people = People.Get(1000).AsQueryable();

        private static readonly LanguageConfig<Person> _languageConfig = new LanguageConfig<Person>()
            .AddColumn(x => x.FirstName)
            .AddColumn(x => x.LastName)
            .AddColumn(x => x.Age)
            .AddColumn(x => x.DateOfBirth)
            .AddColumn(x => x.PlaceOfResidence)
            .AddColumn(x => x.DueDate)
            .WithPredefinedFunctions() // adds predefined functions such as "today" or "now"
            .AutoCompletion(c => c.ValueDataSource = _people); // source of auto completions 

        private static readonly QueryLanguage<Person> _queryLanguage = new QueryLanguage<Person>(_languageConfig);

        [HttpGet]
        [Route("~/api/mql/predefined-functions/people")]
        [EnableCors("*", "*", "*")]
        public QueryLanguageFilterResult<Person> Get()
        {
            var result = _people.FilterWithQueryLanguage(_languageConfig);

            // for sake of simplicity pagination is not implemented, therefore only 10 elements are returned
            result.Data = result.Data.Take(10);

            return result;
        }

        [HttpGet]
        [Route("~/api/mql/predefined-functions/people/auto-completion")]
        [EnableCors("*", "*", "*")]
        public AutoCompletionResult QueryAutoCompletion()
        {
            // gets auto completions using provided query from web request
            return _queryLanguage.AutoComplete();
        }
    }
}