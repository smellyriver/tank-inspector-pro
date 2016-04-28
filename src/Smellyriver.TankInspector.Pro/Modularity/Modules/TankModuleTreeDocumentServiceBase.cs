using System;
using System.Reflection;
using log4net;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class TankModuleTreeDocumentServiceBase : TankDocumentServiceBase
    {
        public static readonly Guid ViewTankModuleTreeCommandGuid = Guid.Parse("58DB1223-AE5B-4B4A-94C4-4AAF317375A7");

        public const int ViewTankModuleTreeCommandPriority = 0;
        public const string TankModuleTreeScheme = "moduletree";


        public static Uri CreateUri(TankUnikey key)
        {
            return key.CreateUri(TankModuleTreeScheme);
        }

        private readonly Lazy<ILog> _lazyLog 
            = new Lazy<ILog>(() => SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public override ILog Log
        {
            get { return _lazyLog.Value; }
        }

        public override string[] SupportedSchemes
        {
            get { return new[] { TankModuleTreeScheme }; }
        }
    }
}
