using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Graphics.Scene;
using Smellyriver.TankInspector.Pro.ModelShared;

namespace Smellyriver.TankInspector.Pro.ArmorInspector
{
    class ArmorViewVM : ModelVMBase
    {

        public static readonly GradientStopCollection DefaultRegularArmorSpectrumStops;
        public static readonly GradientStopCollection DefaultSpacingArmorSpectrumStops;

        static ArmorViewVM()
        {
            DefaultRegularArmorSpectrumStops = new GradientStopCollection
            {
                new GradientStop(Color.FromArgb(0xff, 0xc0, 0, 0) , 0.0),
                new GradientStop(Color.FromArgb(0xff, 0xc0, 0x30, 0), 0.33),
                new GradientStop(Color.FromArgb(0xff, 0xc0, 0xc0, 0), 0.67),
                new GradientStop(Color.FromArgb(0xff, 0, 0xc0, 0), 1.0),
            };

            DefaultRegularArmorSpectrumStops.Freeze();

            DefaultSpacingArmorSpectrumStops = new GradientStopCollection
            {
                new GradientStop(Color.FromArgb(0xff, 0xc0, 0, 0xc0), 0.0),
                new GradientStop(Color.FromArgb(0xff, 0x30, 0, 0xc0), 0.33),
                new GradientStop(Color.FromArgb(0xff, 0, 0, 0xc0), 0.67),
                new GradientStop(Color.FromArgb(0xff, 0, 0xc0, 0xc0), 1.0),
            };

            DefaultSpacingArmorSpectrumStops.Freeze();
        }

        private CollisionModelRenderParameters _collisionModelRenderParameters;
        public CollisionModelRenderParameters CollisionModelRenderParameters
        {
            get { return _collisionModelRenderParameters; }
            private set
            {
                _collisionModelRenderParameters = value;
                this.RaisePropertyChanged(() => this.CollisionModelRenderParameters);
            }
        }

        public LegendVM[] Legends { get; private set; }

        public DoubleCollection RegularArmorValues { get; private set; }
        public DoubleCollection SpacedArmorValues { get; private set; }

        public ArmorViewVM(TankInstance tankInstance)
            : base(tankInstance)
        {
            this.RegularArmorValues = new DoubleCollection(tankInstance.GetArmorValues(false));
            this.SpacedArmorValues = new DoubleCollection(tankInstance.GetArmorValues(true));

            var thickestArmor = this.TankInstance.GetThickestArmor(false);
            var thinnestArmor = this.TankInstance.GetThinnestArmor(false);
            var thickestSpacedArmor = this.TankInstance.GetThickestArmor(true);
            var thinnestSpacedArmor = this.TankInstance.GetThinnestArmor(true);

            this.CollisionModelRenderParameters = new CollisionModelRenderParameters()
            {
                TankThickestArmor = thickestArmor,
                TankThinnestArmor = thinnestArmor,
                TankThickestSpacingArmor = thickestSpacedArmor,
                TankThinnestSpacingArmor = thinnestSpacedArmor,
                RegularArmorSpectrumStops = DefaultRegularArmorSpectrumStops,
                SpacingArmorSpectrumStops = DefaultSpacingArmorSpectrumStops,
                //TBD
                RegularArmorValueSelectionBegin = thinnestArmor,
                RegularArmorValueSelectionEnd = thickestArmor,
                SpacingArmorValueSelectionBegin = thinnestSpacedArmor,
                SpacingArmorValueSelectionEnd = thickestSpacedArmor,
            };


        }
    }
}
