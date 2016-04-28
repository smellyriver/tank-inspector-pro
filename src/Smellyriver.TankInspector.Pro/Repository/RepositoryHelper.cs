using System;
using System.Security;
using log4net;
using Microsoft.Win32;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public static class RepositoryHelper
    {
        private readonly static ILog log = SafeLog.GetLogger("RepositoryHelper");
        public const string Xmlns = "http://schemas.smellyriver.com/stipro/repositories";

        public static string GetTankFullKey(string nation, string tankKey)
        {
            return string.Format("{0}:{1}", nation, tankKey);
        }

        public static string GetTankFullKeyEscaped(string nation, string tankKey)
        {
            return SecurityElement.Escape(RepositoryHelper.GetTankFullKey(nation, tankKey));
        }

        public static string GetTankHyphenKey(string nation, string tankKey)
        {
            return string.Format("{0}-{1}", nation, tankKey);
        }

        public static string GetTankHyphenKeyEscaped(string nation, string tankKey)
        {
            return SecurityElement.Escape(RepositoryHelper.GetTankHyphenKey(nation, tankKey));
        }

        public static string RegistryGameClientPath
        {
            get
            {
                try
                {
                    var wotReplayIcon = Registry.GetValue(@"HKEY_CLASSES_ROOT\.wotreplay\shell\open\command", "", null) as string;
                    if (!string.IsNullOrEmpty(wotReplayIcon) && wotReplayIcon.StartsWith("\""))
                    {
                        var closingQuote = wotReplayIcon.IndexOf('"', 1);
                        if (closingQuote >= 0)
                        {
                            var lastSlash = wotReplayIcon.LastIndexOfAny(new[] { '\\', '/' }, closingQuote);
                            if (lastSlash >= 0)
                            {
                                return wotReplayIcon.Substring(1, lastSlash);
                            }
                        }
                    }

                    return null;
                }
                catch(Exception ex)
                {
                    log.ErrorFormat("failed to retrieve game client path from registry: {0}", ex.Message);
                    return null;
                }
            }
        }

    }
}
