using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using Smellyriver.TankInspector.Common.Utilities;
using IOPath = System.IO.Path;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    static class IPackageFolderVMImpl
    {
        public static IEnumerable<FileSystemObjectVM> GenerateChildren<TThis>(this TThis @this, IEnumerable<ZipEntry> childEntries)
            where TThis : FileSystemObjectVM, IPackageFolderVM
        {

            var childFolders = new Dictionary<string, PackageFolderVM>();
            var childFiles = new Dictionary<string, PackageFileVM>();

            foreach (var childEntry in childEntries)
            {
                var childPath = PathEx.NormalizeDirectorySeparators(childEntry.Name);
                var lowerChildPath = childPath.ToLower();
                var relativePath = PathEx.Relativize(childPath, @this.LocalPath.Replace('/', '\\'));
                if (relativePath[0] == '.')
                    relativePath = relativePath.Substring(1);   // remove initial dot
                if (relativePath[0] == IOPath.DirectorySeparatorChar)
                    relativePath = relativePath.Substring(1);   // remove initial slash
                var firstSlashIndex = relativePath.IndexOf(IOPath.DirectorySeparatorChar);
                if (firstSlashIndex == -1)
                {
                    if (childEntry.IsDirectory)
                    {
                        if (!childFolders.ContainsKey(lowerChildPath))
                            childFolders.Add(lowerChildPath, new PackageFolderVM(@this, @this.PackagePath, childEntry.Name));
                    }
                    else
                    {
                        childFiles.Add(lowerChildPath, new PackageFileVM(@this, @this.PackagePath, childPath));
                    }
                }
                else
                {
                    var folderName = IOPath.Combine(@this.LocalPath, relativePath.Substring(0, firstSlashIndex)).Replace('\\', '/');
                    var lowerFolderName = folderName.ToLower();
                    var folder = childFolders.GetOrCreate(lowerFolderName, () => new PackageFolderVM(@this, @this.PackagePath, folderName));
                    folder.AddChild(childEntry);
                }
            }

            var children = new List<FileSystemObjectVM>();

            foreach (var childFolder in childFolders.Values)
                children.Add(childFolder);

            foreach (var childFile in childFiles.Values)
                children.Add(childFile);

            return children;
        }
    }
}
