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

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    [ModuleExport("ModelInspector", typeof(ModelInspectorModule))]
    [ExportMetadata("Guid", "506CA948-0027-4958-8D07-F55D49712DA6")]
    [ExportMetadata("Name", "#model_inspector:module_name")]
    [ExportMetadata("Description", "#model_inspector:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#model_inspector:module_provider")]
    public class ModelInspectorModule : ModuleBase
    {

        public override void Initialize()
        {
            DocumentServiceManager.Instance.Register(ModelDocumentService.Instance);

            TankCommandManager.Instance.Register(new TankCommand(guid: ModelDocumentServiceBase.InspectModelCommandGuid,
                                                                 name: this.L("model_inspector", "inspect_model_menu_item"),
                                                                 execute: this.InspectModel,
                                                                 priority: ModelDocumentServiceBase.InspectModelCommandPriority,
                                                                 icon: BitmapImageEx.LoadAsFrozen("Resources/Images/InspectModel_16.png")));

            this.InitializeMenu();
        }

        private void InitializeMenu()
        {
            Menu.RegisterSharedMenuItems();

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "export_tank_mesh_menu_item"), Commands.ExportTankMesh)
            {
            }, MenuAnchor.Export);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "export_textures_menu_item"), Commands.ExportTextures)
            {
            }, MenuAnchor.Export);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "export_uv_map_menu_item"), Commands.ExportUVMap)
            {
            }, MenuAnchor.Export);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "switch_to_undamaged_model_menu_item"), Commands.SwitchToUndamagedModel)
            {
                IsCheckable = true,
                IsChecked = true
            },
            MenuAnchor.View,
            ModelShared.Commands.WireframeGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "switch_to_destroyed_model_menu_item"), Commands.SwitchToDestroyedModel)
            {
                IsCheckable = true,
            },
            MenuAnchor.View,
            ModelShared.Commands.WireframeGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "switch_to_normal_texture_mode_menu_item"), Commands.SwitchToNormalTextureMode)
            {
                IsCheckable = true,
                IsChecked = true
            },
            MenuAnchor.View,
            ModelShared.Commands.WireframeGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "switch_to_grid_texture_mode_menu_item"), Commands.SwitchToGridTextureMode)
            {
                IsCheckable = true,
            },
            MenuAnchor.View,
            ModelShared.Commands.WireframeGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "switch_to_official_texture_source_menu_item"), Commands.SwitchToOfficialTextureSource)
            {
                IsCheckable = true,
                IsChecked = true
            },
            MenuAnchor.View,
            ModelShared.Commands.WireframeGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "switch_to_mod_texture_source_menu_item"), Commands.SwitchToModTextureSource)
            {
                IsCheckable = true,
            },
            MenuAnchor.View,
            ModelShared.Commands.WireframeGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_inspector", "toggle_camouflage_menu_item"), Commands.ToggleCamouflage)
            {
                IsCheckable = true,
            },
            MenuAnchor.View,
            ModelShared.Commands.ShowModulesGroupPriority);
        }

        private void InspectModel(TankUnikey key)
        {
            var uri = ModelDocumentServiceBase.CreateUri(key);
            DockingViewManager.Instance.DocumentManager.OpenDocument(uri);
        }
    }
}
