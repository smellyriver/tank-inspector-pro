using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector
{
    public interface ICoreSupport
    {
        Stream GetStatsXmlStream();
        Stream GetDefaultStatesStream();

        bool EnableXPathStatDebug { get; }

        IRepository GetRepository(string id);

        void LogInfo(object from, string message);
        void LogWarning(object from, string message);
        void LogError(object from, string message);

        string Localize(string catalog, string key);
        string Localize(string compositeKey);
    }
}
