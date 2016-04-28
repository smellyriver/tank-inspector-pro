using System;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class IComparableExtensions
    {
        public static T Clamp<T>(this T value, T minValue, T maxValue)
            where T : IComparable<T>
        {
            if (value.CompareTo(minValue) < 0)
                return minValue;
            else if (value.CompareTo(maxValue) > 0)
                return maxValue;
            else
                return value;
        }
    }
}
