using System;
using System.Linq;
using MarvelousSoftware.Grid.DataSource;

namespace MarvelousSoftware.Core
{
    public class RequestUtils
    {
        private static IRequestWrapper _requestWrapper;

        public static IRequestWrapper GetRequestWrapper()
        {
            if (_requestWrapper != null)
            {
                return _requestWrapper;
            }

            var dataSourceAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.FullName.StartsWith("MarvelousSoftware.Core.Host."));

            if (dataSourceAssembly == null)
            {
                var msg = "In order to use this method MarvelousSoftware.Core.Host.* has to be referenced.";
                throw new CoreException(msg);
            }

            var requestWrapperType = dataSourceAssembly.GetTypes().First(x => x.IsClass && typeof(IRequestWrapper).IsAssignableFrom(x));
            _requestWrapper = (IRequestWrapper)Activator.CreateInstance(requestWrapperType);
            
            return _requestWrapper;
        }
    }
}
