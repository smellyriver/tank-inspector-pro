using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using IOPath = System.IO.Path;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    class FileVM : FileSystemObjectVM
    {

        public string ExtensionName
        {
            get { return UnifiedPath.GetExtension(this.Path).Substring(1).ToLower(); }
        }

        public override ImageSource IconSource
        {
            get
            {
                var nodeType = string.Format("{0}{1}", NodeTypes.FileProtocal, this.ExtensionName);
                return NodeIconService.Current.GetNodeIcon(nodeType);
            }
        }

        public FileVM(ExplorerTreeNodeVM parent, string path)
            : this(parent, path, UnifiedPath.GetFileName(path))
        {

        }

        public FileVM(ExplorerTreeNodeVM parent, string path, string name)
            : base(parent, path, name, LoadChildenStrategy.Manual)
        {
            this.InitializeCommands();
        }

        private void InitializeCommands()
        {
            if (FileDocumentService.Instance.HasFileViewerService(this.ExtensionName))
                this.DefaultCommand = new RelayCommand(this.ViewFile);
            else
                this.DefaultCommand = new RelayCommand(this.PromptAndOpenWithDefaultProgram);
        }

        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();

            if (!this.IsInPackage)
            {
                list.Add(new ExplorerTreeContextMenuItemVM(-100, 
                                                           this.L("game_client_explorer", "open_menu_item"),
                                                           new RelayCommand(this.OpenFileWithDefaultProgram),
                                                           BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/OpenExternal_16.png")));
            }

            if (FileDocumentService.Instance.HasFileViewerService(this.ExtensionName))
            {
                list.Add(new ExplorerTreeContextMenuItemVM(-1000, 
                                                           this.L("game_client_explorer", "view_menu_item"), 
                                                           this.DefaultCommand,
                                                           BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/OpenInternal_16.png")));
            }

            return list;
        }

        private void PromptAndOpenWithDefaultProgram()
        {

            DialogManager.Instance.ShowYesNoMessageAsync(
                this.L("game_client_explorer", "open_with_default_program_message_title"),
                this.L("game_client_explorer", "open_with_default_program_message"))
                .ContinueWith(t =>
                              {
                                  if (t.Result == MessageDialogResult.Affirmative)
                                      this.OpenFileWithDefaultProgram();
                              });
        }

        private void ViewFile()
        {
            var uri = new Uri(this.Path);
            DockingViewManager.Instance.DocumentManager.OpenDocument(uri);
        }

        private void OpenFileWithDefaultProgram()
        {
            if (!File.Exists(this.Path) && !Directory.Exists(this.Path))
                return;
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.Path;
            processStartInfo.UseShellExecute = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(processStartInfo);
        }
    }
}
