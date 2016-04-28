using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Controls;

namespace Smellyriver.TankInspector.Pro.ArmorInspector
{
    public partial class ArmorDocumentView : UserControl
    {
        internal ArmorDocumentVM ViewModel
        {
            get { return this.DataContext as ArmorDocumentVM; }
            set
            {
                this.DataContext = value;
                value.SnapshotProvider = this.ModelView.SnapshotProvider;
                value.AlternativeSnapshotProvider = this.ModelView.AlternativeSnapshotProvider;

                value.Capture.IsCapturingChanged += Capture_IsCapturingChanged;
                this.NotifyViewportSizeToViewModel();
            }
        }


        public ArmorDocumentView()
        {
            InitializeComponent();
            if (this.ViewModel != null)
                this.ViewModel.SnapshotProvider = this.ModelView.SnapshotProvider;

            this.ModelView.SizeChanged += ModelView_SizeChanged;
        }

        void ModelView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.NotifyViewportSizeToViewModel();
        }

        private void NotifyViewportSizeToViewModel()
        {
            if (this.ModelView.IsLoaded)
                this.ViewModel.Capture.NotifyViewportSize(this.ModelView.ActualWidth, this.ModelView.ActualHeight);
        }

        private CroppingAdorner _croppingAdorner;

        void Capture_IsCapturingChanged(object sender, EventArgs e)
        {
            if (this.ViewModel.Capture.IsCapturing)
            {
                var interior = new Rect(this.ModelView.ActualWidth * 0.1,
                                        this.ModelView.ActualHeight * 0.1,
                                        this.ModelView.ActualWidth * 0.8,
                                        this.ModelView.ActualHeight * 0.8);
                var adornerLayer = AdornerLayer.GetAdornerLayer(this.ModelView);
                _croppingAdorner = new CroppingAdorner(this.ModelView, interior);
                _croppingAdorner.Fill = new SolidColorBrush(Color.FromArgb(0xC0, 0, 0, 0));

                _croppingAdorner.SetBinding(CroppingAdorner.ClippingRectangleProperty,
                                            new Binding("ClippingRectangle")
                                            {
                                                Source = this.ViewModel.Capture,
                                                Mode = BindingMode.TwoWay
                                            });

                _croppingAdorner.SetBinding(CroppingAdorner.MinimumClippingWidthProperty,
                                            new Binding("MinimumClippingWidth") { Source = this.ViewModel.Capture });

                _croppingAdorner.SetBinding(CroppingAdorner.MinimumClippingHeightProperty,
                                           new Binding("MinimumClippingHeight") { Source = this.ViewModel.Capture });

                _croppingAdorner.SetBinding(CroppingAdorner.IsResizeEnabledProperty,
                                            new Binding("IsResizeEnabled") { Source = this.ViewModel.Capture });

                _croppingAdorner.SetBinding(CroppingAdorner.UniformScaleProperty,
                                            new Binding("IsOutputSizeLocked") { Source = this.ViewModel.Capture });

                adornerLayer.Add(_croppingAdorner);
            }
            else
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this.ModelView);
                adornerLayer.Remove(_croppingAdorner);
            }
        }

        private void ModelView_AlternativeHangarSceneChanged(object sender, EventArgs e)
        {
            this.ViewModel.AlternativeSnapshotProvider = this.ModelView.AlternativeSnapshotProvider;
        }

    }
}
