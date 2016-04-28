using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole
{
    [ModuleExport("InteractiveConsole", typeof(InteractiveConsoleModule))]
    [ExportMetadata("Guid", "C060F96A-D689-4B13-8879-F80DA87D5027")]
    [ExportMetadata("Name", "#interactive_console:module_name")]
    [ExportMetadata("Description", "#interactive_console:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#interactive_console:module_provider")]
    public class InteractiveConsoleModule : ModuleBase
    {
        public override void Initialize()
        {
            var panel = new PanelInfo(Guid.Parse("0ABFF437-1F61-41EB-B0EA-BFCEA65A174B"))
            {
                Title = this.L("interactive_console", "panel_title"),
                CanHide = true,
                CanClose = true,
                CanFloat = true,
                Width = 200,
                Content = new InteractiveView()
            };

            DockingViewManager.Instance.PanelManager.Register(panel);
        }
    }
}
