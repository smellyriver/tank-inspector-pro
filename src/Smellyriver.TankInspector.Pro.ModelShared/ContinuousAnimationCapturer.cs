using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public abstract class ContinuousAnimationCapturer : GifAnimationCapturer
    {

        protected override double FrameTime { get { return 1.0 / (double)this.AnimationFramerate; } }

        public int AnimationFramerate
        {
            get
            {
                return ModelSharedSettings.Default.AnimationFramerate == -1
                     ? ModelSharedSettings.Default.DefaultAnimationFramerate
                     : ModelSharedSettings.Default.AnimationFramerate;
            }
            set
            {
                ModelSharedSettings.Default.AnimationFramerate = value.Clamp(this.MinimumAnimationFramerate, this.MaximumAnimationFramerate);
                ModelSharedSettings.Default.Save();
                this.RaisePropertyChanged(() => this.AnimationFramerate);
            }
        }

        public int MinimumAnimationFramerate
        {
            get { return ModelSharedSettings.Default.MinimumAnimationFramerate; }
        }

        public int MaximumAnimationFramerate
        {
            get { return ModelSharedSettings.Default.MaximumAnimationFramerate; }
        }



        protected override bool IsBackgroundTransparent { get { return true; } }


        public ContinuousAnimationCapturer(CaptureVMBase owner)
            : base(owner)
        {

        }
    }
}
