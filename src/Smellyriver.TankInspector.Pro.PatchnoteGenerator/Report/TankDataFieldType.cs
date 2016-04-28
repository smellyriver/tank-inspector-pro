using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DataContract]
    enum TankDataFieldType
    {
        [EnumMember]
        Value,
        [EnumMember]
        SimpleCollection,
        [EnumMember]
        ComplexCollection,
    }
}
