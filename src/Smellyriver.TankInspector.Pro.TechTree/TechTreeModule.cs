using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.TechTree
{
    [ModuleExport("TechTree", typeof(TechTreeModule))]
    [ExportMetadata("Guid", "5A24E364-0457-492A-A39F-115F5CCCBA0A")]
    [ExportMetadata("Name", "#techtree:module_name")]
    [ExportMetadata("Description", "#techtree:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#techtree:module_provider")]
    public class TechTreeModule : ModuleBase
    {
        

        internal static readonly BitmapImage TechTreeIcon = BitmapImageEx.LoadAsFrozen("Resources/Images/TechTree_16.png");


        public override void Initialize()
        {
            DocumentServiceManager.Instance.Register(TechTreeDocumentService.Instance);

            NationCommandManager.Instance.Register(new NationCommand(guid: TechTreeDocumentServiceBase.ViewNationalTechTreeCommandGuid,
                                                                     name: this.L("techtree", "view_techtree_menu_item"), 
                                                                     execute: this.ViewTechTree,
                                                                     canExecute: this.CanViewTechTree,
                                                                     priority: TechTreeDocumentServiceBase.ViewNationalTechTreeCommandPriority,
                                                                     icon: TechTreeIcon));
        }

        private void ViewTechTree(NationUnikey key)
        {
            var uri = TechTreeDocumentServiceBase.CreateUri(key.RepositoryID);

            var document = DockingViewManager.Instance.DocumentManager.OpenDocument(uri)
                .ContinueWith(t =>
                    {
                        App.BeginInvokeBackground(() =>
                            {
                                if (t.Result == null)
                                {
                                    this.LogError("failed to create tech tree document");
                                    return;
                                }

                                var vm = t.Result.Content.DataContext as TechTreeDocumentVM;
                                if (vm == null)
                                {
                                    this.LogError("failed to retrieve tech tree document view model");
                                    return;
                                }

                                vm.SelectNation(key.NationKey);
                            });
                    });
        }

        private bool CanViewTechTree(NationUnikey key)
        {
            return RepositoryManager.Instance[key.RepositoryID] is LocalGameClient;
        }
    }
}
