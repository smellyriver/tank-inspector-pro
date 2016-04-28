using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.UserInterface
{
    public static class PackageImage
    {
        private static Dictionary<string, ImageSource> s_packageImageCache;

        static PackageImage()
        {
            s_packageImageCache = new Dictionary<string, ImageSource>();
        }

        public static ImageSource Load(string packagePath, string localPath, double dpi = 96)
        {
            return s_packageImageCache.GetOrCreate(UnifiedPath.Combine(packagePath, localPath),
                () =>
                {
                    try
                    {
                        using (var stream = new PackageStream(packagePath, localPath))
                        {
                            BitmapSource image = BitmapImageEx.FromStream(stream);

                            if (image.DpiX != dpi || image.DpiY != dpi)
                                image = image.ChangeDPI(dpi);

                            image.Freeze();
                            return (ImageSource)image;
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                });
        }

        public static ImageSource Load(IPackageIndexer indexer, string path, double dpi = 96)
        {
            return PackageImage.Load(indexer.GetPackagePath(path), path, dpi);
        }

        public static ImageSource Load(string path, double dpi = 96)
        {
            string packagePath, localPath;
            UnifiedPath.ParsePath(path, out packagePath, out localPath);

            return PackageImage.Load(packagePath, localPath, dpi);
        }

    }
}
