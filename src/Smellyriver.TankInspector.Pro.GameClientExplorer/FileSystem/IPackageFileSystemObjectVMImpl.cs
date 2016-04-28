using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.IO;
using IOPath = System.IO.Path;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    static class IPackageFileSystemObjectVMImpl
    {
        public static string GetDescription(this IPackageFileSystemObjectVM @this)
        {
            return Localization.Instance.L("game_client_explorer", 
                                           "package_file_description",
                                           @this.LocalPath, 
                                           @this.PackagePath);
        }

        public static void CopyToModFolder<T>(this T @this)
            where T : FileSystemObjectVM, IPackageFileSystemObjectVM
        {
            var modDirectory = @this.GameClientRoot.Model.ModDirectory;
            PackageManager.Instance.Extract(@this.PackagePath, @this.LocalPath, modDirectory, OverwriteStrategy.Ask, false);
        }
    }
}
