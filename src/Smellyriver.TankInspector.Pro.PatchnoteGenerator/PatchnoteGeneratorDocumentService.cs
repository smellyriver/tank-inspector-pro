using System;
using System.Text.RegularExpressions;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator
{
    class PatchnoteGeneratorDocumentService : IDocumentService
    {
        
        public static PatchnoteGeneratorDocumentService Instance { get; private set; }

        static PatchnoteGeneratorDocumentService()
        {
            PatchnoteGeneratorDocumentService.Instance = new PatchnoteGeneratorDocumentService();
        }

        private static Guid s_guid = Guid.Parse("5D501B34-7746-481A-A296-6B6D7329DD10");

        public const string Scheme = "patchnote";

        public static Uri CreateUri(IRepository target, IRepository reference)
        {
            return new Uri(string.Format("{0}://{1}?ref={2}",
                                         Scheme,
                                         target.ID,
                                         Uri.EscapeDataString(reference.ID)),
                           UriKind.Absolute);
        }

        public Guid Guid
        {
            get { return s_guid; }
        }

        public string[] SupportedSchemes
        {
            get { return new[] { Scheme }; }
        }

        private PatchnoteGeneratorDocumentService()
        {

        }

        public ICreateDocumentTask CreateCreateDocumentTask(Uri uri, Guid guid, string persistentInfo)
        {
            var targetRepositoryId = uri.LocalPath;
            var refMatch = Regex.Match(uri.Query, @"\?ref\=(.+)");

            if(!refMatch.Success)
            {
                this.LogWarning("invalid patchnote Uri format: {0}", uri);
                return null;
            }

            var referenceRepositoryId = Uri.UnescapeDataString( refMatch.Groups[1].Value);

            var targetRepository = RepositoryManager.Instance[targetRepositoryId];

            if(targetRepository == null)
            {
                this.LogWarning("invalid target repository: {0}", targetRepositoryId);
                return null;
            }

            var referenceRepository = RepositoryManager.Instance[referenceRepositoryId];

            if (referenceRepository == null)
            {
                this.LogWarning("invalid reference repository: {0}", referenceRepositoryId);
                return null;
            }

            return CreateDocumentTask.FromFactory(() =>
                {
                    var view = new PatchnoteGeneratorDocumentView();
                    var vm = new PatchnoteGeneratorDocumenVM(view.CommandBindings, targetRepository, referenceRepository, persistentInfo);
                    view.ViewModel = vm;

                    var docInfo = new DocumentInfo(guid: guid,
                                                   repositoryId: targetRepositoryId,
                                                   uri: uri,
                                                   title: this.L("patchnote_generator", "document_title"),
                                                   content: view,
                                                   persistentInfoProvider: vm.PersistentInfo);

                    return docInfo;
                });
        }
    }
}
