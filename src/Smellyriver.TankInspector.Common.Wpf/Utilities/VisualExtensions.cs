using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class VisualExtensions
    {
        internal static RenderTargetBitmap RenderToBitmap(Visual target, Rect bounds, int dpiX, int dpiY)
        {
            var width = Math.Max((int)(bounds.Right * dpiX / 96.0), 1);
            var height = Math.Max((int)(bounds.Bottom * dpiY / 96.0), 1);

            var renderTarget = new RenderTargetBitmap(width, height, dpiX, dpiY, PixelFormats.Pbgra32);
            var drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                var brush = new VisualBrush(target);
                drawingContext.DrawRectangle(brush, null, bounds);
            }

            renderTarget.Render(drawingVisual);
            return renderTarget;
        }

        public static RenderTargetBitmap RenderToBitmap(this Visual target, int dpiX = 96, int dpiY = 96)
        {
            if (target == null)
                return null;

            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            return RenderToBitmap(target, bounds, dpiX, dpiY);

        }
    }
}
