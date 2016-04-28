using System.Windows;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Graphics.Scene;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public abstract class Capturer : NotificationObject
    {
        public abstract string Name { get; }
        
        public abstract string Description { get; }


        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                this.RaisePropertyChanged(() => this.IsEnabled);
            }
        }

        public abstract bool AllowPreview { get; }

        private bool _isPreviewing;
        public virtual bool IsPreviewing
        {
            get { return _isPreviewing; }
            set
            {
                _isPreviewing = value;
                this.RaisePropertyChanged(() => this.IsPreviewing);
                this.RaisePropertyChanged(() => this.ShouldPreview);
            }
        }


        public bool ShouldPreview
        {
            get
            {
                return this.IsPreviewing
                    && this.AllowPreview;
            }
        }

        public abstract int MinimumOutputWidth { get; }
        public abstract int MinimumOutputHeight { get; }
        public abstract int MaximumOutputWidth { get; }
        public abstract int MaximumOutputHeight { get; }
        public abstract double SampleFactor { get; set; }

        public abstract string OutputExtensionName { get; }
        public abstract string OutputFileFilter { get; }

        protected CaptureVMBase Owner { get; private set; }

        public Capturer(CaptureVMBase owner)
        {
            this.Owner = owner;
            this.IsEnabled = true;
        }

        public abstract void BeginCapture(ISnapshotProvider snapshotProvider, TankInstance tank, Rect clippingRectangle, Color shadeColor, string outputFilename);

        public virtual void BeginPreview()
        {
            
        }

        public virtual void UpdatePreview()
        {

        }

        public virtual void EndPreview()
        {

        }

        internal protected virtual void OnClippingRectangleChanged(Rect clippingRectangle)
        {

        }

    }
}
