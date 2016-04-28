using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.Input;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Menus;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public abstract class FlowDocumentModuleBase : ModuleBase
    {
        private static bool s_isInitialized;
        private static void InitializeMenuItems()
        {
            if (s_isInitialized)
                return;

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("document", "export_to_xps_document_menu_item"), FlowDocumentCommands.ExportXps)
            {
                Icon = BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/XPS_16.png")
            }, MenuAnchor.Export);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("document", "export_to_rtf_document_menu_item"), FlowDocumentCommands.ExportRtf)
            {
                Icon = BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/RTF_16.png")
            }, MenuAnchor.Export);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("document", "export_to_text_file_menu_item"), FlowDocumentCommands.ExportTxt)
            {
                Icon = BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/TXT_16.png")
            }, MenuAnchor.Export);

            s_isInitialized = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            FlowDocumentModuleBase.InitializeMenuItems();
        }
    }
}
