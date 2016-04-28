using Smellyriver.TankInspector.Pro.ModelShared;

namespace Smellyriver.TankInspector.Pro.ArmorInspector
{
    class CaptureVM : CaptureVMBase
    {
        public bool IsVisualModelComparerCapturerSelected { get { return this.SelectedCapturer is ArmorComparerCapturer; } }


        public override Capturer SelectedCapturer
        {
            get { return base.SelectedCapturer; }
            set
            {
                base.SelectedCapturer = value;
                this.RaisePropertyChanged(() => this.IsVisualModelComparerCapturerSelected);
            }
        }

        protected override Capturer[] GetCapturers()
        {
            return new Capturer[]
                {
                    new StaticCapturer(this),
                    new ArmorComparerCapturer(this)
                };
        }

        public CaptureVM(ArmorDocumentVM owner)
            : base(owner)
        {

        }

    }
}
