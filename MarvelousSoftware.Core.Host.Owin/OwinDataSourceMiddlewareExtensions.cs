using Owin;

namespace MarvelousSoftware.Core.Host.Owin
{
    public static class OwinDataSourceMiddlewareExtensions
    {
        public static IAppBuilder UseMarvelousSoftware(this IAppBuilder app, bool isThreadsafeItem = true)
        {
            return app.Use(typeof(OwinDataSourceMiddleware), isThreadsafeItem);
        }
    }
}