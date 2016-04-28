using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.UserInterface;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.Wpf.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Pro.TankModuleTree
{
    [ModuleExport("TankModuleTree", typeof(TankModuleTreeModule))]
    [ExportMetadata("Guid", "C229BE91-98D2-479C-9199-19EFF9962EC3")]
    [ExportMetadata("Name", "Tank Module Tree")]
    [ExportMetadata("Description", "View model tree of tanks")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "Smellyriver")]
    public class TankModuleTreeModule : ModuleBase
    {
        public override void Initialize()
        {
            DocumentServiceManager.Instance.Register(TankModuleTreeDocumentService.Instance);

            TankCommandManager.Instance.Register(new TankCommand(guid: TankModuleTreeDocumentServiceBase.ViewTankModuleTreeCommandGuid, 
                                                                 name: "View Module Tree", 
                                                                 execute: this.ViewModuleTree, 
                                                                 priority: TankModuleTreeDocumentServiceBase.ViewTankModuleTreeCommandPriority,
                                                                 icon: BitmapImageEx.LoadAsFrozen("Resources/Images/ModuleTree_16.png")));
        }

        private void ViewModuleTree(TankUnikey key)
        {
            var uri = TankModuleTreeDocumentServiceBase.CreateUri(key);
            DockingViewManager.Instance.DocumentManager.OpenDocument(uri);
        }
    }
}
