using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DataContract(Name = "SimpleCollection", Namespace = TankDataFieldManager.Xmlns)]
    class SimpleCollectionDataField : TankDataFieldBase
    {
        [DataMember]
        public string ItemName { get; set; }
        public override TankDataFieldType FieldType
        {
            get { return TankDataFieldType.SimpleCollection; }
        }
    }
}
