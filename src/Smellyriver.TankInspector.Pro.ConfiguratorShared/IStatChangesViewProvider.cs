using System.Windows;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.ConfiguratorShared
{
    public interface IStatChangesViewProvider
    {
        FrameworkElement CreateStatChangesView(TankInstance before, TankInstance after);
    }
}
