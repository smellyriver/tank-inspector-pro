using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    [DataContract(Namespace = Stat.Xmlns)]
    public class BenchmarkThreshold
    {
        public const double DefaultRelativeValue = 0.25;

        public static readonly BenchmarkThreshold Default 
            = new BenchmarkThreshold(BenchmarkThresholdType.Relative, DefaultRelativeValue);
        public static BenchmarkThreshold Absolute(double value)
        {
            return new BenchmarkThreshold(BenchmarkThresholdType.Absolute, value);
        }

        public static BenchmarkThreshold Relative(double value)
        {
            return new BenchmarkThreshold(BenchmarkThresholdType.Relative, value);
        }


        [DataMember]
        public BenchmarkThresholdType Type { get; set; }

        [DataMember(IsRequired = true)]
        public double Value { get; set; }

        public BenchmarkThreshold(BenchmarkThresholdType type, double value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
}
