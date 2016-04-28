using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Graphics.Controls;
using Smellyriver.TankInspector.Pro.Graphics.Scene;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public abstract class ModelComparerCapturer : GifAnimationCapturer
    {
        

        public double MaximumInterval
        {
            get { return ModelSharedSettings.Default.MaximumModelComparerInterval; }
        }
        public double MinimumInterval
        {
            get { return ModelSharedSettings.Default.MinimumModelComparerInterval; }
        }


        public double Interval
        {
            get { return ModelSharedSettings.Default.ModelComparerInterval; }
            set
            {
                ModelSharedSettings.Default.ModelComparerInterval = value.Clamp(this.MinimumInterval,
                                                                                this.MaximumInterval);
                this.RaisePropertyChanged(() => this.Interval);
            }
        }

        public override bool IsPreviewing
        {
            get { return base.IsPreviewing; }
            set
            {
                base.IsPreviewing = value;
                this.RaisePropertyChanged(() => this.ShouldPreviewPrintVersion);
            }
        }

        public bool PrintVersion
        {
            get { return ModelSharedSettings.Default.PrintVersionOnModelComparer; }
            set
            {
                ModelSharedSettings.Default.PrintVersionOnModelComparer = value;
                this.RaisePropertyChanged(() => this.PrintVersion);
                this.RaisePropertyChanged(() => this.ShouldPreviewPrintVersion);
            }
        }

        public bool ShouldPreviewPrintVersion
        {
            get { return this.ShouldPreview && this.PrintVersion; }
        }

        public double VersionTagWidth { get { return this.Owner.ClippingRectangle.Width; } }
        public double VersionTagHeight { get { return 30; } }
        public double VersionTagLeft { get { return this.Owner.ClippingRectangle.Left; } }
        public double VersionTagTop { get { return this.Owner.ClippingRectangle.Bottom - this.VersionTagHeight; } }

        protected override double FrameTime
        {
            get { return this.Interval; }
        }

        private IEnumerable<ModelComparerRivalVM> _rivals;
        public IEnumerable<ModelComparerRivalVM> Rivals
        {
            get { return _rivals; }
            private set
            {
                _rivals = value;
                this.RaisePropertyChanged(() => this.Rivals);
            }
        }

        private ModelComparerRivalVM _selectedRival;
        public ModelComparerRivalVM SelectedRival
        {
            get { return _selectedRival; }
            set
            {
                _selectedRival = value;
                this.RaisePropertyChanged(() => this.SelectedRival);
                this.LoadRival();
            }
        }

        private TankInstance TankInstance { get { return this.Owner.Owner.TankInstance; } }
        private TankInstance RivalTankInstance { get; set; }

        public TankInstance CurrentTankInstance
        {
            get
            {
                if (this.Owner.Owner.HangarSceneSource == HangarSceneSource.Alternative)
                    return this.RivalTankInstance;
                else
                    return this.TankInstance;
            }
        }

        protected override bool IsBackgroundTransparent { get { return false; } }

        private void LoadRival()
        {
            if (this.SelectedRival == null)
                this.RivalTankInstance = null;
            else
            {
                this.RivalTankInstance = this.SelectedRival.Model.Clone();
                this.RivalTankInstance.TankConfiguration = this.TankInstance.TankConfiguration;
                this.RivalTankInstance.CrewConfiguration = this.TankInstance.CrewConfiguration;
                this.RivalTankInstance.CustomizationConfiguration = this.TankInstance.CustomizationConfiguration;
            }

            this.Owner.Owner.AlternativeTankInstance = this.RivalTankInstance;
        }

        protected internal override void OnClippingRectangleChanged(Rect clippingRectangle)
        {
            base.OnClippingRectangleChanged(clippingRectangle);
            this.RaisePropertyChanged(() => this.VersionTagLeft);
            this.RaisePropertyChanged(() => this.VersionTagTop);
            this.RaisePropertyChanged(() => this.VersionTagWidth);
        }

        public ModelComparerCapturer(CaptureVMBase owner)
            : base(owner)
        {
            ((INotifyCollectionChanged)RepositoryManager.Instance.Repositories).CollectionChanged += Repositories_CollectionChanged;
            this.UpdateRivals();

            this.TankInstance.TankConfiguration.PropertyChanged += TankConfiguration_PropertyChanged;
            this.TankInstance.CrewConfiguration.PropertyChanged += CrewConfiguration_PropertyChanged;
            this.TankInstance.CustomizationConfiguration.PropertyChanged += CustomizationConfiguration_PropertyChanged;
        }

        void CustomizationConfiguration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.RivalTankInstance != null)
                this.RivalTankInstance.CustomizationConfiguration = this.TankInstance.CustomizationConfiguration;
        }

        void CrewConfiguration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.RivalTankInstance != null)
                this.RivalTankInstance.CrewConfiguration = this.TankInstance.CrewConfiguration;
        }

        void TankConfiguration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.RivalTankInstance != null)
                this.RivalTankInstance.TankConfiguration = this.TankInstance.TankConfiguration;
        }

        private void Repositories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateRivals();
        }

        private void UpdateRivals()
        {
            var rivals = new List<ModelComparerRivalVM>();

            var currentTank = this.TankInstance;
            foreach (var repository in RepositoryManager.Instance.Repositories)
            {
                if (!(repository is LocalGameClient))
                    continue;

                if (repository == currentTank.Repository)
                    continue;

                var tank = repository.GetTank(currentTank.Tank.NationKey, currentTank.Tank.Key);
                if (tank == null)
                    continue;

                var instance = TankInstanceManager.GetInstance(repository, tank);
                var rival = new ModelComparerRivalVM(instance);
                rivals.Add(rival);
            }

            this.Rivals = rivals;

            if (rivals.Count > 0)
            {
                if (this.SelectedRival != null)
                    this.SelectedRival = rivals.FirstOrDefault(r => r.Model.Repository == this.SelectedRival.Model.Repository);

                if (this.SelectedRival == null)
                    this.SelectedRival = rivals.First();
            }
            else
                this.SelectedRival = null;

            this.IsEnabled = rivals.Count > 0;
        }


        protected override BitmapSource[] CaptureFrames(ISnapshotProvider snapshotProvider, Rect clippingRectangle, Color shadeColor, IProgressScope progress, Func<bool> getIsCancelled)
        {
            var frame1 = this.PrintVersionTag(snapshotProvider.Snapshot(clippingRectangle, this.SampleFactor, shadeColor),
                                              this.Owner.Owner.TankInstance);

            var frame2 = this.PrintVersionTag(this.Owner.Owner.AlternativeSnapshotProvider.Snapshot(clippingRectangle, this.SampleFactor, shadeColor),
                                              this.Owner.Owner.AlternativeTankInstance);

            return new[] { frame1, frame2 };
        }

        private BitmapSource PrintVersionTag(BitmapSource image, TankInstance tank)
        {
            var versionTag = new VersionTag();
            versionTag.Width = image.PixelWidth;
            versionTag.VerticalAlignment = VerticalAlignment.Bottom;
            versionTag.TankInstance = tank;
            versionTag.MeasureAndArrange();
            var versionTagImage = versionTag.RenderToBitmap();

            var composite = new WriteableBitmap(image);

            var pixels = new int[(int)versionTagImage.Width * (int)versionTagImage.Height];
            var stride = (int)(4 * versionTagImage.Width);
            versionTagImage.CopyPixels(pixels, stride, 0);
            var rect = new Int32Rect(0,
                                     (int)(image.PixelHeight - versionTagImage.Height),
                                     (int)versionTagImage.Width,
                                     (int)versionTagImage.Height);
            composite.WritePixels(rect, pixels, stride, 0);

            return composite;
        }

        protected override void UpdatePreview(double dt)
        {
            if (this.Owner.Owner.HangarSceneSource == HangarSceneSource.Alternative)
                this.Owner.Owner.HangarSceneSource = HangarSceneSource.Primary;
            else
                this.Owner.Owner.HangarSceneSource = HangarSceneSource.Alternative;

            this.RaisePropertyChanged(() => this.CurrentTankInstance);
        }

        public override void EndPreview()
        {
            this.Owner.Owner.HangarSceneSource = HangarSceneSource.Primary;
            this.RaisePropertyChanged(() => this.CurrentTankInstance);
            this.RaisePropertyChanged(() => this.ShouldPreviewPrintVersion);
        }
    }
}
