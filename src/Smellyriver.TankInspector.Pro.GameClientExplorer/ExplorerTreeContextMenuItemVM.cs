using System.Windows.Input;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    class ExplorerTreeContextMenuItemVM : MenuItemVM
    {
        public int Order { get; private set; }

        public bool IsDefault { get; set; }

        public ExplorerTreeContextMenuItemVM(int order, string name, ICommand command, object commandParameter = null, ImageSource iconSource = null)
            : base(name, command, commandParameter)
        {
            this.Order = order;
            this.Icon = iconSource;
        }
    }
}
