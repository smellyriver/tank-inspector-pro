using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    [DataContract]
    class StatComparisonDocumentPersistentInfo: DocumentPersistentInfoProviderBase
    {
        [DataMember]
        public StatValueMode ValueMode { get; set; }

        [DataMember]
        public ColumnMode ColumnMode { get; set; }

        [DataMember]
        public string TemplateFilename { get; set; }

        [DataMember]
        public double EditPanelHeight { get; set; }

        [DataMember]
        public bool IsEditPanelShown { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string[] StatKeys { get; set; }

        [DataMember]
        public TankUnikey[] TankKeys { get; set; }

        [DataMember]
        public TankUnikey? BenchmarkTankKey { get; set; }

        [DataMember]
        public string SaveAsPath { get; set; }

        public StatComparisonDocumentPersistentInfo()
        {
            this.ValueMode = (StatValueMode)StatComparerSettings.Default.StatValueMode;
            this.ColumnMode = (ColumnMode)StatComparerSettings.Default.ColumnMode;
            this.StatKeys = new string[0];
            this.TankKeys = new TankUnikey[0];
        }

    }
}
