using System.IO;
using Smellyriver.TankInspector.IO;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    class Utility
    {
        public static PackageStream OpenTexture(IPackageIndexer packageIndexer, string path)
        {
            var packagePath = packageIndexer.GetPackagePath(path);

            if (packagePath == null && Path.GetExtension(path) == ".tga")
            {
                path = path.Substring(0, path.Length - 4) + ".dds";
                packagePath = packageIndexer.GetPackagePath(path);
            }

            return new PackageStream(packagePath, path);
        }
    }
}
