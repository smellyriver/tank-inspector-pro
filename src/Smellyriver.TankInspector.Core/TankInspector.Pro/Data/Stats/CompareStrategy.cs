using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    [DataContract]
    public enum CompareStrategy
    {
        [EnumMember]
        NotComparable,
        [EnumMember]
        HigherBetter,
        [EnumMember]
        LowerBetter,
        [EnumMember]
        Plain
    }
}
