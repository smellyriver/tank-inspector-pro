using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class BitmapImageEx
    {
        public static BitmapImage FromStream(Stream stream)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        public static BitmapImage LoadAsFrozen(Uri uri)
        {
            var image = new BitmapImage(uri);
            image.Freeze();
            return image;
        }

        public static BitmapImage LoadAsFrozen(Assembly packAssembly, string path)
        {
            var callingASM = Assembly.GetCallingAssembly();
            var uri = new Uri(string.Format("pack://application:,,,/{0};component/{1}", packAssembly.GetName().Name, path), UriKind.Absolute);
            return BitmapImageEx.LoadAsFrozen(uri);
        }

        public static BitmapImage LoadAsFrozen(string path)
        {
            return BitmapImageEx.LoadAsFrozen(Assembly.GetCallingAssembly(), path);
        }
    }
}
