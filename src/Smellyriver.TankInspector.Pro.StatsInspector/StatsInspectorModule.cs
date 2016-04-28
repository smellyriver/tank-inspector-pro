using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.StatsInspector
{
    [ModuleExport("StatsInspector", typeof(StatsInspectorModule))]
    [ExportMetadata("Guid", "947DEB4A-AE9B-40A8-8D64-33DFC70C3923")]
    [ExportMetadata("Name", "#stats_inspector:module_name")]
    [ExportMetadata("Description", "#stats_inspector:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#stats_inspector:module_provider")]
    public class StatsInspectorModule : FlowDocumentModuleBase
    {
        public override void Initialize()
        {
            DocumentServiceManager.Instance.Register(StatsDocumentService.Instance);

            TankCommandManager.Instance.Register(new TankCommand(guid: StatsDocumentServiceBase.InspectStatsCommandGuid,
                                                                 name: this.L("stats_inspector", "inspect_stats_menu_item"), 
                                                                 execute: this.InspectStats, 
                                                                 priority: StatsDocumentServiceBase.InspectStatsCommandPriority,
                                                                 icon: BitmapImageEx.LoadAsFrozen("Resources/Images/InspectStats_16.png")));

            base.Initialize();
        }

        private void InspectStats(TankUnikey key)
        {
            var uri = StatsDocumentServiceBase.CreateUri(key);
            DockingViewManager.Instance.DocumentManager.OpenDocument(uri);
        }
    }
}
