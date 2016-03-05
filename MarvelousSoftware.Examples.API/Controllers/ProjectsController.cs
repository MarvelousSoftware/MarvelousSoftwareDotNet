using System.Web.Http;
using System.Web.Http.Cors;
using MarvelousSoftware.Examples.Samples;
using MarvelousSoftware.Grid.DataSource;

namespace MarvelousSoftware.Examples.API.Controllers
{
    public class ProjectsController : ApiController
    {
        [HttpGet]
        [EnableCors("*", "*", "*")]
        public DataSourceResult<Project> Get()
        {
            return Project.GetAll().ToDataSourceResult();
        }
    }
}