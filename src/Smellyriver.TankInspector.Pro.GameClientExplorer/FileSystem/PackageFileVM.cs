using System.Collections.Generic;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    class PackageFileVM : FileVM, IPackageFileSystemObjectVM
    {
        public string PackagePath { get; }
        public string LocalPath { get; }

        public override string Description
        {
            get { return this.GetDescription(); }
        }

        public override bool IsInPackage
        {
            get { return true; }
        }

        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();

            list.Add(new ExplorerTreeContextMenuItemVM(100, 
                                                       this.L("game_client_explorer", "copy_to_mod_folder_menu_item"), 
                                                       new RelayCommand(() => this.CopyToModFolder())));

            return list;
        }

        public PackageFileVM(ExplorerTreeNodeVM parent, string packageFile, string localPath)
            : base(parent, UnifiedPath.Combine(packageFile, localPath))
        {
            this.LocalPath = localPath;
            this.PackagePath = packageFile;
        }

        public PackageFileVM(ExplorerTreeNodeVM parent, string packageFile, string localPath, string name)
            : base(parent, UnifiedPath.Combine(packageFile, localPath), name)
        {
            this.LocalPath = localPath;
            this.PackagePath = packageFile;
        }
    }
}
