using System;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public class PanelInfo : LayoutContentInfo
    {

        public bool CanClose { get; set; }
        public bool CanHide { get; set; }
        public bool CanFloat { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double MinWidth { get; set; }
        public double MinHeight { get; set; }
        public bool DefaultlyShown { get; set; }
        public virtual Action<DocumentInfo> ActiveDocumentChangedCallback { get; set; }

        public PanelInfo(Guid guid)
            : base(guid)
        {
            this.CanClose = true;
            this.CanHide = true;
            this.CanFloat = true;
            this.Width = 200;
            this.Height = 200;
            this.DefaultlyShown = true;
        }
    }
}
