using System;
using System.Reflection;
using log4net;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{

    public abstract class ArmorDocumentServiceBase : TankDocumentServiceBase
    {
        public static readonly Guid InspectArmorCommandGuid = Guid.Parse("1045C9DD-D09A-44A5-98AC-87EE2822E0C1");
        public const int InspectArmorCommandPriority = -20;
        public const string ArmorScheme = "armor";


        public static Uri CreateUri(TankUnikey key)
        {
            return key.CreateUri(ArmorScheme);
        }

        private readonly Lazy<ILog> _lazyLog 
            = new Lazy<ILog>(() => SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public override ILog Log
        {
            get { return _lazyLog.Value; }
        }

        public override string[] SupportedSchemes
        {
            get { return new[] { ArmorScheme }; }
        }
    }
}
