using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.Utilities
{
    internal static class IntegerExtensions
    {
        public static int Clamp(this int value, int minValue, int maxValue)
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
