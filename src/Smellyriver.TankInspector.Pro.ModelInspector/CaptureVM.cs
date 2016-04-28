using Smellyriver.TankInspector.Pro.ModelShared;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    class CaptureVM : CaptureVMBase
    {
        public bool IsYawAnimationCapturerSelected { get { return this.SelectedCapturer is YawAnimationCapturer; } }
        public bool IsVisualModelComparerCapturerSelected { get { return this.SelectedCapturer is VisualModelComparerCapturer; } }


        public override Capturer SelectedCapturer
        {
            get { return base.SelectedCapturer; }
            set
            {
                base.SelectedCapturer = value;
                this.RaisePropertyChanged(() => this.IsYawAnimationCapturerSelected);
                this.RaisePropertyChanged(() => this.IsVisualModelComparerCapturerSelected);
            }
        }

        protected override Capturer[] GetCapturers()
        {
            return new Capturer[]
                {
                    new StaticCapturer(this),
                    new YawAnimationCapturer(this),
                    new VisualModelComparerCapturer(this)
                };
        }

        public CaptureVM(ModelDocumentVM owner)
            : base(owner)
        {

        }

    }
}
