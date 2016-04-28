using System;
using System.Collections.ObjectModel;
using System.Windows;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Common.Collections;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using MetroFlyout = MahApps.Metro.Controls.Flyout;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    public abstract class FlyoutManager
    {
        public static FlyoutManager Instance { get; private set; }

        static FlyoutManager()
        {
            FlyoutManager.Instance = new InternalFlyoutManager();
        }

        private readonly BidirectionalDictionary<Flyout, MetroFlyout> _flyoutsMap;
        protected ObservableCollection<MetroFlyout> InternalFlyouts { get; }

        internal FlyoutManager()
        {
            _flyoutsMap = new BidirectionalDictionary<Flyout, MetroFlyout>();
            this.InternalFlyouts = new ObservableCollection<MetroFlyout>();
        }

        public void Open(Flyout flyout)
        {
            if (_flyoutsMap.ContainsKey(flyout))
                return;

            var metroFlyout = flyout.ToMetroFlyout();

            metroFlyout.Loaded += metroFlyout_Loaded;
            metroFlyout.AddPropertyChangedHandler(UIElement.VisibilityProperty, metroFlyout_VisibilityChanged);

            _flyoutsMap.Add(flyout, metroFlyout);
            this.InternalFlyouts.Add(metroFlyout);

        }

        private void metroFlyout_VisibilityChanged(object sender, EventArgs e)
        {
            var metroFlyout = (MetroFlyout)sender;
            if (!metroFlyout.IsLoaded)
                return;

            if(metroFlyout.Visibility == Visibility.Hidden)
                this.RemoveFlyout(metroFlyout);
        }

        void metroFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            var metroFlyout = (MetroFlyout)sender;
            metroFlyout.IsOpen = true;

            metroFlyout.Loaded -= metroFlyout_Loaded;
        }

        public void Close(Flyout flyout)
        {
            if (!_flyoutsMap.ContainsKey(flyout))
                return;

            _flyoutsMap[flyout].IsOpen = false;
        }

        private void RemoveFlyout(MetroFlyout metroFlyout)
        {
            metroFlyout.RemovePropertyChangedHandler(UIElement.VisibilityProperty, metroFlyout_VisibilityChanged);

            _flyoutsMap.RemoveValue(metroFlyout);
            this.InternalFlyouts.Remove(metroFlyout);
        }
    }
}
