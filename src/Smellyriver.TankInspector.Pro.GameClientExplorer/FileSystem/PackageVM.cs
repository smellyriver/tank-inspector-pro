using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.SharpZipLib.Zip;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;
using IOPath = System.IO.Path;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    class PackageVM : FolderVM, IPackageFolderVM
    {

        ObservableCollection<TreeNodeVM> IPackageFolderVM.Children
        {
            get { return this.InternalChildren; }
        }

        string IPackageFileSystemObjectVM.PackagePath
        {
            get { return this.Path; }
        }

        string IPackageFileSystemObjectVM.LocalPath
        {
            get { return string.Empty; }
        }


        public override ImageSource IconSource
        {
            get { return NodeIconService.Current.GetNodeIcon("file:pkg"); }
        }

        public PackageVM(ExplorerTreeNodeVM parent, string path)
            : base(parent, path)
        {

        }

        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();

            list.Add(new ExplorerTreeContextMenuItemVM(100,
                                                       this.L("game_client_explorer", "extract_to_mod_folder_menu_item"), 
                                                       new RelayCommand(() => this.CopyToModFolder())));

            return list;
        }


        protected override IEnumerable<TreeNodeVM> LoadChildren()
        {
            using (var stream = File.OpenRead(this.Path))
            {
                using (var zipFile = new ZipFile(stream))
                {
                    var entries = zipFile.Cast<ZipEntry>();
                    return this.GenerateChildren(entries);
                }
            }
        }

    }
}
