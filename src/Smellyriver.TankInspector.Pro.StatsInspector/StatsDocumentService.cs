using System;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.StatsInspector
{
    class StatsDocumentService : StatsDocumentServiceBase
    {
        public static readonly StatsDocumentService Instance = new StatsDocumentService();

        public override Guid Guid
        {
            get { return Guid.Parse("A3498BE2-F7A9-4D75-8BAA-98AB4D013494"); }
        }

        private StatsDocumentService()
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

                    var view = new StatsDocumentView();
                    var vm = new StatsDocumentVM(this, view.CommandBindings, tankInstance, persistentInfo);
                    view.ViewModel = vm;

                    return new DocumentInfo(guid: guid,
                                            repositoryId: repository.ID,
                                            uri: uri,
                                            title: this.L("stats_inspector", "document_title", tankInstance.Tank.Name),
                                            content: view,
                                            features: new IFeature[] { vm },
                                            persistentInfoProvider: vm.PersistentInfo);
                });
        }
    }
}
