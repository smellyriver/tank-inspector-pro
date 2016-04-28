using System.Collections.Specialized;
using System.Windows;

namespace Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu
{
    public class ContextMenuVM : MenuItemContainerVM
    {

        private Visibility _visibility;
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                this.RaisePropertyChanged(() => this.Visibility);
            }
        }

        public ContextMenuVM()
        {
            this.Visibility = Visibility.Hidden;
            this.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
        }

        void MenuItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Visibility = this.MenuItems.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
