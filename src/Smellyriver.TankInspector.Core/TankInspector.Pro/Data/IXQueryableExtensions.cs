using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Smellyriver.TankInspector.Pro.Data
{
    public static class IXQueryableExtensions
    {

        public static IXQueryable Query(this IXQueryable xqueryable, string xpathFormat, params object[] args)
        {
            return xqueryable.Query(string.Format(xpathFormat, args));
        }

        public static float QueryFloat(this IXQueryable xqueryable, string xpath, float defaultValue = default(float))
        {
            var value = xqueryable.QueryValue(xpath);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            else
                return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public static double QueryDouble(this IXQueryable xqueryable, string xpath, double defaultValue = default(double))
        {
            var value = xqueryable.QueryValue(xpath);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            else
                return double.Parse(value, CultureInfo.InvariantCulture);
        }

        public static int QueryInt(this IXQueryable xqueryable, string xpath, int defaultValue = default(int))
        {
            var value = xqueryable.QueryValue(xpath);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            else
                return int.Parse(value, CultureInfo.InvariantCulture);
        }

        public static bool QueryBool(this IXQueryable xqueryable, string xpath, bool defaultValue = default(bool))
        {
            var value = xqueryable.QueryValue(xpath);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            else
                return bool.Parse(value);
        }

        public static T QueryValue<T>(this IXQueryable xqueryable, string xpath)
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(null, CultureInfo.InvariantCulture, xqueryable.QueryValue(xpath));
        }

        public static string QueryValue(this IXQueryable xqueryable, string xpathFormat, params object[] args)
        {
            return xqueryable.QueryValue(string.Format(xpathFormat, args));
        }

        public static T QueryValue<T>(this IXQueryable xqueryable, string xpathFormat, params object[] args)
        {
            return xqueryable.QueryValue<T>(string.Format(xpathFormat, args));
        }

        public static IEnumerable<IXQueryable> QueryMany(this IXQueryable xqueryable, string xpathFormat, params object[] args)
        {
            return xqueryable.QueryMany(string.Format(xpathFormat, args));
        }

        public static IEnumerable<string> QueryManyValues(this IXQueryable xqueryable, string xpathFormat, params object[] args)
        {
            return xqueryable.QueryManyValues(string.Format(xpathFormat, args));
        }

        public static bool EqualsAt(this IXQueryable @this, IXQueryable other, string xpath)
        {
            return @this[xpath].Equals(other[xpath]);
        }
    }
}
