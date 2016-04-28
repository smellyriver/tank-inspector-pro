using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using IOPath = System.IO.Path;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    abstract class FileSystemObjectVM : ExplorerTreeNodeVM
    {
        public string Path { get; }

        public override string Description
        {
            get { return this.Path; }
        }

        public FileSystemObjectVM(ExplorerTreeNodeVM parent, string path, string name, LoadChildenStrategy loadChildenStrategy)
            : base(parent, name, loadChildenStrategy)
        {
            this.Path = path == null ? null : PathEx.NormalizeDirectorySeparators(path);
        }
        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();

            var showInExplorerCommand = new RelayCommand(this.ShowInExplorer);
            this.DefaultCommand = showInExplorerCommand;

            if (!this.IsInPackage)
                list.Add(new ExplorerTreeContextMenuItemVM(0, 
                                                           this.L("game_client_explorer", "show_in_explorer_menu_item"), 
                                                           showInExplorerCommand,
                                                           BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/Explorer_16.png")));

            return list;
        }

        private void ShowInExplorer()
        {
            if (!File.Exists(this.Path) && !Directory.Exists(this.Path))
                return;
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "explorer";
            processStartInfo.UseShellExecute = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            processStartInfo.Arguments =
                string.Format("/e,/select,\"{0}\"", this.Path);
            Process.Start(processStartInfo);
        }
    }
}
