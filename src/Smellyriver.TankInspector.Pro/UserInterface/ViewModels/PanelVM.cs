using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    class PanelVM : LayoutContentVM
    {
        public PanelInfo Panel { get; }

        public bool CanClose { get { return this.Panel.CanClose; } }
        public bool CanHide { get { return this.Panel.CanHide; } }
        public bool CanFloat { get { return this.Panel.CanFloat; } }
        public double Width { get { return this.Panel.Width; } }
        public double Height { get { return this.Panel.Height; } }


        public PanelVM(PanelInfo panel)
            : base(panel)
        {
            this.Panel = panel;
            this.IsVisible = panel.DefaultlyShown;
        }

    }
}
