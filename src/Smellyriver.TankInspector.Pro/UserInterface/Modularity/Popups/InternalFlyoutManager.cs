using System.Collections.ObjectModel;
using MetroFlyout = MahApps.Metro.Controls.Flyout;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    internal class InternalFlyoutManager : FlyoutManager
    {
        public ObservableCollection<MetroFlyout> Flyouts
        {
            get { return this.InternalFlyouts; }
        }
    }
}
