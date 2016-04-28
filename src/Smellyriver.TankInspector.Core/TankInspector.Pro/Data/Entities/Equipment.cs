using System.Globalization;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class Equipment : Accessory
    {
        public Equipment(IXQueryable equipmentData)
            : base(equipmentData)
        {

        }

        public double GetWeight(double modulesTotalWeight)
        {

            var weightString = this["script/weight"];
            if (weightString != null)
            {
                return double.Parse(weightString, CultureInfo.InvariantCulture);
            }
            else
            {
                var weightFractionString = this["script/vehicleWeightFraction"];
                if (weightFractionString != null)
                {
                    var weightFraction = double.Parse(weightFractionString, CultureInfo.InvariantCulture);
                    return modulesTotalWeight * weightFraction;
                }
            }

            return 0.0;
        }
    }
}
