using MarvelousSoftware.Grid.DataSource;
using Microsoft.Owin;

namespace MarvelousSoftware.Core.Host.Owin
{
    public class OwinRequestWrapper : IRequestWrapper
    {
        private static OwinContext Context => new OwinContext(OwinRequestScopeContext.Current.Environment);

        public string this[string name] => Context.Request.Query[name];

        public bool Has(string name)
        {
            var context = Context;

            /*var body = new StreamReader(context.Request.Body).ReadToEnd();
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
            var requestData = Encoding.UTF8.GetBytes(body);
            context.Request.Body = new MemoryStream(requestData);
            // ... or
            var formData = context.Request.ReadFormAsync();
            formData.Wait();
            var result = formData.Result;*/

            return context.Request.Query.Get(name) != null;
        }
    }
}