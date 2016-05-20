using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Stats.Specialized
{
    [DataContract(Name = "Currency", Namespace = Stat.Xmlns)]
    public class CurrencyStat : StringStat
    {
        public CurrencyStat(string key,
                            string name,
                            string shortName,
                            string description,
                            string unit,
                            string xpath,
                            string baseValueXPath,
                            string showCondition)
            : base(key, name, shortName, description, null, xpath, baseValueXPath, showCondition, CompareStrategy.NotComparable, "{0}")
        {

        }

        protected override string Convert(string value)
        {
            return value == "credit" ? "cr" : "g";
        }

        protected override double? InternalGetDifferenceRatio(string value1, string value2)
        {
            if (value1 == value2)
                return 0.0;

            if (value1 == "g")
                return 1.0;

            return -1.0;
        }

        protected override double? InternalGetDifference(string value1, string value2)
        {
            if (value1 == value2)
                return 0.0;

            if (value1 == "g")
                return 1.0;

            return -1.0;
        }
    }
}
