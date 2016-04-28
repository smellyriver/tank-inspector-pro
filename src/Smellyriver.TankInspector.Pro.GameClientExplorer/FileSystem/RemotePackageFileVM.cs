using System.Collections.Generic;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    class RemotePackageFileVM : PackageFileVM
    {
        public RemotePackageFileVM(ExplorerTreeNodeVM parent, string packageFile, string localPath)
            : base(parent, packageFile, localPath)
        {

        }

        public RemotePackageFileVM(ExplorerTreeNodeVM parent, string packageFile, string localPath, string name)
            : base(parent, packageFile, localPath, name)
        {

        }

        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();

            list.Add(new ExplorerTreeContextMenuItemVM(150, 
                                                       this.L("game_client_explorer", "locate_in_files_menu_item"), 
                                                       new RelayCommand(this.LocateInFiles),
                                                       BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/Locate_16.png")));

            return list;
        }

        private void LocateInFiles()
        {
            this.GameClientRoot.FilesNode.ExpandToFile(this.Path);
        }
    }
}
