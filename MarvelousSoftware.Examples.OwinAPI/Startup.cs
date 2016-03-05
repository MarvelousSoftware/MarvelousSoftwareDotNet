using System.Globalization;
using System.Threading;
using System.Web.Http;
using MarvelousSoftware.Core.Host.Owin;
using Newtonsoft.Json;
using Owin;

namespace MarvelousSoftware.Examples.OwinAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var webApiConfiguration = ConfigureWebApi();
            app.Use((context, next) =>
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
                return next.Invoke();
            });

            app.UseMarvelousSoftware();
            app.UseWebApi(webApiConfiguration);
        }


        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();

            config.EnableCors();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            return config;
        }
    }
}