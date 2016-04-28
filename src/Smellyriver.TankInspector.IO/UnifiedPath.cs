using System;
using IOPath = System.IO.Path;

namespace Smellyriver.TankInspector.IO
{
    public static class UnifiedPath
    {
        private static readonly char[] s_extensionNameSearchChars = new[] { '.', IOPath.DirectorySeparatorChar, IOPath.AltDirectorySeparatorChar, PackageBoundarySeparator };
        private static readonly char[] s_fileNameSearchChars = new[] { IOPath.DirectorySeparatorChar, IOPath.AltDirectorySeparatorChar, PackageBoundarySeparator };


        public const char PackageBoundarySeparator = '>';

        public static string GetPackageFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var separatorIndex = path.LastIndexOf(PackageBoundarySeparator);
            if (separatorIndex == -1)
                return null;

            return path.Substring(0, separatorIndex);
        }

        public static string GetLocalPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var separatorIndex = path.LastIndexOf(PackageBoundarySeparator);
            if (separatorIndex == -1)
                return path;

            return path.Substring(separatorIndex + 1).Replace('\\', '/');
        }

        public static bool IsInPackage(string path)
        {
            return path.IndexOf(PackageBoundarySeparator) != -1;
        }

        public static void ParsePath(string path, out string packagePath, out string localPath)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var separatorIndex = path.LastIndexOf(PackageBoundarySeparator);
            if (separatorIndex == -1)
            {
                packagePath = null;
                localPath = path;
                return;
            }

            packagePath = path.Substring(0, separatorIndex);
            localPath = path.Substring(separatorIndex + 1).Replace('\\', '/');
        }

        public static string GetFileName(string path)
        {
            if (path == null)
                return null;

            var separatorIndex = path.LastIndexOfAny(s_fileNameSearchChars);
            return separatorIndex == -1 ? path : path.Substring(separatorIndex + 1);
        }

        public static string Combine(string packagePath, string localPath)
        {
            return string.Format("{0}{1}{2}", packagePath, PackageBoundarySeparator, localPath);
        }


        public static string GetExtension(string path)
        {
            if (path == null)
                return null;

            var separatorIndex = path.LastIndexOfAny(s_extensionNameSearchChars);
            return separatorIndex == -1 ? path : path.Substring(separatorIndex);
        }
    }
}
