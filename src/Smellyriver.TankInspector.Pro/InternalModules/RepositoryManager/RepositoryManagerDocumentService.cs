using System;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.InternalModules.RepositoryManager
{
    class RepositoryManagerDocumentService
    {

        public static readonly RepositoryManagerDocumentService Instance
            = new RepositoryManagerDocumentService();

        public DocumentInfo CreateDocument(Uri uri, Guid guid, string persistentInfo)
        {

            var view = new RepositoryManagerDocumentView();
            var vm = new RepositoryManagerDocumentVM(this, view.CommandBindings, persistentInfo);
            view.ViewModel = vm;

            var docInfo = new DocumentInfo(guid: guid,
                                           repositoryId: null,
                                           uri: uri,
                                           title: this.L("game_client_manager", "document_title"),
                                           content: view,
                                           persistentInfoProvider: vm.PersistentInfo);

            return docInfo;

        }
    }
}
