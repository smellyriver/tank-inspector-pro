using System.Collections.ObjectModel;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity
{
    partial class DockingViewManager
    {
        private class PanelManagerImpl : IPanelManager
        {

            private readonly ObservableCollection<PanelInfo> _panels;
            public ReadOnlyObservableCollection<PanelInfo> Panels { get; }

            public PanelManagerImpl()
            {
                _panels = new ObservableCollection<PanelInfo>();
                this.Panels = new ReadOnlyObservableCollection<PanelInfo>(_panels);
            }

            public void Register(PanelInfo panel)
            {
                _panels.Add(panel);
            }
        }
    }
}
