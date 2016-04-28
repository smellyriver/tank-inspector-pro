using System;
using System.Text.RegularExpressions;
using log4net;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class TankDocumentServiceBase : IDocumentService
    {
        public abstract Guid Guid { get; }

        public abstract string[] SupportedSchemes { get; }

        public abstract ILog Log { get; }

        protected abstract ICreateDocumentTask CreateCreateDocumentTask(IRepository repository, 
                                                                        Uri uri, 
                                                                        Guid guid, 
                                                                        IXQueryable tankElement, 
                                                                        string persistentInfo);


        public ICreateDocumentTask CreateCreateDocumentTask(Uri uri, Guid guid, string persistentInfo)
        {
            var match = Regex.Match(uri.LocalPath, "(.+);(.+):(.+)");

            if (!match.Success)
            {
                this.LogWarning("invalid stats Uri format: {0}", uri);
                return null;
            }

            var repositoryId = match.Groups[1].Value;
            var nationKey = Uri.UnescapeDataString(match.Groups[2].Value);
            var tankKey = Uri.UnescapeDataString(match.Groups[3].Value);

            var repository = RepositoryManager.Instance[repositoryId];
            if (repository == null)
            {
                this.LogWarning("unknown repository: {0}", repositoryId);
                return null;
            }

            var tank = repository.GetTank(nationKey, tankKey);
            if (repository == null)
            {
                this.LogWarning("unable to find tank '{0}:{1}' in repository '{2}'", nationKey, tankKey, repositoryId);
                return null;
            }

            return this.CreateCreateDocumentTask(repository, uri, guid, tank, persistentInfo);
        }
    }
}
