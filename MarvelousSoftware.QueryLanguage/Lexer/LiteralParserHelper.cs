using System;
using System.Collections.Generic;
using System.Globalization;

namespace MarvelousSoftware.QueryLanguage.Lexer
{
    public static class LiteralParserHelper
    {
        public static bool TryParseDate(string value, IEnumerable<string> dateFormats, IFormatProvider formatProvider, out DateTime date)
        {
            foreach (var dateTimeFormat in dateFormats)
            {
                if (DateTime.TryParseExact(value.Trim(), dateTimeFormat, formatProvider, DateTimeStyles.None, out date))
                {
                    return true;
                }
            }

            date = default(DateTime);
            return false;
        }

        public static bool TryParseInteger(string value, Type type, out object number)
        {
            int intValue;
            short shortValue;
            long longValue;

            if ((type == typeof(short) || type == typeof(short?)) && short.TryParse(value, out shortValue))
            {
                number = shortValue;
                return true;
            }

            if ((type == typeof(int) || type == typeof(int?)) && int.TryParse(value, out intValue))
            {
                number = intValue;
                return true;
            }

            if ((type == typeof(long) || type == typeof(long?)) && long.TryParse(value, out longValue))
            {
                number = longValue;
                return true;
            }

            number = null;
            return false;
        }

        public static bool TryParseFloat(string value, Type type, IFormatProvider formatProvider, out object number)
        {
            if (type == typeof(float) || type == typeof(float?))
            {
                float n;
                if (float.TryParse(value, NumberStyles.Any, formatProvider, out n))
                {
                    number = n;
                    return true;
                }
            }

            if (type == typeof(double) || type == typeof(double?))
            {
                double n;
                if (double.TryParse(value, NumberStyles.Any, formatProvider, out n))
                {
                    number = n;
                    return true;
                }
            }


            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                decimal n;
                if (decimal.TryParse(value, NumberStyles.Any, formatProvider, out n))
                {
                    number = n;
                    return true;
                }
            }

            number = null;
            return false;
        }
    }
}