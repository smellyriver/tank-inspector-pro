using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.ArmorInspector
{
    class LegendVM : NotificationObject
    {
        public Brush ArmorBrush { get; private set; }
        public Brush TextBrush { get; private set; }

        private bool _isMouseOver;
        public bool IsMouseOver
        {
            get { return _isMouseOver; }
            set
            {
                _isMouseOver = value;
                this.RaisePropertyChanged(() => this.IsMouseOver);
            }
        }

        public double ArmorValue { get; private set; }

        public LegendVM(double armorValue, Color armorColor)
        {
            this.ArmorBrush = new SolidColorBrush(armorColor);
            this.ArmorValue = armorValue;
            this.TextBrush = armorColor.GetLuminance() > 0.5 ? Brushes.Black : Brushes.White;
        }
    }
}
