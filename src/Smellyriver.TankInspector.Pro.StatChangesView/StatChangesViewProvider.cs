using System.ComponentModel.Composition;
using System.Windows;
using Smellyriver.TankInspector.Pro.ConfiguratorShared;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.StatChangesView
{
    [Export(typeof(IStatChangesViewProvider))]
    public class StatChangesViewProvider : IStatChangesViewProvider
    {
        public FrameworkElement CreateStatChangesView(TankInstance before, TankInstance after)
        {
            var vm = new StatChangesVM(before, after);
            var view = new StatChangesView();
            view.ViewModel = vm;
            return view;
        }
    }
}
