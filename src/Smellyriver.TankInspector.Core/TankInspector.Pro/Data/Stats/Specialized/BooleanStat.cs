using System.Collections.Generic;
using System.Runtime.Serialization;

using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Data.Stats.Specialized
{

    [DataContract(Name = "Boolean", Namespace = Stat.Xmlns)]
    public class BooleanStat : XPathStat<bool>
    {


        public BooleanStat(string key,
                           string name,
                           string shortName,
                           string description,
                           string xpath,
                           string baseValueXPath,
                           string showCondition,
                           CompareStrategy compareStrategy,
                           string formatString)
            : base(key, name, shortName, description, null, xpath, baseValueXPath, showCondition, compareStrategy, formatString)
        {

        }

        protected override string GetValue(IXQueryable queryable, IRepository repository, string xpath)
        {
            var result = base.GetValue(queryable, repository, xpath);

            if (string.IsNullOrEmpty(result))
            {
                Core.Support.LogError(this,
                                      string.Format("error getting boolean stat value (key='{0}', xpath='{1}')",
                                                    this.Key,
                                                    xpath));
                return "false";
            }

            return result;
        }

        

        protected override bool Convert(string value)
        {
            return bool.Parse(value);
        }

        protected override double? InternalGetDifferenceRatio(bool value1, bool value2)
        {
            if (value1 == value2)
                return 0.0;
            else if (value1 == true)
                return 1.0;
            else
                return -1.0;
        }

        protected override double? InternalGetDifference(bool value1, bool value2)
        {
            if (value1 == value2)
                return 0.0;
            else if (value1 == true)
                return 1.0;
            else
                return -1.0;
        }

    }
}
