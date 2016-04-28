using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public abstract class CaptureVMBase : NotificationObject, IDataErrorInfo
    {
        public event EventHandler IsCapturingChanged;

        private bool _isCapturing;
        public bool IsCapturing
        {
            get { return _isCapturing; }
            set
            {
                _isCapturing = value;

                if (this.IsCapturingChanged != null)
                    this.IsCapturingChanged(this, EventArgs.Empty);

                this.RaisePropertyChanged(() => this.IsCapturing);

                this.UpdateAnimationPreview();
            }
        }


        private bool _isPreviewingAnimation;

        private Capturer[] _capturers;
        public IEnumerable<Capturer> Capturers { get { return _capturers; } }

        private Capturer _selectedCapturer;
        public virtual Capturer SelectedCapturer
        {
            get { return _selectedCapturer; }
            set
            {
                _selectedCapturer = value;
                this.RaisePropertyChanged(() => this.SelectedCapturer);
                this.UpdateOutputFilename();
                this.UpdateAnimationPreview();

            }
        }


        public bool IsPreviewEnabled
        {
            get { return ModelSharedSettings.Default.EnableAnimationPreview; }
            set
            {
                ModelSharedSettings.Default.EnableAnimationPreview = value;
                ModelSharedSettings.Default.Save();
                this.UpdateAnimationPreview();
            }
        }

        public Color ShadeColor
        {
            get { return ModelSharedSettings.Default.ShadeColor; }
            set
            {
                ModelSharedSettings.Default.ShadeColor = value;
                ModelSharedSettings.Default.Save();
                this.RaisePropertyChanged(() => this.ShadeColor);
                this.RaisePropertyChanged(() => this.IsBlackShadeSelected);
                this.RaisePropertyChanged(() => this.IsWhiteShadeSelected);
            }
        }

        public bool IsBlackShadeSelected
        {
            get { return this.ShadeColor == Colors.Black; }
            set
            {
                if (value == true)
                    this.ShadeColor = Colors.Black;
            }
        }

        public bool IsWhiteShadeSelected
        {
            get { return this.ShadeColor == Colors.White; }
            set
            {
                if (value == true)
                    this.ShadeColor = Colors.White;
            }
        }

        private double _viewportWidth;
        private double _viewportHeight;

        private bool _firstViewportSizeNotified;


        private Rect _clippingRectangle;
        public Rect ClippingRectangle
        {
            get { return _clippingRectangle; }
            set
            {
                //if (!this.IsCapturing)
                //    return;

                _clippingRectangle = value;
                this.RaisePropertyChanged(() => this.ClippingRectangle);
                this.RaisePropertyChanged(() => this.ClippingRectangleDisplay);
                this.RaisePropertyChanged(() => this.MaximumSampleFactor);
                this.RaisePropertyChanged(() => this.MinimumSampleFactor);
                this.SampleFactor = this.SampleFactor.Clamp(this.MinimumSampleFactor, this.MaximumSampleFactor);
                this.UpdateClippingRatioRectangle();
                if (this.SelectedCapturer != null)
                    this.SelectedCapturer.OnClippingRectangleChanged(_clippingRectangle);
            }
        }

        public string ClippingRectangleDisplay
        {
            get
            {
                return this.L("model_shared",
                              "selected_region_format",
                              this.ClippingRectangle.Left,
                              this.ClippingRectangle.Top,
                              this.ClippingRectangle.Width,
                              this.ClippingRectangle.Height);
            }
        }


        public double MinimumSampleFactor
        {
            get
            {
                return Math.Max(this.SelectedCapturer.MinimumOutputWidth / this.ClippingRectangle.Width,
                                this.SelectedCapturer.MinimumOutputHeight / this.ClippingRectangle.Height);
            }
        }

        public double MaximumSampleFactor
        {
            get
            {
                return Math.Min(this.SelectedCapturer.MaximumOutputWidth / this.ClippingRectangle.Width,
                                this.SelectedCapturer.MaximumOutputHeight / this.ClippingRectangle.Height);
            }
        }

        public double SampleFactor
        {
            get
            {
                return this.SelectedCapturer.SampleFactor;
            }
            set
            {
                this.SelectedCapturer.SampleFactor = value.Clamp(this.MinimumSampleFactor, this.MaximumSampleFactor);

                this.RaisePropertyChanged(() => this.OutputWidth);
                this.RaisePropertyChanged(() => this.OutputHeight);
                this.RaisePropertyChanged(() => this.SampleFactor);
            }
        }


        public int OutputWidth
        {
            get { return (int)(this.ClippingRectangle.Width * this.SampleFactor); }
        }

        public int OutputHeight
        {
            get { return (int)(this.ClippingRectangle.Height * this.SampleFactor); }
        }


        public string OutputFilename
        {
            get { return this.Owner.PersistentInfo.CaptureFilename; }
            set
            {
                this.Owner.PersistentInfo.CaptureFilename = value;

                if (this.IsOutputFilenameValid)
                    ModelSharedSettings.Default.OutputDirectory = Path.GetDirectoryName(value);

                this.RaisePropertyChanged(() => this.OutputFilename);
                CommandManager.InvalidateRequerySuggested();
            }
        }


        private bool IsOutputFilenameValid
        {
            get
            {
                return this.OutputFilename != null
                    && this.OutputFilename.IndexOfAny(Path.GetInvalidPathChars()) == -1
                    && Directory.Exists(Path.GetDirectoryName(this.OutputFilename));
            }
        }


        public ICommand BrowseOutputFileCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }

        public ModelDocumentVMBase Owner { get; }


        public CaptureVMBase(ModelDocumentVMBase owner)
        {
            this.Owner = owner;

            _capturers = this.GetCapturers();

            this.SelectedCapturer = _capturers.First();

            this.BrowseOutputFileCommand = new RelayCommand(this.BrowseOutputFile);
            this.ExportCommand = new RelayCommand(this.Export, this.CanExport);

            this.UpdateOutputFilename();
            this.UpdateAnimationPreview();
        }

        protected abstract Capturer[] GetCapturers();

        private void UpdateOutputFilename()
        {
            if (this.IsOutputFilenameValid)
            {
                this.OutputFilename = Path.ChangeExtension(this.OutputFilename, this.SelectedCapturer.OutputExtensionName);
            }
            else
            {
                if (Directory.Exists(ModelSharedSettings.Default.OutputDirectory))
                {
                    this.OutputFilename = Path.Combine(ModelSharedSettings.Default.OutputDirectory, string.Format("{0}{1}", this.Owner.TankInstance.Tank.Key, this.SelectedCapturer.OutputExtensionName));
                }
            }

        }
        private bool CanExport()
        {
            return this.IsOutputFilenameValid
                && this.SelectedCapturer != null
                && this.SelectedCapturer.IsEnabled;
        }


        private void Export()
        {
            this.SelectedCapturer.BeginCapture(this.Owner.SnapshotProvider, this.Owner.TankInstance, this.ClippingRectangle, this.ShadeColor, this.OutputFilename);
        }

        private void BrowseOutputFile()
        {
            var fileName = this.OutputFilename ?? string.Format("{0}{1}", this.Owner.TankInstance.Tank.Key, this.SelectedCapturer.OutputExtensionName)
                           .Replace(Path.GetInvalidFileNameChars(), '-');

            var filter = string.Format("{0}|{1} (*.*)|*.*",
                                       this.SelectedCapturer.OutputFileFilter,
                                       this.L("common", "all_file_types_filter"));

            var result = DialogManager.Instance.ShowSaveFileDialog(title: this.L("model_shared", "export_dialog_title"),
                                                                   fileName: ref fileName,
                                                                   filter: filter,
                                                                   filterIndex: 0,
                                                                   defaultExtensionName: this.SelectedCapturer.OutputExtensionName,
                                                                   overwritePrompt: true,
                                                                   addExtension: true,
                                                                   checkPathExists: true);
            if (result == true)
            {
                this.OutputFilename = fileName;
            }
        }

        private void UpdateAnimationPreview()
        {
            var shouldPreview = this.IsCapturing && this.IsPreviewEnabled;
            this.SelectedCapturer.IsPreviewing = shouldPreview;

            if (shouldPreview)
            {
                if (!_isPreviewingAnimation)
                {
                    this.SelectedCapturer.BeginPreview();
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                    _isPreviewingAnimation = true;
                }
            }
            else
            {
                if (_isPreviewingAnimation)
                {
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                    _isPreviewingAnimation = false;
                    this.SelectedCapturer.EndPreview();
                }
            }

        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this.SelectedCapturer.UpdatePreview();
        }


        public void NotifyViewportSize(double width, double height)
        {
            _viewportWidth = width;
            _viewportHeight = height;

            if (!_firstViewportSizeNotified)
            {
                _firstViewportSizeNotified = true;
                var clippingRatioRect = ModelSharedSettings.Default.ClippingRatioRectangle;
                this.ClippingRectangle =
                    new Rect(clippingRatioRect.Left * _viewportWidth,
                             clippingRatioRect.Top * _viewportHeight,
                             clippingRatioRect.Width * _viewportWidth,
                             clippingRatioRect.Height * _viewportHeight);
            }
        }

        private void UpdateClippingRatioRectangle()
        {
            ModelSharedSettings.Default.ClippingRatioRectangle =
                new Rect((this.ClippingRectangle.Left / _viewportWidth).Clamp(0, 1),
                         (this.ClippingRectangle.Top / _viewportHeight).Clamp(0, 1),
                         (this.ClippingRectangle.Width / _viewportWidth).Clamp(0, 1),
                         (this.ClippingRectangle.Height / _viewportHeight).Clamp(0, 1));

            ModelSharedSettings.Default.Save();
        }


        string IDataErrorInfo.Error
        {
            get { return string.Empty; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (columnName == "OutputFilename")
                {
                    if (!this.IsOutputFilenameValid)
                        return this.L("model_shared", "invalid_filename");
                }
                return null;
            }
        }

    }
}
