using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    [DataContract(Namespace = Stat.Xmlns)]
    public enum BenchmarkThresholdType
    {
        [EnumMember]
        Absolute,
        [EnumMember]
        Relative
    }
}
