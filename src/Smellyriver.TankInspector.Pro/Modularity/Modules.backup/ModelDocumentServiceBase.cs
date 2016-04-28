using log4net;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class ModelDocumentServiceBase : TankDocumentServiceBase
    {
        public static readonly Guid InspectModelCommandGuid = Guid.Parse("98E25EF4-3EEB-4FEA-8FCB-9783C0868E15");
        public const int InspectModelCommandPriority = -40;

        public const string ModelScheme = "model";

        public static Uri CreateUri(TankUnikey key)
        {
            return key.CreateUri(ModelDocumentServiceBase.ModelScheme);
        }

        private readonly Lazy<ILog> _lazyLog = new Lazy<ILog>(() => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public override ILog Log
        {
            get { return _lazyLog.Value; }
        }

        public override string[] SupportedSchemes
        {
            get { return new[] { ModelDocumentServiceBase.ModelScheme }; }
        }
    }
}
