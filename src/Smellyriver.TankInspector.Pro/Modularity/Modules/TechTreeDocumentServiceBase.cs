using System;
using System.Reflection;
using log4net;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class TechTreeDocumentServiceBase : IDocumentService
    {
        public const string TechTreeScheme = "techtree";
        public static readonly Guid ViewNationalTechTreeCommandGuid = Guid.Parse("02B08A09-D9C0-4D64-878C-3474F3F4B4B4");
        public const int ViewNationalTechTreeCommandPriority = 0;

        public static Uri CreateUri(string repositoryId)
        {

            return new Uri(string.Format("{0}://{1}",
                                         TechTreeScheme,
                                         repositoryId),
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
            get { return new[] { TechTreeScheme }; }
        }

        protected abstract ICreateDocumentTask CreateCreateDocumentTask(Uri uri, Guid guid, IRepository repository, string persistentInfo);


        public ICreateDocumentTask CreateCreateDocumentTask(Uri uri, Guid guid, string persistentInfo)
        {
            var repositoryId = uri.LocalPath;
            var repository = RepositoryManager.Instance[repositoryId];
            if (repository == null)
            {
                this.LogWarning("unknown repository: {0}", repositoryId);
                return null;
            }

            return this.CreateCreateDocumentTask(uri, guid, repository, persistentInfo);
        }
    }
}
