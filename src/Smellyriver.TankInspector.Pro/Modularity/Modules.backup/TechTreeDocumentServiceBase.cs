using log4net;
using Smellyriver.TankInspector.Pro.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

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
                                         TechTreeDocumentServiceBase.TechTreeScheme,
                                         repositoryId),
                           UriKind.Absolute);
        }

        private readonly Lazy<ILog> _lazyLog = new Lazy<ILog>(() => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name));
        public virtual ILog Log
        {
            get { return _lazyLog.Value; }
        }

        public abstract Guid Guid { get; }

        [Import]
        protected IRepositoryManager RepositoryManager { get; private set; }

        public virtual string[] SupportedSchemes
        {
            get { return new[] { TechTreeDocumentServiceBase.TechTreeScheme }; }
        }

        public DocumentInfo CreateDocument(Uri uri, Guid guid, string persistentInfo)
        {
            var repositoryId = uri.LocalPath;
            var repository = this.RepositoryManager[repositoryId];
            if (repository == null)
            {
                this.Log.WarnFormat("unknown repository: {0}", repositoryId);
                return null;
            }

            return this.CreateDocument(uri, guid, repository, persistentInfo);
        }

        protected abstract DocumentInfo CreateDocument(Uri uri, Guid guid, IRepository repository, string persistentInfo);
    }
}
