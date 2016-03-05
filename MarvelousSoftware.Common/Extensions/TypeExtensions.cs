using System;
using System.Reflection;

namespace MarvelousSoftware.Common.Extensions
{
    public static class TypeExtensions
    {

        public static PropertyInfo GetNestedProperty(this Type type, string fullPropertyName)
        {
            if (type == null || String.IsNullOrWhiteSpace(fullPropertyName))
                return null;

            PropertyInfo result = null;
            var parts = fullPropertyName.Split('.');
            foreach (var part in parts)
            {
                result = type.GetProperty(part);
                if (result == null)
                    return null;

                type = result.PropertyType;
            }

            return result;
        }

        public static bool IsNullable(this Type type)
        {
            if (type.IsValueType == false)
                return true; // ref-type

            if (Nullable.GetUnderlyingType(type) != null)
                return true; // Nullable<T>

            return false; // value-type
        }

    }
}
