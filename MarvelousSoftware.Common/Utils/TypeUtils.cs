using System;
using System.Linq;

namespace MarvelousSoftware.Common.Utils
{
    public static class TypeUtils
    {
        /// <summary>
        /// Creates all types of T from current assembly.
        /// </summary>
        public static T[] CreateAllOf<T>(Type typeToSearch = null, params object[] parameters) where T : class
        {
            if (typeToSearch == null)
            {
                typeToSearch = typeof(T);
                if (typeToSearch.GetGenericArguments().Any())
                {
                    // todo: it is possible to do this using "is" operator: 
                    // http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class 
                    var msg = "Generic types are not supported. Please use typeToSearch parameter in order to aviod this issue.";
                    throw new InvalidOperationException(msg);
                }
            }

            var types = typeToSearch.Assembly.GetTypes()
                .Where(x => x.IsSubclassOf(typeToSearch))
                .Where(x => x.IsAbstract == false)
                .ToArray();
            var result = new T[types.Length];

            var genericArgs = typeof(T).GetGenericArguments();
            var parametersTypes = parameters.Select(x => x.GetType()).ToArray();

            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i].MakeGenericType(genericArgs);

                // todo: doesn't work because of different types: 
                // var constructor = types[i].GetConstructor(parametersTypes);
                var constructor = type.GetConstructors().SingleOrDefault();
                if (constructor == null)
                {
                    throw new ArgumentException($"Type {type} has insufficient constructor.");
                }
                result[i] = constructor.Invoke(parameters) as T;
            }

            return result;
        }
    }
}