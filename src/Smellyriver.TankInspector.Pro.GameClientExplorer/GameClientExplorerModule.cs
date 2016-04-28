using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    [ModuleExport("GameClientExplorer", typeof(GameClientExplorerModule))]
    [ExportMetadata("Guid", "9F17D473-4305-41B1-BEEA-7DB202584EBF")]
    [ExportMetadata("Name", "#game_client_explorer:module_name")]
    [ExportMetadata("Description", "#game_client_explorer:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#game_client_explorer:module_provider")]
    public class GameClientExplorerModule : ModuleBase
    {

        public override void Initialize()
        {
            var view = new GameClientExplorerView();
            var viewModel = new GameClientExplorerVM(this, view.Dispatcher);
            view.ViewModel = viewModel;
            
            var panel = new PanelInfo(Guid.Parse("DAD9E8C0-14DB-4E24-9437-E7FF6C4D27B1"))
            {
                Title = this.L("game_client_explorer", "panel_title"),
                CanHide = true,
                CanClose = true,
                CanFloat = true,
                Width = 200,
                Content = view
            };

            DockingViewManager.Instance.PanelManager.Register(panel);
        }


    }
}
