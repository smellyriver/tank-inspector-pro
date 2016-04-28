using Smellyriver.TankInspector.Pro.ModelShared;

namespace Smellyriver.TankInspector.Pro.ArmorInspector
{
    class ArmorComparerCapturer : ModelComparerCapturer
    {
        public override string Name
        {
            get { return this.L("armor_inspector", "capture_mode_armor_change"); }
        }

        public override string Description
        {
            get { return this.L("armor_inspector", "capture_mode_armor_change_description"); }
        }

        protected override string CaptureDialogTitle
        {
            get { return this.L("armor_inspector", "capturing_armor_change_title"); }
        }

        protected override string CaptureDialogMessage
        {
            get { return this.L("armor_inspector", "capturing_armor_change_message"); }
        }

        protected override string CaptureCompletedDialogTitle
        {
            get { return this.L("armor_inspector", "capture_armor_change_completed_title"); }
        }

        protected override string CaptureCompletedDialogMessage
        {
            get { return this.L("armor_inspector", "capture_armor_change_completed_message"); }
        }

        public ArmorComparerCapturer(CaptureVM owner)
            : base(owner)
        {

        }

    }
}
