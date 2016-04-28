namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class DoubleExtensions
    {
        public static double Clamp(this double value, double minValue, double maxValue)
        {
            if (value < minValue)
                return minValue;
            else if (value > maxValue)
                return maxValue;
            else
                return value;
        }
    }
}
