using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.Utilities
{
    internal static class DoubleExtensions
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
