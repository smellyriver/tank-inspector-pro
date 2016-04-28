using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Graphics.Scene;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public abstract class GifAnimationCapturer : Capturer
    {
        public override bool AllowPreview
        {
            get { return true; }
        }

        public override int MinimumOutputWidth
        {
            get { return ModelSharedSettings.Default.MinimumAnimationOutputWidth; }
        }

        public override int MinimumOutputHeight
        {
            get { return ModelSharedSettings.Default.MinimumAnimationOutputHeight; }
        }

        public override int MaximumOutputWidth
        {
            get { return ModelSharedSettings.Default.MaximumAnimationOutputWidth; }
        }

        public override int MaximumOutputHeight
        {
            get { return ModelSharedSettings.Default.MaximumAnimationOutputHeight; }
        }

        public override double SampleFactor
        {
            get
            {
                return ModelSharedSettings.Default.AnimationSampleFactor;
            }
            set
            {
                ModelSharedSettings.Default.AnimationSampleFactor = value;
                ModelSharedSettings.Default.Save();
                this.RaisePropertyChanged(() => this.SampleFactor);
            }
        }

        public override string OutputExtensionName
        {
            get { return ".gif"; }
        }

        public override string OutputFileFilter
        {
            get { return string.Format("{0} (*.gif)|*.gif", this.L("model_shared", "gif_file_type_filter")); }
        }

        protected DateTime PreviousFrameTime { get; private set; }

        protected abstract string CaptureDialogTitle { get; }
        protected abstract string CaptureDialogMessage { get; }
        protected abstract string CaptureCompletedDialogTitle { get; }
        protected abstract string CaptureCompletedDialogMessage { get; }
        protected abstract double FrameTime { get; }
        protected abstract bool IsBackgroundTransparent { get; }

        public GifAnimationCapturer(CaptureVMBase owner)
            : base(owner)
        {

        }

        public override void BeginPreview()
        {
            this.PreviousFrameTime = DateTime.Now.AddSeconds(-this.FrameTime);
        }

        protected abstract BitmapSource[] CaptureFrames(ISnapshotProvider snapshotProvider, Rect clippingRectangle, Color shadeColor, IProgressScope progress, Func<bool> getIsCancelled);


        public override void BeginCapture(ISnapshotProvider snapshotProvider, TankInstance tank, Rect clippingRectangle, Color shadeColor, string outputFilename)
        {
            DialogManager.Instance.ShowProgressAsync(this.CaptureDialogTitle,
                                                     string.Format(this.CaptureDialogMessage, tank.Name),
                                                     isCancellable: true)
                .ContinueWith(t =>
                {
                    Thread.Sleep(500);

                    DialogManager.AssignTask(
                        t.Result,
                        ActionTask.Create("ExportAnimationCapture",
                            progress =>
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {

                                    var takeSnapshotProgress = progress.AddChildScope("TakeSnapshot", 80);
                                    var encodeProgress = progress.AddChildScope("EncodeGIF", 20);

                                    var frameSources = this.CaptureFrames(snapshotProvider, clippingRectangle, shadeColor, takeSnapshotProgress, () => t.Result.IsCanceled);

                                    if (t.Result.IsCanceled)
                                        return;

                                    var encoder = new AnimatedGifEncoder();

                                    using (var file = File.OpenWrite(outputFilename))
                                    {
                                        encoder.Start(file);

                                        encoder.SetRepeat(0);
                                        encoder.SetDelay((int)(this.FrameTime * 1000));
                                        for (var i = 0; i < frameSources.Length; ++i)
                                        {
                                            encodeProgress.ReportProgress((double)i / frameSources.Length);

                                            if (t.Result.IsCanceled)
                                                return;

                                            if (this.IsBackgroundTransparent)
                                                encoder.SetTransparentColor(shadeColor);

                                            encoder.AddFrame(frameSources[i]);
                                        }

                                        encoder.Finish();
                                    }

                                    progress.ReportIsIndetermine();

                                    t.Result.SetTitle(this.CaptureCompletedDialogTitle);
                                    t.Result.SetMessage(string.Format(this.CaptureCompletedDialogMessage, outputFilename));
                                    Thread.Sleep(1000);

                                    progress.ReportProgress(1.0);
                                }));
                            }));
                });

        }

        public override void UpdatePreview()
        {
            if (this.ShouldPreview)
            {
                var now = DateTime.Now;
                var dt = (now - this.PreviousFrameTime).TotalSeconds;
                if (dt >= this.FrameTime)
                {
                    this.PreviousFrameTime = now;
                    this.UpdatePreview(dt);
                }
            }
        }

        protected virtual void UpdatePreview(double dt)
        {

        }
    }
}
