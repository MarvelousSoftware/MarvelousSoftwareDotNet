using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MarvelousSoftware.Core.Host.SystemWeb;

namespace MarvelousSoftware.Examples.API
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            MarvelousSoftwareHost.UseSystemWeb();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
        }
    }
}
