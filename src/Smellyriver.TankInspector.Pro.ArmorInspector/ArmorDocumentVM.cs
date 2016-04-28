using System;
using System.Windows.Input;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.ModelShared;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;

namespace Smellyriver.TankInspector.Pro.ArmorInspector
{
    partial class ArmorDocumentVM : ModelDocumentVMBase, ICustomizationConfigurable
    {

        public new ArmorDocumentPersistentInfo PersistentInfo
        {
            get { return (ArmorDocumentPersistentInfo)base.PersistentInfo; }
        }

        public ArmorViewVM ArmorView
        {
            get { return (ArmorViewVM)this.ModelView; }
        }

        public ArmorDocumentService ModelDocumentService { get; private set; }

        public string Name { get { return this.TankInstance.Tank.Name; } }

        CustomizationConfiguration ICustomizationConfigurable.CustomizationConfiguration
        {
            get { return this.TankInstance.CustomizationConfiguration; }
        }

        event EventHandler ICustomizationConfigurable.CustomizationConfigurationChanged { add { } remove { } }

        public CaptureVM Capture { get; private set; }


        public ArmorDocumentVM(ArmorDocumentService modelDocumentService,
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
            return new ArmorViewVM(tankInstance);
        }

        protected override ModelDocumentPersistentInfoBase LoadPersistentInfo(string persistentInfo)
        {
            return DocumentPersistentInfoProviderBase.Load(persistentInfo, () => new ArmorDocumentPersistentInfo());
        }

    }
}
