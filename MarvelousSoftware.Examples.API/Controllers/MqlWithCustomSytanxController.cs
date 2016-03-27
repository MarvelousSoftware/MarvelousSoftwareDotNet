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
    public class MqlWithCustomSytanxController : ApiController
    {
        private static readonly IQueryable<Person> _people = People.Get(1000).AsQueryable();

        private static readonly LanguageConfig<Person> _languageConfig = new LanguageConfig<Person>()
            .AddColumn(x => x.FirstName) // columns mapping: public name -> internal property
            .AddColumn(x => x.LastName)
            .AddColumn(x => x.Age)
            .AddColumn(x => x.DateOfBirth)
            .AddColumn(x => x.PlaceOfResidence)
            .AddColumn(x => x.UserName)
            .AddColumn(x => x.Rating)
            .Syntax(x =>
            {
                x.KeywordCaseSensitive = true;
                x.ColumnNameCaseSensitive = true;
                x.NullConstant = "NONE";
                x.NullConstantCaseSensitive = true;
                x.NumberDecimalSeparator = ',';
                x.ParenOpen = '[';
                x.ParenClose = ']';
                x.StringLiteralIdentifier = '\'';
                x.DateTimeFormats = new[] {"M/d/yyyy H:m", "M/d/yyyy"};
                x.ReplaceKeyword(new Keyword("equals", KeywordType.Equal));
                x.ReplaceKeyword(new Keyword("not equals", KeywordType.NotEqual));
            })
            .AutoCompletion(c => c.ValueDataSource = _people); // source of auto completions 

        private static readonly QueryLanguage<Person> _queryLanguage = new QueryLanguage<Person>(_languageConfig);

        [HttpGet]
        [Route("~/api/mql/syntax/people")]
        [EnableCors("*", "*", "*")]
        public QueryLanguageFilterResult<Person> Get()
        {
            var result = _people.FilterWithQueryLanguage(_languageConfig);

            // for sake of simplicity pagination is not implemented, therefore only 10 elements are returned
            result.Data = result.Data.Take(10);

            return result;
        }

        [HttpGet]
        [Route("~/api/mql/syntax/people/auto-completion")]
        [EnableCors("*", "*", "*")]
        public AutoCompletionResult QueryAutoCompletion()
        {
            // gets auto completions using provided query from web request
            return _queryLanguage.AutoComplete();
        }
    }
}