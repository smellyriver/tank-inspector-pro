using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class StatComparisonDocumentServiceBase : IDocumentService
    {
        public const string StatComparsionScheme = "statcmp";
        public static readonly Guid AddToComparisonCommandGuid = Guid.Parse("841E5104-B8B2-4B6E-92EB-FEF7942C62C6");
        public const int AddToComparisonCommandPriority = -10;

        public static Uri CreateUri()
        {

            return new Uri(string.Format("{0}://{1}",
                                         StatComparisonDocumentServiceBase.StatComparsionScheme,
                                         Guid.NewGuid().ToString()),
                           UriKind.Absolute);
        }

        private readonly Lazy<ILog> _lazyLog = new Lazy<ILog>(() => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public virtual ILog Log
        {
            get { return _lazyLog.Value; }
        }

        public abstract Guid Guid { get; }

        public virtual string[] SupportedSchemes
        {
            get { return new[] { StatComparisonDocumentServiceBase.StatComparsionScheme }; }
        }

        public abstract DocumentInfo CreateDocument(Uri uri, Guid guid, string persistentInfo);
    }
}
