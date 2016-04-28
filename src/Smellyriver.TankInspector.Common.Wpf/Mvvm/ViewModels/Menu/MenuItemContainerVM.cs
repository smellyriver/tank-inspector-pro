using System.Collections.ObjectModel;

namespace Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu
{
    public abstract class MenuItemContainerVM : MenuItemVMBase
    {
        public ObservableCollection<MenuItemVM> MenuItems { get; private set; }

        public MenuItemContainerVM()
        {
            MenuItems = new ObservableCollection<MenuItemVM>();
        }
    }
}
