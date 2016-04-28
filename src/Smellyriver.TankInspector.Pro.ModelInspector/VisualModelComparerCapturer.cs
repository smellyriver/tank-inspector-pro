using Smellyriver.TankInspector.Pro.ModelShared;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    class VisualModelComparerCapturer : ModelComparerCapturer
    {
        public override string Name
        {
            get { return this.L("model_inspector", "capture_mode_visual_change"); }
        }

        public override string Description
        {
            get { return this.L("model_inspector", "capture_mode_visual_change_description"); }
        }

        protected override string CaptureDialogTitle
        {
            get { return this.L("model_inspector", "capturing_visual_change_title"); }
        }

        protected override string CaptureDialogMessage
        {
            get { return this.L("model_inspector", "capturing_visual_change_message"); }
        }

        protected override string CaptureCompletedDialogTitle
        {
            get { return this.L("model_inspector", "capture_visual_change_completed_title"); }
        }

        protected override string CaptureCompletedDialogMessage
        {
            get { return this.L("model_inspector", "capture_visual_change_completed_message"); }
        }

        public VisualModelComparerCapturer(CaptureVM owner)
            : base(owner)
        {

        }

    }
}
