using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Collections;
using Smellyriver.TankInspector.Common.Wpf.Converters;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Menus;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    class MainMenuVM : NotificationObject
    {
        public ShellVM Shell { get; private set; }

        private bool _hasActiveModel;
        public bool HasActiveModel
        {
            get { return _hasActiveModel; }
            set
            {
                _hasActiveModel = value;
                this.RaisePropertyChanged(() => this.HasActiveModel);
            }
        }

        public ViewModelMap<MenuItemVM, MenuItem> NewMenuItems { get; }
        public bool HasNewMenuItems { get { return this.NewMenuItems.Any(m => m.Visibility == Visibility.Visible); } }
        public ViewModelMap<MenuItemVM, MenuItem> ExportMenuItems { get; }
        public bool HasExportMenuItems { get { return this.ExportMenuItems.Any(m => m.Visibility == Visibility.Visible); } }
        public ViewModelMap<MenuItemVM, MenuItem> OpenMenuItems { get; }
        public bool HasOpenMenuItems { get { return this.OpenMenuItems.Any(m => m.Visibility == Visibility.Visible); } }
        public ViewModelMap<MenuItemVM, MenuItem> ViewMenuItems { get; }
        public bool HasViewMenuItems { get { return this.ViewMenuItems.Any(m => m.Visibility == Visibility.Visible); } }
        public ViewModelMap<MenuItemVM, MenuItem> HelpMenuItems { get; }
        public bool HasHelpMenuItems { get { return this.HelpMenuItems.Any(m => m.Visibility == Visibility.Visible); } }
        public ViewModelMap<MenuItemVM, MenuItem> RootMenuItems { get; }
        public bool HasRootMenuItems { get { return this.RootMenuItems.Any(m => m.Visibility == Visibility.Visible); } }
        public ViewModelMap<MenuItemVM, MenuItem> ToolsMenuItems { get; }
        public bool HasToolsMenuItems { get { return this.ToolsMenuItems.Any(m => m.Visibility == Visibility.Visible); } }
        public ObservableCollection<MenuItemVM> RecentDocuments { get; }
        public bool HasRecentDocuments { get { return this.RecentDocuments.Count > 0; } }

        public MainMenuVM(ShellVM shell)
        {
            this.Shell = shell;

            this.NewMenuItems = this.CreateViewModelMap(MenuAnchor.New, () => this.HasNewMenuItems);
            this.ExportMenuItems = this.CreateViewModelMap(MenuAnchor.Export, () => this.HasExportMenuItems);
            this.OpenMenuItems = this.CreateViewModelMap(MenuAnchor.Open, () => this.HasOpenMenuItems);
            this.ViewMenuItems = this.CreateViewModelMap(MenuAnchor.View, () => this.HasViewMenuItems);
            this.HelpMenuItems = this.CreateViewModelMap(MenuAnchor.Help, () => this.HasHelpMenuItems);
            this.RootMenuItems = this.CreateViewModelMap(MenuAnchor.Root, () => this.HasRootMenuItems);
            this.ToolsMenuItems = this.CreateViewModelMap(MenuAnchor.Tools, () => this.HasToolsMenuItems);

            this.RecentDocuments = new ObservableCollection<MenuItemVM>();

            DockingViewManager.Instance.DocumentManager.ActiveDocumentChanged += DocumentManager_ActiveDocumentChanged;
        }

        void DocumentManager_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            var mi = this.ExportMenuItems.FirstOrDefault();
        }

        private ViewModelMap<MenuItemVM, MenuItem> CreateViewModelMap(MenuAnchor anchor, Expression<Func<bool>> hasItemsProperty)
        {
            var map = new ViewModelMap<MenuItemVM, MenuItem>(MenuManager.Instance.GetMenuItems(anchor), vm => this.CreateMenuItem(vm, hasItemsProperty));
            map.CollectionChanged += (o, e) => this.RaisePropertyChanged(hasItemsProperty);
            
            return map;
        }

        private MenuItem CreateMenuItem(MenuItemVM vm, Expression<Func<bool>> hasItemsProperty)
        {
            var menuItem = new MenuItem();
            menuItem.SetBinding(HeaderedItemsControl.HeaderProperty, "Name");
            menuItem.SetBinding(MenuItem.CommandParameterProperty, "CommandParameter");
            menuItem.SetBinding(MenuItem.CommandProperty, "Command");
            menuItem.SetBinding(MenuItem.IsCheckableProperty, "IsCheckable");
            menuItem.SetBinding(MenuItem.IsCheckedProperty, "IsChecked");
            var image = new Image();
            image.Stretch = Stretch.None;
            image.SetBinding(Image.SourceProperty, "Icon");
            menuItem.Icon = image;
            menuItem.DataContext = vm;

            var visibilityBinding = new Binding("IsEnabled")
            {
                Converter = new BoolToVisibilityConverter(),
                Source = menuItem
            };

            BindingOperations.SetBinding(menuItem, UIElement.VisibilityProperty, visibilityBinding);

            menuItem.AddPropertyChangedHandler(UIElement.VisibilityProperty, (o, e) =>
                {
                    this.RaisePropertyChanged(hasItemsProperty);
                });
            
            return menuItem;
        }
    }
}
