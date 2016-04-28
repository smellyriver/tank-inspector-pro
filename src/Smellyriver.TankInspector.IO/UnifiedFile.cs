using System.IO;

namespace Smellyriver.TankInspector.IO
{
    public static class UnifiedFile
    {
        public static bool Exists(string path)
        {
            string packagePath, localPath;
            UnifiedPath.ParsePath(path, out packagePath, out localPath);

            if (packagePath == null)
                return File.Exists(localPath);

            return PackageStream.IsFileExisted(packagePath, localPath);
        }

        public static Stream OpenRead(string path)
        {
            string packagePath, localPath;
            UnifiedPath.ParsePath(path, out packagePath, out localPath);

            if (packagePath == null)
                return File.OpenRead(localPath);

            return new PackageStream(packagePath, localPath);
        }

    }
}
