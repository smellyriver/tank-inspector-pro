using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.SharpZipLib.Zip;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using IOPath = System.IO.Path;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    class PackageFolderVM : FolderVM, IPackageFileSystemObjectVM, IPackageFolderVM
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

        private readonly List<ZipEntry> _childEntries;

        ObservableCollection<TreeNodeVM> IPackageFolderVM.Children
        {
            get { return this.InternalChildren; }
        }

        public PackageFolderVM(ExplorerTreeNodeVM parent, string packageFile, string localPath)
            : base(parent, UnifiedPath.Combine(packageFile, localPath), UnifiedPath.GetFileName(localPath))
        {
            this.LocalPath = localPath;
            this.PackagePath = packageFile;

            _childEntries = new List<ZipEntry>();
        }

        internal void AddChild(ZipEntry entry)
        {
            _childEntries.Add(entry);
        }

        protected override IEnumerable<TreeNodeVM> LoadChildren()
        {
            return this.GenerateChildren(_childEntries);
        }

        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();

            list.Add(new ExplorerTreeContextMenuItemVM(100,
                                                       this.L("game_client_explorer", "copy_to_mod_folder_menu_item"), 
                                                       new RelayCommand(() => this.CopyToModFolder()),
                                                       BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/Copy_16.png")));

            return list;
        }



    }
}
