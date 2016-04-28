using System.Reflection;
using Smellyriver.TankInspector.Pro.Globalization;

namespace Smellyriver.TankInspector.Pro
{
    public static class LocalizationExtensions
    {
        public static string L(this object @this, string compositeKey, Assembly assembly = null)
        {
            if (compositeKey == null)
                return null;

            var trimmedCompositeKey = compositeKey.Trim();
            if (trimmedCompositeKey.StartsWith("#"))
            {
                var colonPosition = trimmedCompositeKey.IndexOf(':', 1);
                if (colonPosition == -1)
                    return compositeKey;

                var catalogName = trimmedCompositeKey.Substring(1, colonPosition - 1);
                var key = trimmedCompositeKey.Substring(colonPosition + 1);
                return Localization.Instance.Translate(key, catalogName, assembly ?? Assembly.GetCallingAssembly());
            }

            return compositeKey;
        }

        public static string L(this object @this, string catalogName, string key, Assembly assembly = null)
        {
            return Localization.Instance.Translate(key.Trim(), catalogName.Trim(), assembly ?? Assembly.GetCallingAssembly());
        }

        public static string L(this object @this, string catalogName, string key, params object[] args)
        {
            return string.Format(Localization.Instance.Translate(key.Trim(), catalogName.Trim(), Assembly.GetCallingAssembly()), args);
        }
    }
}
