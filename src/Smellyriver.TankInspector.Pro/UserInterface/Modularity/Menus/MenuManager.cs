using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Menus
{
    public class MenuManager
    {
        public static MenuManager Instance { get; private set; }

        static MenuManager()
        {
            MenuManager.Instance = new MenuManager();
        }

        private readonly Dictionary<MenuAnchor, ObservableCollection<MenuItemVM>> _menuItems;
        private readonly Dictionary<MenuAnchor, Dictionary<MenuItemVM, int>> _menuItemPriorities;

        private MenuManager()
        {
            _menuItemPriorities = new Dictionary<MenuAnchor, Dictionary<MenuItemVM, int>>();
            _menuItems = new Dictionary<MenuAnchor, ObservableCollection<MenuItemVM>>();
            foreach (MenuAnchor anchor in Enum.GetValues(typeof(MenuAnchor)))
            {
                _menuItems[anchor] = new ObservableCollection<MenuItemVM>();
                _menuItemPriorities[anchor] = new Dictionary<MenuItemVM, int>();
            }
        }

        internal ObservableCollection<MenuItemVM> GetMenuItems(MenuAnchor anchor)
        {
            return _menuItems[anchor];
        }

        public void Register(MenuItemVM menuItem, MenuAnchor anchor, int priority = 0)
        {
            var prioritiyLookup = _menuItemPriorities[anchor];
            var menuItems = _menuItems[anchor];

            var inserted = false;

            for (var i = 0; i < menuItems.Count;++i )
            {
                if(prioritiyLookup[menuItems[i]] > priority)
                {
                    menuItems.Insert(i, menuItem);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
                menuItems.Add(menuItem);

            prioritiyLookup[menuItem] = priority;
        }

        public void Remove(MenuItemVM menuItem, MenuAnchor anchor)
        {
            var menuItems = _menuItems[anchor];
            if(!menuItems.Remove(menuItem))
                this.LogError("failed to remove menu item '{0}' at '{1}'", menuItem.Name, anchor);

            _menuItemPriorities[anchor].Remove(menuItem);
        }
    }
}
