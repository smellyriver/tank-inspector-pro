using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    abstract class ExplorerTreeNodeVM : TreeNodeVM
    {
        private Lazy<ContextMenuVM> _lazyContextMenu;
        public ContextMenuVM ContextMenu { get { return _lazyContextMenu.Value; } }

        public virtual LocalGameClientNodeVM GameClientRoot
        {
            get { return ((ExplorerTreeNodeVM)this.Parent).GameClientRoot; }
        }

        public virtual ImageSource IconSource { get { return null; } }

        public virtual bool IsInPackage { get { return false; } }

        public virtual ICommand DefaultCommand { get; protected set; }

        public virtual string Description
        {
            get { return string.Empty; }
        }


        public ExplorerTreeNodeVM(ExplorerTreeNodeVM parent, string name, LoadChildenStrategy loadChildrenStrategy)
            : base(parent, name, loadChildrenStrategy)
        {
            _lazyContextMenu = new Lazy<ContextMenuVM>(this.CreateContextMenu);
        }


        protected virtual ContextMenuVM CreateContextMenu()
        {
            var contextMenu = new ContextMenuVM();

            foreach (var menuItem in this.CreateContextMenuItems().OrderBy(i => i.Order))
                contextMenu.MenuItems.Add(menuItem);

            foreach (var menuItem in contextMenu.MenuItems.Cast<ExplorerTreeContextMenuItemVM>())
                menuItem.IsDefault = this.DefaultCommand != null && menuItem.Command == this.DefaultCommand;

            return contextMenu;
        }

        protected virtual List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            return new List<ExplorerTreeContextMenuItemVM>();
        }
    }
}
