using Smellyriver.TankInspector.Pro.ModelShared;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    class StaticCapturer : StaticCapturerBase
    {

        public override string Name
        {
            get { return this.L("model_inspector", "capture_mode_static"); }
        }

        public override string Description
        {
            get { return this.L("model_inspector", "capture_mode_static_description"); }
        }

        protected override string CapturingDialogTitle
        {
            get { return this.L("model_inspector", "capturing_static_picture_title"); }
        }

        protected override string CapturingDialogMessage
        {
            get { return this.L("model_inspector", "capturing_static_picture_message"); }
        }

        protected override string CaptureCompletedDialogTitle
        {
            get { return this.L("model_inspector", "capture_static_picture_completed_title"); }
        }

        protected override string CaptureCompletedDialogMessage
        {
            get { return this.L("model_inspector", "capture_static_picture_completed_message"); }
        }


        public StaticCapturer(CaptureVM owner)
            : base(owner)
        {

        }

    }
}
