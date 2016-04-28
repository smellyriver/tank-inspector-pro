using System;
using System.Windows.Input;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.ModelShared;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    partial class ModelDocumentVM : ModelDocumentVMBase, ICustomizationConfigurable
    {

        public new ModelDocumentPersistentInfo PersistentInfo
        {
            get { return (ModelDocumentPersistentInfo)base.PersistentInfo; }
        }

        public new ModelViewVM ModelView
        {
            get { return (ModelViewVM)base.ModelView; }
        }

        public ModelDocumentService ModelDocumentService { get; private set; }

        public string Name { get { return this.TankInstance.Tank.Name; } }

        CustomizationConfiguration ICustomizationConfigurable.CustomizationConfiguration
        {
            get { return this.TankInstance.CustomizationConfiguration; }
        }

        event EventHandler ICustomizationConfigurable.CustomizationConfigurationChanged { add { } remove { } }

        public CaptureVM Capture { get; private set; }


        public ModelDocumentVM(ModelDocumentService modelDocumentService,
                               CommandBindingCollection commandBindings,
                               TankInstance tank,
                               string persistentInfo)
            : base(commandBindings, tank, persistentInfo)
        {
            this.ModelDocumentService = modelDocumentService;
            this.Capture = new CaptureVM(this);
        }

        protected override ModelVMBase CreateModelVM(TankInstance tankInstance)
        {
            return new ModelViewVM(tankInstance);
        }

        protected override ModelDocumentPersistentInfoBase LoadPersistentInfo(string persistentInfo)
        {
            return DocumentPersistentInfoProviderBase.Load(persistentInfo, () => new ModelDocumentPersistentInfo());
        }

    }
}
