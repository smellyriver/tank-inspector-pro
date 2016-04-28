using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Graphics.Scene;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public abstract class StaticCapturerBase : Capturer
    {
        protected static Color GetStaticShadeColor(Color shadeColor)
        {
            return Color.FromArgb(0, shadeColor.R, shadeColor.G, shadeColor.B);
        }

        public override bool AllowPreview
        {
            get { return false; }
        }

        public override int MinimumOutputWidth
        {
            get { return ModelSharedSettings.Default.MinimumStaticOutputWidth; }
        }

        public override int MinimumOutputHeight
        {
            get { return ModelSharedSettings.Default.MinimumStaticOutputHeight; }
        }

        public override int MaximumOutputWidth
        {
            get { return ModelSharedSettings.Default.MaximumStaticOutputWidth; }
        }

        public override int MaximumOutputHeight
        {
            get { return ModelSharedSettings.Default.MaximumStaticOutputHeight; }
        }

        public override double SampleFactor
        {
            get
            {
                return ModelSharedSettings.Default.StaticCaptureSampleFactor;
            }
            set
            {
                ModelSharedSettings.Default.StaticCaptureSampleFactor = value;
                ModelSharedSettings.Default.Save();
                this.RaisePropertyChanged(() => this.SampleFactor);
            }
        }

        public override string OutputExtensionName
        {
            get { return ".png"; }
        }

        public override string OutputFileFilter
        {
            get { return string.Format("{0} (*.png)|*.png", this.L("model_shared", "png_file_type_filter")); }
        }

        protected abstract string CapturingDialogTitle { get; }
        protected abstract string CapturingDialogMessage { get; }

        protected abstract string CaptureCompletedDialogTitle { get; }
        protected abstract string CaptureCompletedDialogMessage { get; }
        public StaticCapturerBase(CaptureVMBase owner)
            : base(owner)
        {

        }

        public override void BeginCapture(ISnapshotProvider snapshotProvider, TankInstance tank, Rect clippingRectangle, Color shadeColor, string outputFilename)
        {
            DialogManager.Instance.ShowProgressAsync(this.CapturingDialogTitle,
                                                     string.Format(this.CapturingDialogMessage, tank.Name),
                                                     isCancellable: false)
                .ContinueWith(t =>
                {
                    t.Result.SetIndeterminate();

                    var captureCompleted = false;

                    var dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background,
                                                              Application.Current.Dispatcher);
                    dispatcherTimer.Interval = TimeSpan.FromSeconds(0.5);
                    dispatcherTimer.Tick +=
                        (o, e) =>
                        {
                            dispatcherTimer.Stop();

                            var bitmapSource = snapshotProvider.Snapshot(clippingRectangle, (float)this.SampleFactor, StaticCapturerBase.GetStaticShadeColor(shadeColor));
                            using (var file = File.OpenWrite(outputFilename))
                            {
                                var encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                                encoder.Save(file);
                            }

                            captureCompleted = true;
                        };

                    dispatcherTimer.Start();


                    while (!captureCompleted)
                    {
                        Thread.Sleep(100);
                    }

                    t.Result.SetTitle(this.CaptureCompletedDialogTitle);
                    t.Result.SetMessage(string.Format(this.CaptureCompletedDialogMessage, outputFilename));
                    Thread.Sleep(1000);
                    t.Result.CloseAsync();
                });
        }
    }
}
