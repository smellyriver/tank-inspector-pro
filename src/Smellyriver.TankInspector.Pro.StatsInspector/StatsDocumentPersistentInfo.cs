using System.Runtime.Serialization;
using System.Windows.Controls;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.StatsInspector
{
    [DataContract]
    class StatsDocumentPersistentInfo : DocumentPersistentInfoProviderBase
    {
        [DataMember]
        public FlowDocumentReaderViewingMode ViewingMode { get; set; }

        [DataMember]
        public StatValueMode ValueMode { get; set; }

        [DataMember]
        public string TemplateFilename { get; set; }

        [DataMember]
        public string SaveAsPath { get; set; }

        public StatsDocumentPersistentInfo(TankInstance instance)
        {
            this.ViewingMode = StatsInspectorSettings.Default.StatDocumentViewingMode;
            this.ValueMode = (StatValueMode)StatsInspectorSettings.Default.StatValueMode;
            this.TemplateFilename = StatsInspectorSettings.Default.LastTemplateFilename;
        }

    }
}
