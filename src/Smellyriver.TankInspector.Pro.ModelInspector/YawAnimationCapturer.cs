using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Smellyriver.TankInspector.Pro.Graphics.Scene;
using Smellyriver.TankInspector.Pro.ModelShared;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    class YawAnimationCapturer : ContinuousAnimationCapturer
    {
        public override string Name
        {
            get { return this.L("model_inspector", "capture_mode_yaw_animation"); }
        }

        public override string Description
        {
            get { return this.L("model_inspector", "capture_mode_yaw_animation_description"); }
        }

        protected override string CaptureDialogTitle
        {
            get { return this.L("model_inspector", "capturing_yaw_animation_title"); }
        }

        protected override string CaptureDialogMessage
        {
            get { return this.L("model_inspector", "capturing_yaw_animation_message"); }
        }

        protected override string CaptureCompletedDialogTitle
        {
            get { return this.L("model_inspector", "capture_yaw_animation_completed_title"); }
        }

        protected override string CaptureCompletedDialogMessage
        {
            get { return this.L("model_inspector", "capture_yaw_animation_completed_message"); }
        }


        public double RotationSpeed
        {
            get { return ModelInspectorSettings.Default.RotationSpeed; }
            set
            {
                ModelInspectorSettings.Default.RotationSpeed = value.Clamp(this.MinimumRotationSpeed, this.MaximumRotationSpeed);
                ModelInspectorSettings.Default.Save();
                this.RaisePropertyChanged(() => this.RotationSpeed);
            }
        }

        public double MaximumRotationSpeed
        {
            get { return ModelInspectorSettings.Default.MaximumRotationSpeed; }
        }

        public double MinimumRotationSpeed
        {
            get { return ModelInspectorSettings.Default.MinimumRotationSpeed; }
        }

        private double _originalRotationYaw;

        public YawAnimationCapturer(CaptureVM owner)
            : base(owner)
        {

        }

        protected override BitmapSource[] CaptureFrames(ISnapshotProvider snapshotProvider, Rect clippingRectangle, Color shadeColor, IProgressScope progress, Func<bool> getIsCancelled)
        {
            return snapshotProvider.YawAnimationSnapshot(clippingRectangle,
                                                         this.SampleFactor,
                                                         shadeColor,
                                                         this.RotationSpeed,
                                                         this.AnimationFramerate,
                                                         progress,
                                                         getIsCancelled);
        }

        public override void BeginPreview()
        {
            _originalRotationYaw = this.Owner.Owner.Camera.RotationYaw;
        }

        protected override void UpdatePreview(double dt)
        {
            this.Owner.Owner.Camera.RotationYaw += this.RotationSpeed * dt;
        }

        public override void EndPreview()
        {
            this.Owner.Owner.Camera.RotationYaw = _originalRotationYaw;
        }
    }
}
