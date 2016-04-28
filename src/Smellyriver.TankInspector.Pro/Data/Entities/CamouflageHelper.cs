using System;
using System.Globalization;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    static class CamouflageHelper
    {
        public static Color? QueryColor(IXQueryable queryable, string xpath, byte? fixedAlpha = null)
        {
            var result = queryable[xpath];
            if (result == null)
                return null;

            var values = result.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            var r = byte.Parse(values[0], CultureInfo.InvariantCulture);
            var g = byte.Parse(values[1], CultureInfo.InvariantCulture);
            var b = byte.Parse(values[2], CultureInfo.InvariantCulture);
            var a = fixedAlpha == null ? byte.Parse(values[3], CultureInfo.InvariantCulture) : fixedAlpha.Value;
            return Color.FromArgb(a, r, g, b);
        }

        public static double[] QueryVector4(IXQueryable queryable, string xpath)
        {
            var result = queryable[xpath];
            if (result == null)
                return null;

            var values = result.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            var x = double.Parse(values[0], CultureInfo.InvariantCulture);
            var y = double.Parse(values[1], CultureInfo.InvariantCulture);
            var z = double.Parse(values[2], CultureInfo.InvariantCulture);
            var w = double.Parse(values[3], CultureInfo.InvariantCulture);
            return new double[] { x, y, z, w };
        }
    }
}
