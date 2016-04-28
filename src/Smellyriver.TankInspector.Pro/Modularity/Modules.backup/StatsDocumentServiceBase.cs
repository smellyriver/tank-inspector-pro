using log4net;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class StatsDocumentServiceBase : TankDocumentServiceBase
    {
        public static readonly Guid InspectStatsCommandGuid = Guid.Parse("8F6C1897-E565-42E6-9A5C-32089D1B27CF");

        public const int InspectStatsCommandPriority = -20;
        public const string StatsScheme = "stats";
        

        public static Uri CreateUri(TankUnikey key)
        {
            return key.CreateUri(StatsDocumentServiceBase.StatsScheme);
        }

        private readonly Lazy<ILog> _lazyLog = new Lazy<ILog>(() => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public override ILog Log
        {
            get { return _lazyLog.Value; }
        }
        
        public override string[] SupportedSchemes
        {
            get { return new[] { StatsDocumentServiceBase.StatsScheme }; }
        }
    }
}
