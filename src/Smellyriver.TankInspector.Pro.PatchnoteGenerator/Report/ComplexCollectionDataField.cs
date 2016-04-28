using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DataContract(Name = "ComplexCollection", Namespace = TankDataFieldManager.Xmlns)]
    class ComplexCollectionDataField : TankDataFieldBase
    {
        [DataMember]
        public string ItemName { get; set; }

        [DataMember]
        public bool OmitSingleHeader { get; set; }

        public override TankDataFieldType FieldType
        {
            get { return TankDataFieldType.ComplexCollection; }
        }
    }
}
