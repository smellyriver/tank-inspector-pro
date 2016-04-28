using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Data.Stats;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DataContract(Name = "Value", Namespace = TankDataFieldManager.Xmlns)]
    class ValueDataField : TankDataFieldBase
    {
        [DataMember]
        public CompareStrategy CompareStrategy { get; set; }

        [DataMember]
        public string Unit { get; private set; }

        [DataMember]
        public string FormatString { get; private set; }

        public override TankDataFieldType FieldType
        {
            get { return TankDataFieldType.Value; }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.Unit = this.L(this.Unit);
        }
    }
}
