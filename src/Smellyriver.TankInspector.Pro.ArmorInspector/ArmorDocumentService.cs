using System;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.ArmorInspector
{
    class ArmorDocumentService : ArmorDocumentServiceBase
    {
        public static readonly ArmorDocumentService Instance = new ArmorDocumentService();

        public override Guid Guid
        {
            get { return Guid.Parse("70EEB238-8FD3-4D9C-BF93-17645C1F212C"); }
        }


        private ArmorDocumentService()
        {

        }


        protected override ICreateDocumentTask CreateCreateDocumentTask(IRepository repository, 
                                                                        Uri uri, 
                                                                        Guid guid,
                                                                        IXQueryable tank, 
                                                                        string persistentInfo)
        {
            return CreateDocumentTask.FromFactory(() =>
                {
                    var tankInstance = TankInstanceManager.GetInstance(repository, tank);

                    var view = new ArmorDocumentView();
                    var vm = new ArmorDocumentVM(this, view.CommandBindings, tankInstance, persistentInfo);
                    view.ViewModel = vm;

                    return new DocumentInfo(guid: guid,
                                            repositoryId: repository.ID,
                                            uri: uri,
                                            title: this.L("armor_inspector", "document_title", tankInstance.Tank.Name),
                                            content: view,
                                            features: new IFeature[] { vm },
                                            persistentInfoProvider: vm.PersistentInfo);
                });
        }
    }
}
