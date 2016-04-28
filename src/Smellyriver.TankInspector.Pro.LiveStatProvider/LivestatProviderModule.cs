using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.LivestatProvider
{
    [ModuleExport("LivestatProvider", typeof(LivestatProviderModule))]
    [ExportMetadata("Guid", "5E0D0754-4D25-4B72-B2AB-BA1981C908A6")]
    [ExportMetadata("Name", "Livestat Provider")]
    [ExportMetadata("Description", "Provide Livestats (provided by VBAddict.net) for tanks")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "Smellyriver")]
    public class LivestatProviderModule : ModuleBase
    {
        public override void Initialize()
        {
            var livestatProvider = new LivestatProvider();
            StatsProviderManager.Instance.Register(livestatProvider);
        }
    }
}
