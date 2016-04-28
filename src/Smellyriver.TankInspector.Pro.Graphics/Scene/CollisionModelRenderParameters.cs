using System;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    public sealed class CollisionModelRenderParameters
    {
        internal event EventHandler ArmorColorMapInvalidated;

        private GradientStopCollection _regularArmorSpectrumStops;
        public GradientStopCollection RegularArmorSpectrumStops
        {
            get { return _regularArmorSpectrumStops; }
            set
            {
                _regularArmorSpectrumStops = value;
                this.InvalidateArmorColorMap();
            }
        }

        private GradientStopCollection _spacingArmorSpectrumStops;
        public GradientStopCollection SpacingArmorSpectrumStops
        {
            get { return _spacingArmorSpectrumStops; }
            set
            {
                _spacingArmorSpectrumStops = value;
                this.InvalidateArmorColorMap();
            }
        }
        public double RegularArmorValueSelectionBegin { get; set; }
        public double RegularArmorValueSelectionEnd { get; set; }
        public double SpacingArmorValueSelectionBegin { get; set; }
        public double SpacingArmorValueSelectionEnd { get; set; }
        public double TankThickestArmor { get; set; }
        public double TankThinnestArmor { get; set; }
        public double TankThickestSpacingArmor { get; set; }
        public double TankThinnestSpacingArmor { get; set; }
        public bool HasRegularArmorHintValue { get; set; }
        public bool HasSpacingArmorHintValue { get; set; }
        public double RegularArmorHintValue { get; set; }
        public double SpacingArmorHintValue { get; set; }

        private void InvalidateArmorColorMap()
        {
            if (this.ArmorColorMapInvalidated != null)
                this.ArmorColorMapInvalidated(this, EventArgs.Empty);
        }
    }
}
