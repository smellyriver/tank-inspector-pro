using log4net;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class TankDocumentServiceBase : IDocumentService
    {
        public abstract Guid Guid { get; }

        public abstract string[] SupportedSchemes { get; }

        public abstract ILog Log { get; }


        [Import]
        protected IRepositoryManager RepositoryManager { get; private set; }

        public DocumentInfo CreateDocument(Uri uri, Guid guid, string persistentInfo)
        {
            var match = Regex.Match(uri.LocalPath, "(.+);(.+):(.+)");

            if (!match.Success)
            {
                this.Log.WarnFormat("invalid stats Uri format: {0}", uri);
                return null;
            }

            var repositoryId = match.Groups[1].Value;
            var nationKey = Uri.UnescapeDataString(match.Groups[2].Value);
            var tankKey = Uri.UnescapeDataString(match.Groups[3].Value);

            var repository = this.RepositoryManager[repositoryId];
            if (repository == null)
            {
                this.Log.WarnFormat("unknown repository: {0}", repositoryId);
                return null;
            }

            var tank = repository.GetTank(nationKey, tankKey);
            if (repository == null)
            {
                this.Log.WarnFormat("unable to find tank '{0}:{1}' in repository '{2}'", nationKey, tankKey, repositoryId);
                return null;
            }

            return this.CreateDocument(repository, uri, guid, tank, persistentInfo);
        }

        protected abstract DocumentInfo CreateDocument(IRepository repository, Uri uri, Guid guid, IXQueryable tankElement, string persistentInfo);
    }
}
