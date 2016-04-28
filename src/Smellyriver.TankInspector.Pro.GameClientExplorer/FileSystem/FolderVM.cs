using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using IOPath = System.IO.Path;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    class FolderVM : FileSystemObjectVM
    {

        private ImageSource _iconSource;

        public override ImageSource IconSource
        {
            get { return _iconSource; }
        }

        public override bool IsExpanded
        {
            get { return base.IsExpanded; }
            set
            {
                if (base.IsExpanded == value)
                    return;

                base.IsExpanded = value;

                this.UpdateIconSource();

                if (_watcher != null)
                {
                    if (base.IsExpanded)
                        _watcher.EnableRaisingEvents = true;
                    else
                        _watcher.EnableRaisingEvents = false;
                }
            }
        }

        private readonly FileSystemWatcher _watcher;

        public FolderVM(ExplorerTreeNodeVM parent, string path)
            : this(parent, path, IOPath.GetFileName(path))
        {

        }



        public FolderVM(ExplorerTreeNodeVM parent, string path, string name)
            : base(parent, path, name, LoadChildenStrategy.LazyStaticAsync)
        {
            this.UpdateIconSource();

            if (!this.IsInPackage && Directory.Exists(this.Path))
            {
                _watcher = new FileSystemWatcher(this.Path);
                _watcher.Created += FileSystemWatcher_Changed;
                _watcher.Deleted += FileSystemWatcher_Changed;
                _watcher.Renamed += FileSystemWatcher_Renamed;
            }
        }

        private void UpdateIconSource()
        {
            this.Dispatcher.AutoInvoke(() =>
            {
                _iconSource = NodeIconService.Current.GetNodeIcon(NodeTypes.Folder, this.IsExpanded);
                this.RaisePropertyChanged(() => this.IconSource);
            });

        }

        private void InvokeReloadChildren()
        {
            this.Dispatcher.AutoInvoke(() => this.ReloadChildren());
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            this.InvokeReloadChildren();
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.InvokeReloadChildren();
        }

        protected override IEnumerable<TreeNodeVM> LoadChildren()
        {

            var children = new List<FileSystemObjectVM>();
            foreach (var directory in Directory.GetDirectories(this.Path))
            {
                children.Add(new FolderVM(this, directory));
            }

            var files = Directory.GetFiles(this.Path);

            foreach (var package in files.Where(f => IOPath.GetExtension(f).Equals(".pkg", StringComparison.InvariantCultureIgnoreCase)))
            {
                children.Add(new PackageVM(this, package));
            }

            foreach (var file in files.Where(f => !IOPath.GetExtension(f).Equals(".pkg", StringComparison.InvariantCultureIgnoreCase)))
            {
                children.Add(new FileVM(this, file));
            }

            return children;
        }
    }
}
