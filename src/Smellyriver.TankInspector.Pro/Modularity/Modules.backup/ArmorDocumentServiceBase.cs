using log4net;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{

    public abstract class ArmorDocumentServiceBase : TankDocumentServiceBase
    {
        public static readonly Guid InspectModelCommandGuid = Guid.Parse("1045C9DD-D09A-44A5-98AC-87EE2822E0C1");
        public const int InspectArmorCommandPriority = -20;
        public const string ArmorScheme = "armor";


        public static Uri CreateUri(TankUnikey key)
        {
            return key.CreateUri(ArmorDocumentServiceBase.ArmorScheme);
        }

        private readonly Lazy<ILog> _lazyLog = new Lazy<ILog>(() => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public override ILog Log
        {
            get { return _lazyLog.Value; }
        }

        public override string[] SupportedSchemes
        {
            get { return new[] { ArmorDocumentServiceBase.ArmorScheme }; }
        }
    }
}
