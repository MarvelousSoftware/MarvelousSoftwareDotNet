using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using MarvelousSoftware.Examples.Samples;
using MarvelousSoftware.Grid.DataSource;

namespace MarvelousSoftware.Examples.API.Controllers
{
    public class PeopleController : ApiController
    {
        [HttpGet]
        [HttpPost]
        [EnableCors("*", "*", "*")]
        public DataSourceResult<Person> Get()
        {
            Thread.Sleep(400);
            return People.Get(1000).ToDataSourceResult();
        }

        [HttpGet]
        [EnableCors("*", "*", "*")]
        [Route("~/api/project/{ProjectId}/people")]
        public DataSourceResult<Person> GetByProject(int projectId)
        {
            Thread.Sleep(400);
            return People.Get(1000).Where(x => x.ProjectId == projectId).ToDataSourceResult();
        }
    }
}