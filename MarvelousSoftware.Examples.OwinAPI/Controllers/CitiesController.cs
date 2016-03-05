using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using MarvelousSoftware.Examples.Samples;
using MarvelousSoftware.Grid.DataSource;

namespace MarvelousSoftware.Examples.OwinAPI.Controllers
{
    public class CitiesController : ApiController
    {
        [HttpGet]
        [HttpPost]
        [EnableCors("*", "*", "*")]
        public DataSourceResult<City> Get()
        {
            Thread.Sleep(400);
            return Cities.Get().AsQueryable().ToDataSourceResult();
        }
    }
}