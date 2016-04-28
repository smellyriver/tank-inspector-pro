using log4net;
using Smellyriver.TankInspector.Pro.Data.Tank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class TankModuleTreeDocumentServiceBase : TankDocumentServiceBase
    {
        public static readonly Guid ViewTankModuleTreeCommandGuid = Guid.Parse("58DB1223-AE5B-4B4A-94C4-4AAF317375A7");

        public const int ViewTankModuleTreeCommandPriority = 0;
        public const string TankModuleTreeScheme = "moduletree";


        public static Uri CreateUri(TankUnikey key)
        {
            return key.CreateUri(TankModuleTreeDocumentServiceBase.TankModuleTreeScheme);
        }

        private readonly Lazy<ILog> _lazyLog = new Lazy<ILog>(() => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public override ILog Log
        {
            get { return _lazyLog.Value; }
        }

        public override string[] SupportedSchemes
        {
            get { return new[] { TankModuleTreeDocumentServiceBase.TankModuleTreeScheme }; }
        }
    }
}
