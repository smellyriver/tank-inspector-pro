using System.Windows.Media;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class ColorExtensions
    {
        public static double GetLuminance(this Color color)
        {
            var rgbMax = MathEx.Max(color.R, color.G, color.B);
            var rgbMin = MathEx.Min(color.R, color.G, color.B);
            return (double)(rgbMax + rgbMin) / 256.0 / 2.0;
        }
    }
}
