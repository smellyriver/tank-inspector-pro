using System;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.TechTree
{
    class TechTreeDocumentService : TechTreeDocumentServiceBase
    {
        public static readonly TechTreeDocumentService Instance = new TechTreeDocumentService();

        private readonly Guid _guid = Guid.Parse("7393A5A8-FD4B-444F-9E0C-02DF15BCE6FE");
        public override Guid Guid
        {
            get { return _guid; }
        }


        protected override ICreateDocumentTask CreateCreateDocumentTask(Uri uri, Guid guid, IRepository repository, string persistentInfo)
        {
            return CreateDocumentTask.FromFactory(() =>
                {
                    var view = new TechTreeDocumentView();
                    var vm = new TechTreeDocumentVM(this, view.CommandBindings, repository, persistentInfo);
                    view.ViewModel = vm;

                    var docInfo = new DocumentInfo(guid: guid,
                                                   repositoryId: repository.ID,
                                                   uri: uri,
                                                   title: this.L("techtree", "document_title"),
                                                   content: view,
                                                   persistentInfoProvider: vm.PersistentInfo);

                    return docInfo;
                });
        }
    }
}
