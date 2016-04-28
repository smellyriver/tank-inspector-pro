using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.StatChangesView
{
    [ModuleExport("StatChangesView", typeof(StatChangesViewModule))]
    [ExportMetadata("Guid", "6FFB1569-CE09-4C79-B40E-3F5C57AFBFB0")]
    [ExportMetadata("Name", "Stat Changes View")]
    [ExportMetadata("Description", "Provide stat change view for tank and crew configurators")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "Smellyriver")]
    public class StatChangesViewModule : ModuleBase
    {
        
    }
}
