using System;
using System.Globalization;
using System.Reflection;

namespace MarvelousSoftware.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static Object GetPropValue(this Object obj, string name)
        {
            if (obj == null) { return null; }

            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static T GetPropValue<T>(this Object obj, String name)
        {
            Object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }

        public static Object GetFieldValue(this Object obj, string name)
        {
            if (obj == null) { return null; }

            foreach (String part in name.Split('.'))
            {
                Type type = obj.GetType();
                FieldInfo info = type.GetField(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj);
            }
            return obj;
        }

        public static bool IsNumeric(this object expression)
        {
            if (expression == null)
                return false;

            double number;
            return Double.TryParse(Convert.ToString(expression, CultureInfo.InvariantCulture), System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out number);
        }
    }
}
