using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Smellyriver.TankInspector.Pro;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro
{
    class CoreSupport : ICoreSupport
    {


        public bool EnableXPathStatDebug { get { return DebugSettings.Default.EnableXPathStatDebug; } }

        public IRepository GetRepository(string id)
        {
            return RepositoryManager.Instance[id];
        }

        public void LogInfo(object @from, string message)
        {
            @from.LogInfo(message);
        }

        public void LogWarning(object @from, string message)
        {
            @from.LogWarning(message);
        }

        public void LogError(object @from, string message)
        {
            @from.LogError(message);
        }

        public string Localize(string catalog, string key)
        {
            return this.L(catalog, key);
        }

        public string Localize(string compositeKey)
        {
            return this.L(compositeKey);
        }

        public Stream GetStatsXmlStream()
        {
            return File.OpenRead(ApplicationPath.GetDataFile("stats.xml"));
        }

        public Stream GetDefaultStatesStream()
        {
            return File.OpenRead(ApplicationPath.GetDataFile("default_states.xml"));
        }
    }
}
