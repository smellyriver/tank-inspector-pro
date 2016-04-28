using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DataContract(Name = "TankDataField", Namespace = TankDataFieldManager.Xmlns)]
    [KnownType(typeof(ValueDataField))]
    [KnownType(typeof(SimpleCollectionDataField))]
    [KnownType(typeof(ComplexCollectionDataField))]
    abstract class TankDataFieldBase
    {
        [DataMember]
        public string ElementName { get; set; }

        [DataMember]
        public string XPath { get; set; }

        [DataMember]
        public string Name { get; set; }
        public abstract TankDataFieldType FieldType { get; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.Name = this.L(this.Name);
        }
    }
}
