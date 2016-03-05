using System.Web;
using MarvelousSoftware.Grid.DataSource;
using System.Linq;

namespace MarvelousSoftware.Core.Host.SystemWeb
{
    public class SystemWebRequestWrapper : IRequestWrapper
    {
        public string this[string name] => HttpContext.Current.Request[name];

        public bool Has(string name) => HttpContext.Current.Request.Params.AllKeys.Contains(name);
    }
}
