using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    [ModuleExport("TankMuseum", typeof(TankMuseumModule))]
    [ExportMetadata("Guid", "883365D1-D087-4A73-9BE7-DD43F679061B")]
    [ExportMetadata("Name", "#tank_museum:module_name")]
    [ExportMetadata("Description", "#tank_museum:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#tank_museum:module_provider")]
    public class TankMuseumModule : ModuleBase
    {
        public override void Initialize()
        {
            var panel = new PanelInfo(Guid.Parse("13EF27D4-DDB2-46C3-B4E0-D8EDF00B0153"))
            {
                Title = this.L("tank_museum", "panel_title"),
                CanHide = true,
                CanClose = true,
                CanFloat = true,
                Width = 200,
                Content = new TankMuseumView()
            };

            DockingViewManager.Instance.PanelManager.Register(panel);
        }
    }
}
