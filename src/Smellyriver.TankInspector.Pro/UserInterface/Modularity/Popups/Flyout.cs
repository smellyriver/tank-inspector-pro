using System.Windows.Input;
using MahApps.Metro.Controls;
using MetroFlyout = MahApps.Metro.Controls.Flyout;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    public class Flyout
    {
        public string Header { get; set; }
        public FlyoutPosition Position { get; set; }
        public bool IsModal { get; set; }
        public ICommand CloseCommand { get; set; }
        public object Content { get; set; }

        public double Width { get; set; }
        public double MinWidth { get; set; }
        public double MaxWidth { get; set; }
        public double Height { get; set; }
        public double MinHeight { get; set; }
        public double MaxHeight { get; set; }

        public Flyout()
        {
            this.Width = this.Height = double.NaN;
            this.MinWidth = this.MinHeight = 0.0;
            this.MaxWidth = this.MaxHeight = double.PositiveInfinity;
        }

        internal MetroFlyout ToMetroFlyout()
        {
            return new MetroFlyout()
            {
                Header = this.Header,
                Position = (Position)this.Position,
                IsModal = this.IsModal,
                CloseCommand = this.CloseCommand,
                Content = Content,
                AnimateOnPositionChange = true,
                Theme = FlyoutTheme.Accent,
                Width = this.Width,
                MinWidth = this.MinWidth,
                MaxWidth = this.MaxWidth,
                Height = this.Height,
                MinHeight = this.MinHeight,
                MaxHeight = this.MaxHeight
            };
        }
    }
}
