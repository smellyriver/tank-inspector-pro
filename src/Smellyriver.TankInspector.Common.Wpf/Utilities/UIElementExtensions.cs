using System.Windows;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class UIElementExtensions
    {
        public static void MeasureAndArrange(this UIElement target)
        {
            target.Measure(new Size(double.MaxValue, double.MaxValue));
            target.Arrange(new Rect(new Point(0, 0), target.DesiredSize));
        }
    }
}
