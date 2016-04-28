using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.ModelShared;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Menus;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.ArmorInspector
{
    [ModuleExport("ArmorInspector", typeof(ArmorInspectorModule))]
    [ExportMetadata("Guid", "B630D431-4279-4015-8D62-956117EC1290")]
    [ExportMetadata("Name", "#armor_inspector:module_name")]
    [ExportMetadata("Description", "#armor_inspector:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#armor_inspector:module_provider")]
    public class ArmorInspectorModule : ModuleBase
    {

        public override void Initialize()
        {
            DocumentServiceManager.Instance.Register(ArmorDocumentService.Instance);

            TankCommandManager.Instance.Register(new TankCommand(guid: ArmorDocumentServiceBase.InspectArmorCommandGuid,
                                                                 name: this.L("armor_inspector", "inspect_armor_menu_item"),
                                                                 execute: this.InspectArmor,
                                                                 priority: ArmorDocumentServiceBase.InspectArmorCommandPriority,
                                                                 icon: BitmapImageEx.LoadAsFrozen("Resources/Images/InspectArmor_16.png")));

            this.InitializeMenu();
        }

        private void InitializeMenu()
        {
            Menu.RegisterSharedMenuItems();

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("armor_inspector", "export_tank_mesh_menu_item"), Commands.ExportTankMesh)
            {
            }, MenuAnchor.Export);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("armor_inspector", "export_textures_menu_item"), Commands.ExportTextures)
            {
            }, MenuAnchor.Export);
        }

        private void InspectArmor(TankUnikey key)
        {
            var uri = ArmorDocumentServiceBase.CreateUri(key);
            DockingViewManager.Instance.DocumentManager.OpenDocument(uri);
        }
    }
}
