using System;
using System.Reflection;
using log4net;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class StatsDocumentServiceBase : TankDocumentServiceBase
    {
        public static readonly Guid InspectStatsCommandGuid = Guid.Parse("8F6C1897-E565-42E6-9A5C-32089D1B27CF");

        public const int InspectStatsCommandPriority = -20;
        public const string StatsScheme = "stats";
        

        public static Uri CreateUri(TankUnikey key)
        {
            return key.CreateUri(StatsScheme);
        }

        private readonly Lazy<ILog> _lazyLog 
            = new Lazy<ILog>(() => SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public override ILog Log
        {
            get { return _lazyLog.Value; }
        }
        
        public override string[] SupportedSchemes
        {
            get { return new[] { StatsScheme }; }
        }
    }
}
