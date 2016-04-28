using System;
using System.IO;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class PathEx
    {
        public static string EnsureEndsWithSlash(string path, char slashToAppend = '/', bool allowAnotherSlash = true)
        {
            char lastChar = path[path.Length - 1];
            if (allowAnotherSlash)
            {
                return path = lastChar == '/' || lastChar == '\\'
                    ? path
                    : path + slashToAppend;
            }
            else
            {
                char anotherSlash = slashToAppend == '/' ? '\\' : '/';
                if (lastChar == anotherSlash)
                    return path.Substring(path.Length - 1) + slashToAppend;
                else if (lastChar != slashToAppend)
                    return path + slashToAppend;
                else
                    return path;
            }
        }

        public static string RemoveExtension(string path)
        {
            int lastDotPos = path.LastIndexOf('.');
            int lastSlashPos = path.LastIndexOfAny(new[] { '/', '\\' });
            if (lastDotPos > lastSlashPos)
                return path.Substring(0, lastDotPos);
            else
                return path;
        }

        // todo: implement a more reliable one
        public static string Relativize(string path, string relativeTo)
        {
            if (string.IsNullOrEmpty(relativeTo))
                return path;
            else if (string.IsNullOrEmpty(path))
                return "";
            else
            {
                path = PathEx.NormalizeDirectorySeparators(path);
                relativeTo = PathEx.NormalizeDirectorySeparators(relativeTo);
                if (path.ToUpperInvariant().StartsWith(relativeTo.ToUpperInvariant()))
                    return "." + path.Substring(relativeTo.Length);
                else
                    return path;
            }
        }

        public static string NormalizeDirectorySeparators(string path, char slash)
        {
            if (path == null)
                return path;
            char otherSlash = slash == '\\' ? '/' : '\\';
            return path.Replace(otherSlash, slash);
        }

        public static string NormalizeDirectorySeparators(string path)
        {
            if (path == null)
                return path;

            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        public static string CreateTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            return tempDirectory;
        }

        public static bool Equals(string path1, string path2, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return string.Equals(NormalizeDirectorySeparators(path1), NormalizeDirectorySeparators(path2), comparisonType);
        }
    }
}
