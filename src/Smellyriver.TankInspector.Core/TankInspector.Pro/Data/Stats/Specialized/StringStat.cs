using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Stats.Specialized
{
    [DataContract(Name = "String", Namespace = Stat.Xmlns)]
    public class StringStat : XPathStat<string>
    {
        public StringStat(string key,
                          string name,
                          string shortName,
                          string description,
                          string unit,
                          string xpath,
                          string baseValueXPath,
                          string showCondition,
                          CompareStrategy compareStrategy,
                          string formatString)
            : base(key, name, shortName, description, unit, xpath, baseValueXPath, showCondition, compareStrategy, formatString)
        {

        }

        protected override string Convert(string value)
        {
            return value;
        }

        protected override double? InternalGetDifferenceRatio(string value1, string value2)
        {
            return null;
        }

        protected override double? InternalGetDifference(string value1, string value2)
        {
            return null;
        }
    }
}
