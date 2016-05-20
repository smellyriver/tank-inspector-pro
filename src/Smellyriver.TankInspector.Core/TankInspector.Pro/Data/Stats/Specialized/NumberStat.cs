using System;
using System.Globalization;
using System.Runtime.Serialization;

using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Pro.Data.Stats.Specialized
{
    [DataContract(Name = "Number", Namespace = Stat.Xmlns)]
    public class NumberStat : XPathStat<double>
    {
        private readonly double? _maximum;

        public double? Maximum
        {
            get { return _maximum; }
        }

        private readonly double? _minimum;

        public double? Minimum
        {
            get { return _minimum; }
        }

        public NumberStat(string key,
                          string name,
                          string shortName,
                          string description,
                          string unit,
                          string xpath,
                          string baseValueXPath,
                          double? maximum,
                          double? minimum,
                          string showCondition,
                          CompareStrategy compareStrategy,
                          string formatString)
            : base(key, name, shortName, description, unit, xpath, baseValueXPath, showCondition, compareStrategy, formatString)
        {
            this._maximum = maximum;
            this._minimum = minimum;
        }

        protected override string GetValue(IXQueryable queryable, IRepository repository, string xpath)
        {
            var result = base.GetValue(queryable, repository, xpath);

            if (string.IsNullOrEmpty(result))
            {
                Core.Support.LogWarning(this,
                                        string.Format(
                                                      "error getting number stat value (key='{0}', xpath='{1}')",
                                                      this.Key,
                                                      xpath));
                return "0.0";
            }

            return result;
        }

        protected override double? GetBenchmarkValue(IXQueryable queryable, IRepository repository, string xpath)
        {
            var value = queryable.QueryValue(xpath);

            if (string.IsNullOrEmpty(value))
            {
                Core.Support.LogWarning(this,
                                        string.Format(
                                                      "error getting number stat value (key='{0}', xpath='{1}')",
                                                      this.Key,
                                                      xpath));
                return null;
            }

            return double.Parse(value);
        }

        protected override double Convert(string value)
        {
            return double.Parse(value, CultureInfo.InvariantCulture).Clamp(this.Minimum ?? double.MinValue, this.Maximum ?? double.MaxValue);
        }

        protected override double? InternalGetDifferenceRatio(double value1, double value2)
        {
            return (value1 - value2) / value2;
        }

        protected override double? InternalGetDifference(double value1, double value2)
        {
            return value1 - value2;
        }
    }
}
