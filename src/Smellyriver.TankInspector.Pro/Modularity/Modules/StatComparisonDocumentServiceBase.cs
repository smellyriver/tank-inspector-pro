using System;
using System.Reflection;
using log4net;

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
                                         StatComparsionScheme,
                                         Guid.NewGuid().ToString()),
                           UriKind.Absolute);
        }

        private readonly Lazy<ILog> _lazyLog 
            = new Lazy<ILog>(() => SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public virtual ILog Log
        {
            get { return _lazyLog.Value; }
        }

        public abstract Guid Guid { get; }

        public virtual string[] SupportedSchemes
        {
            get { return new[] { StatComparsionScheme }; }
        }

        public abstract ICreateDocumentTask CreateCreateDocumentTask(Uri uri, Guid guid, string persistentInfo);
    }
}
