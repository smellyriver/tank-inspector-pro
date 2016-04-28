using System;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class ColorEx
    {
        public static Color Interpolate(Color begin, Color end, double offset)
        {
            if (offset < 0 || offset > 1.0)
                throw new ArgumentException("offset");

            return Color.FromArgb(ByteEx.Interpolate(begin.A, end.A, offset),
                                  ByteEx.Interpolate(begin.R, end.R, offset),
                                  ByteEx.Interpolate(begin.G, end.G, offset),
                                  ByteEx.Interpolate(begin.B, end.B, offset));
        }
    }
}
