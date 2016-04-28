using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class BitmapSourceExtensions
    {
        public static void SaveToFile(this BitmapSource image, string filename)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var stream = File.Create(filename))
                encoder.Save(stream);
        }

        public static BitmapSource ChangeDPI(this BitmapSource bitmapImage, double dpi = 96)
        {
            int width = bitmapImage.PixelWidth;
            int height = bitmapImage.PixelHeight;

            int stride = width * 4; // 4 bytes per pixel
            byte[] pixelData = new byte[stride * height];
            bitmapImage.CopyPixels(pixelData, stride, 0);

            return BitmapSource.Create(width, height, dpi, dpi, PixelFormats.Bgra32, null, pixelData, stride);
        }
    }
}
