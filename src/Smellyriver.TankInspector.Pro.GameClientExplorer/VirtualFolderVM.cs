using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    class VirtualFolderVM : ExplorerTreeNodeVM, IAddChild
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

            }
        }

        public VirtualFolderVM(ExplorerTreeNodeVM parent, string name)
            : base(parent, name, LoadChildenStrategy.Manual)
        {
            this.UpdateIconSource();
        }

        public void AddChild(ExplorerTreeNodeVM child)
        {
            this.InternalChildren.Add(child);
        }

        private void UpdateIconSource()
        {
            this.GameClientRoot.FileExplorer.Dispatcher.AutoInvoke(() =>
            {
                _iconSource = NodeIconService.Current.GetNodeIcon(NodeTypes.Folder, this.IsExpanded);
                this.RaisePropertyChanged(() => this.IconSource);
            });
        }
    }
}
