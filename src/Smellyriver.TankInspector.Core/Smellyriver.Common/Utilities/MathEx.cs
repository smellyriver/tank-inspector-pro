using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.Utilities
{
    internal static class MathEx
    {
        public static T Max<T>(T first, params T[] others)
            where T: IComparable<T>
        {
            T max = first;
            foreach (var value in others)
            {
                if (value.CompareTo(max) > 0)
                    max = value;
            }

            return max;
        }

        public static T Min<T>(T first, params T[] others)
            where T : IComparable<T>
        {
            T min = first;
            foreach (var value in others)
            {
                if (value.CompareTo(min) < 0)
                    min = value;
            }

            return min;
        }

        public static double NormalizeAngle(double degrees)
        {
            while (degrees >= 360)
                degrees -= 360;

            while (degrees < 0)
                degrees += 360;

            return degrees;
        }

    }
}
