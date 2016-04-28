using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.TankModuleTree
{
    class TankModuleTreeDocumentService : TankModuleTreeDocumentServiceBase
    {
        public static readonly TankModuleTreeDocumentService Instance
            = new TankModuleTreeDocumentService();

        private readonly Guid _guid = Guid.Parse("095765D1-9D54-475C-AE97-E1BC7828BD93");
        public override Guid Guid
        {
            get { return _guid; }
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

                    var view = new TankModuleTreeDocumentView();
                    var vm = new TankModuleTreeDocumentVM(this, view.CommandBindings, tankInstance, persistentInfo);
                    view.ViewModel = vm;

                    return new DocumentInfo(guid: guid,
                                            repositoryId: repository.ID,
                                            uri: uri,
                                            title: string.Format("{0} [module tree]", tankInstance.Tank.Name),
                                            content: view,
                                            features: new IFeature[] { vm });
                });
        }
    }
}
