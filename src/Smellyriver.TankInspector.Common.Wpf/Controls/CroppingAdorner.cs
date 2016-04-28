using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Point = System.Drawing.Point;
using Size = System.Windows.Size;

namespace Smellyriver.TankInspector.Common.Wpf.Controls
{
    public partial class CroppingAdorner : Adorner
    {

        // Width of the thumbs.  I know these really aren't "pixels", but px
        // is still a good mnemonic.
        private const int _thumbWidth = 6;

        // PuncturedRect to hold the "Cropping" portion of the adorner
        private PuncturedRect _cropMask;

        // Canvas to hold the thumbs so they can be moved in response to the user
        private Canvas _thumbsCanvas;

        // Cropping adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        private CropThumb _topLeftThumb, _topRightThumb, _bottomLeftThumb, _bottomRightThumb;
        private CropThumb _topThumb, _leftThumb, _bottomThumb, _rightThumb;

        private DragThumb _dragThumb;

        // To store and manage the adorner's visual children.
        private VisualCollection _visuals;

        // DPI for screen
        private static double s_dpiX, s_dpiY;


        public Rect ClippingRectangle
        {
            get { return (Rect)this.GetValue(ClippingRectangleProperty); }
            set { this.SetValue(ClippingRectangleProperty, value); }
        }

        public static readonly DependencyProperty ClippingRectangleProperty =
            DependencyProperty.Register("ClippingRectangle", typeof(Rect), typeof(CroppingAdorner), new PropertyMetadata(Rect.Empty, CroppingAdorner.OnClippingRectangleChanged, CroppingAdorner.OnCoerceClippingRectangle));

        private static object OnCoerceClippingRectangle(DependencyObject d, object baseValue)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null || !adorner.IsLoaded)
                return baseValue;

            var rect = (Rect)baseValue;

            rect.X = rect.X.Clamp(0, adorner.ActualWidth);
            rect.Y = rect.Y.Clamp(0, adorner.ActualHeight);

            if (rect.X + rect.Width > adorner.ActualWidth)
            {
                var delta = rect.X + rect.Width - adorner.ActualWidth;
                if (rect.X > 0)
                {
                    var dx = Math.Min(rect.X, delta);
                    rect.X -= dx;
                    delta -= dx;
                }
                if(delta > 0)
                {
                    var dx = Math.Min(rect.Width, delta);
                    rect.Width -= dx;
                }
            }

            if (rect.Y + rect.Height > adorner.ActualHeight)
            {
                var delta = rect.Y + rect.Height - adorner.ActualHeight;
                if (rect.Y > 0)
                {
                    var dy = Math.Min(rect.Y, delta);
                    rect.Y -= dy;
                    delta -= dy;
                }
                if (delta > 0)
                {
                    var dy = Math.Min(rect.Height, delta);
                    rect.Height -= dy;
                }
            }

            return rect;
        }

        private static void OnClippingRectangleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null || !adorner.IsLoaded)
                return;

            adorner.HandleThumb(0, 0, 0, 0, 0, 0);
        }

        public double? MinimumClippingWidth
        {
            get { return (double?)this.GetValue(MinimumClippingWidthProperty); }
            set { this.SetValue(MinimumClippingWidthProperty, value); }
        }

        public static readonly DependencyProperty MinimumClippingWidthProperty =
            DependencyProperty.Register("MinimumClippingWidth", typeof(double?), typeof(CroppingAdorner), new PropertyMetadata(null, CroppingAdorner.OnSizeLimitationChanged, CroppingAdorner.OnCoerceMinimumClippingWidth));

        private static object OnCoerceMinimumClippingWidth(DependencyObject d, object baseValue)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null)
                return baseValue;

            if (baseValue == null)
                return null;

            var doubleBaseValue = (double?)baseValue;

            if (doubleBaseValue.Value < 0)
                return 0.0;

            if (adorner.MaximumClippingWidth == null)
                return baseValue;

            return doubleBaseValue.Value > adorner.MaximumClippingWidth.Value ? adorner.MaximumClippingWidth : baseValue;
        }

        public double? MinimumClippingHeight
        {
            get { return (double?)this.GetValue(MinimumClippingHeightProperty); }
            set { this.SetValue(MinimumClippingHeightProperty, value); }
        }

        public static readonly DependencyProperty MinimumClippingHeightProperty =
            DependencyProperty.Register("MinimumClippingHeight", typeof(double?), typeof(CroppingAdorner), new PropertyMetadata(null, CroppingAdorner.OnSizeLimitationChanged, CroppingAdorner.OnCoerceMinimumClippingHeight));

        private static object OnCoerceMinimumClippingHeight(DependencyObject d, object baseValue)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null)
                return baseValue;

            var doubleBaseValue = (double?)baseValue;

            if (doubleBaseValue < 0)
                return 0.0;

            if (adorner.MaximumClippingHeight == null)
                return baseValue;

            return doubleBaseValue.Value > adorner.MaximumClippingHeight.Value ? adorner.MaximumClippingHeight : baseValue;
        }

        public double? MaximumClippingWidth
        {
            get { return (double?)this.GetValue(MaximumClippingWidthProperty); }
            set { this.SetValue(MaximumClippingWidthProperty, value); }
        }

        public static readonly DependencyProperty MaximumClippingWidthProperty =
            DependencyProperty.Register("MaximumClippingWidth", typeof(double?), typeof(CroppingAdorner), new PropertyMetadata(null, CroppingAdorner.OnSizeLimitationChanged, CroppingAdorner.OnCoerceMaximumClippingWidth));

        private static object OnCoerceMaximumClippingWidth(DependencyObject d, object baseValue)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null)
                return baseValue;

            if (baseValue == null)
                return null;

            var doubleBaseValue = (double?)baseValue;

            if (doubleBaseValue.Value < 0)
                return 0.0;

            if (adorner.IsLoaded && doubleBaseValue.Value > adorner.ActualWidth)
                doubleBaseValue = adorner.ActualWidth;

            if (adorner.MinimumClippingWidth == null)
                return doubleBaseValue;

            return doubleBaseValue.Value < adorner.MinimumClippingWidth.Value ? adorner.MinimumClippingWidth : baseValue;
        }

        public double? MaximumClippingHeight
        {
            get { return (double?)this.GetValue(MaximumClippingHeightProperty); }
            set { this.SetValue(MaximumClippingHeightProperty, value); }
        }

        public static readonly DependencyProperty MaximumClippingHeightProperty =
            DependencyProperty.Register("MaximumClippingHeight", typeof(double?), typeof(CroppingAdorner), new PropertyMetadata(null, CroppingAdorner.OnSizeLimitationChanged, CroppingAdorner.OnCoerceMaximumClippingHeight));

        private static object OnCoerceMaximumClippingHeight(DependencyObject d, object baseValue)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null)
                return baseValue;

            if (baseValue == null)
                return null;

            var doubleBaseValue = (double?)baseValue;

            if (doubleBaseValue.Value < 0)
                return 0.0;

            if (adorner.IsLoaded && doubleBaseValue.Value > adorner.ActualHeight)
                doubleBaseValue = adorner.ActualHeight;

            if (adorner.MinimumClippingHeight == null)
                return doubleBaseValue;

            return doubleBaseValue.Value < adorner.MinimumClippingHeight.Value ? adorner.MinimumClippingHeight : baseValue;
        }

        private static void OnSizeLimitationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null)
                return;

            adorner.HandleThumb(0, 0, 0, 0, 0, 0);
        }

        public bool IsResizeEnabled
        {
            get { return (bool)this.GetValue(IsResizeEnabledProperty); }
            set { this.SetValue(IsResizeEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsResizeEnabledProperty =
            DependencyProperty.Register("IsResizeEnabled", typeof(bool), typeof(CroppingAdorner), new PropertyMetadata(true, CroppingAdorner.OnIsResizeEnabledChanged));


        private static void OnIsResizeEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null)
                return;

            adorner._topLeftThumb.Visibility
                = adorner._topRightThumb.Visibility
                = adorner._bottomLeftThumb.Visibility
                = adorner._bottomRightThumb.Visibility
                = (bool)e.NewValue
                ? Visibility.Visible
                : Visibility.Hidden;

            adorner._topThumb.Visibility
                = adorner._leftThumb.Visibility
                = adorner._rightThumb.Visibility
                = adorner._bottomThumb.Visibility
                = ((bool)e.NewValue && !adorner.UniformScale)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        public bool IsDragEnabled
        {
            get { return (bool)this.GetValue(IsDragEnabledProperty); }
            set { this.SetValue(IsDragEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsDragEnabledProperty =
            DependencyProperty.Register("IsDragEnabled", typeof(bool), typeof(CroppingAdorner), new PropertyMetadata(true, CroppingAdorner.OnIsDragEnabledChanged));

        private static void OnIsDragEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null)
                return;

            adorner._dragThumb.Visibility
                = (bool)e.NewValue
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        public bool ScaleOnTargetResize
        {
            get { return (bool)this.GetValue(ScaleOnTargetResizeProperty); }
            set { this.SetValue(ScaleOnTargetResizeProperty, value); }
        }

        public static readonly DependencyProperty ScaleOnTargetResizeProperty =
            DependencyProperty.Register("ScaleOnTargetResize", typeof(bool), typeof(CroppingAdorner), new PropertyMetadata(true));



        public bool UniformScale
        {
            get { return (bool)this.GetValue(UniformScaleProperty); }
            set { this.SetValue(UniformScaleProperty, value); }
        }

        public static readonly DependencyProperty UniformScaleProperty =
            DependencyProperty.Register("UniformScale", typeof(bool), typeof(CroppingAdorner), new PropertyMetadata(false, CroppingAdorner.OnUniformScaleChanged));

        private double _aspectRatio;

        private static void OnUniformScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorner = d as CroppingAdorner;
            if (adorner == null)
                return;

            adorner._topThumb.Visibility
                = adorner._leftThumb.Visibility
                = adorner._rightThumb.Visibility
                = adorner._bottomThumb.Visibility
                = (!(bool)e.NewValue && adorner.IsResizeEnabled)
                ? Visibility.Visible
                : Visibility.Hidden;

            if (adorner.ClippingRectangle.Height == 0)
                adorner._aspectRatio = 1.0;
            else
                adorner._aspectRatio = adorner.ClippingRectangle.Width / adorner.ClippingRectangle.Height;
        }

        public static readonly RoutedEvent CropChangedEvent = EventManager.RegisterRoutedEvent(
            "CropChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(CroppingAdorner));

        public event RoutedEventHandler CropChanged
        {
            add
            {
                base.AddHandler(CropChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(CropChangedEvent, value);
            }
        }



        static public DependencyProperty FillProperty = Shape.FillProperty.AddOwner(typeof(CroppingAdorner));

        public Brush Fill
        {
            get { return (Brush)this.GetValue(FillProperty); }
            set { this.SetValue(FillProperty, value); }
        }

        private static void FillPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            CroppingAdorner crp = d as CroppingAdorner;

            if (crp != null)
            {
                crp._cropMask.Fill = (Brush)args.NewValue;
            }
        }



        static CroppingAdorner()
        {
            Color clr = Colors.Red;
            Graphics g = Graphics.FromHwnd((IntPtr)0);

            s_dpiX = g.DpiX;
            s_dpiY = g.DpiY;
            clr.A = 80;
            FillProperty.OverrideMetadata(typeof(CroppingAdorner),
                new PropertyMetadata(
                    new SolidColorBrush(clr),
                    new PropertyChangedCallback(CroppingAdorner.FillPropChanged)));
        }

        private bool _isFirstSizeChange = true;

        public CroppingAdorner(UIElement adornedElement, Rect rcInit)
            : base(adornedElement)
        {
            this.ClippingRectangle = rcInit;

            _visuals = new VisualCollection(this);
            _cropMask = new PuncturedRect();
            _cropMask.SetBinding(PuncturedRect.RectInteriorProperty, new Binding("ClippingRectangle") { Source = this, Mode = BindingMode.TwoWay });
            _cropMask.IsHitTestVisible = false;
            _cropMask.Fill = this.Fill;
            _visuals.Add(_cropMask);
            _thumbsCanvas = new Canvas();
            _thumbsCanvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            _thumbsCanvas.VerticalAlignment = VerticalAlignment.Stretch;

            _visuals.Add(_thumbsCanvas);
            this.BuildCorner(ref _topThumb, Cursors.SizeNS);
            this.BuildCorner(ref _bottomThumb, Cursors.SizeNS);
            this.BuildCorner(ref _leftThumb, Cursors.SizeWE);
            this.BuildCorner(ref _rightThumb, Cursors.SizeWE);
            this.BuildCorner(ref _topLeftThumb, Cursors.SizeNWSE);
            this.BuildCorner(ref _topRightThumb, Cursors.SizeNESW);
            this.BuildCorner(ref _bottomLeftThumb, Cursors.SizeNESW);
            this.BuildCorner(ref _bottomRightThumb, Cursors.SizeNWSE);

            this.BuildDragThumb(ref _dragThumb);

            // Add handlers for Cropping.
            _bottomLeftThumb.DragDelta += new DragDeltaEventHandler(this.HandleBottomLeft);
            _bottomRightThumb.DragDelta += new DragDeltaEventHandler(this.HandleBottomRight);
            _topLeftThumb.DragDelta += new DragDeltaEventHandler(this.HandleTopLeft);
            _topRightThumb.DragDelta += new DragDeltaEventHandler(this.HandleTopRight);
            _topThumb.DragDelta += new DragDeltaEventHandler(this.HandleTop);
            _bottomThumb.DragDelta += new DragDeltaEventHandler(this.HandleBottom);
            _rightThumb.DragDelta += new DragDeltaEventHandler(this.HandleRight);
            _leftThumb.DragDelta += new DragDeltaEventHandler(this.HandleLeft);
            _dragThumb.DragDelta += new DragDeltaEventHandler(this.HandleDrag);

            // We have to keep the clipping interior withing the bounds of the adorned element
            // so we have to track it's size to guarantee that...
            FrameworkElement element = adornedElement as FrameworkElement;

            if (element != null)
            {
                element.SizeChanged += new SizeChangedEventHandler(this.AdornedElement_SizeChanged);
            }
        }

        // Generic handler for Cropping
        private void HandleThumb(
            double leftFactor,
            double topFactor,
            double widthFactor,
            double heightFactor,
            double dx,
            double dy)
        {

            Rect interior = this.ClippingRectangle;

            var minimumClippingWidth = this.MinimumClippingWidth == null ? 0 : this.MinimumClippingWidth.Value;
            var minimumClippingHeight = this.MinimumClippingHeight == null ? 0 : this.MinimumClippingHeight.Value;
            var maximumClippingWidth = this.MaximumClippingWidth == null ? this.ActualWidth : this.MaximumClippingWidth.Value;
            var maximumClippingHeight = this.MaximumClippingHeight == null ? this.ActualHeight : this.MaximumClippingHeight.Value;

            Action<double> set_dx =
                x =>
                {
                    dx = x;

                    if (this.UniformScale && heightFactor != 0)
                        dy = ((interior.Width + widthFactor * dx) / _aspectRatio - interior.Height) / heightFactor;

                    if (double.IsInfinity(dy))
                    {

                    }
                };


            Action<double> set_dy =
                y =>
                {
                    dy = y;
                    if (this.UniformScale && widthFactor != 0)
                        dx = ((interior.Height + heightFactor * dy) * _aspectRatio - interior.Width) / widthFactor;

                    if (double.IsInfinity(dx))
                    {

                    }
                };

            Func<double> getExpectedWidth = () => interior.Width + widthFactor * dx;
            Func<double> getExpectedHeight = () => interior.Height + heightFactor * dy;
            Func<double> getExpectedLeft = () => interior.Left + leftFactor * dx;
            Func<double> getExpectedTop = () => interior.Top + topFactor * dy;


            if (_aspectRatio > 1.0)
                set_dx(dx);
            else
                set_dy(dy);

            if (getExpectedWidth() < minimumClippingWidth)
                set_dx((minimumClippingWidth - interior.Width) / widthFactor);

            if (getExpectedWidth() > maximumClippingWidth)
                set_dx((maximumClippingWidth - interior.Width) / widthFactor);

            if (getExpectedHeight() < minimumClippingHeight)
                set_dy((minimumClippingHeight - interior.Height) / heightFactor);

            if (getExpectedHeight() > maximumClippingHeight)
                set_dy((maximumClippingHeight - interior.Height) / heightFactor);

            if (getExpectedLeft() < 0)
                set_dx(-interior.Left / leftFactor);

            if (getExpectedLeft() + getExpectedWidth() > this.ActualWidth)
            {
                if (leftFactor == 0)
                    set_dx((this.ActualWidth - getExpectedLeft() - interior.Width) / widthFactor);
                else
                    set_dx((this.ActualWidth - getExpectedWidth() - interior.Left) / leftFactor);
            }

            if (getExpectedTop() < 0)
                set_dy(-interior.Top / topFactor);

            if (getExpectedTop() + getExpectedHeight() > this.ActualHeight)
            {
                if (topFactor == 0)
                    set_dy((this.ActualHeight - getExpectedTop() - interior.Height) / heightFactor);
                else
                    set_dy((this.ActualHeight - getExpectedHeight() - interior.Top) / topFactor);
            }

            interior = new Rect(getExpectedLeft(), getExpectedTop(), getExpectedWidth(), getExpectedHeight());

            _cropMask.RectInterior = interior;
            this.SetThumbs(_cropMask.RectInterior);
            this.RaiseEvent(new RoutedEventArgs(CropChangedEvent, this));
        }

        

        // Handler for Cropping from the bottom-left.
        private void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            if (!this.IsResizeEnabled)
                return;

            if (sender is CropThumb)
            {
                this.HandleThumb(
                    1, 0, -1, 1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the bottom-right.
        private void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            if (!this.IsResizeEnabled)
                return;

            if (sender is CropThumb)
            {
                this.HandleThumb(
                    0, 0, 1, 1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the top-right.
        private void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            if (!this.IsResizeEnabled)
                return;

            if (sender is CropThumb)
            {
                this.HandleThumb(
                    0, 1, 1, -1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the top-left.
        private void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            if (!this.IsResizeEnabled)
                return;

            if (sender is CropThumb)
            {
                this.HandleThumb(
                    1, 1, -1, -1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the top.
        private void HandleTop(object sender, DragDeltaEventArgs args)
        {
            if (!this.IsResizeEnabled || this.UniformScale)
                return;

            if (sender is CropThumb)
            {
                this.HandleThumb(
                    0, 1, 0, -1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the left.
        private void HandleLeft(object sender, DragDeltaEventArgs args)
        {
            if (!this.IsResizeEnabled || this.UniformScale)
                return;

            if (sender is CropThumb)
            {
                this.HandleThumb(
                    1, 0, -1, 0,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the right.
        private void HandleRight(object sender, DragDeltaEventArgs args)
        {
            if (!this.IsResizeEnabled || this.UniformScale)
                return;

            if (sender is CropThumb)
            {
                this.HandleThumb(
                    0, 0, 1, 0,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the bottom.
        private void HandleBottom(object sender, DragDeltaEventArgs args)
        {
            if (!this.IsResizeEnabled || this.UniformScale)
                return;

            if (sender is CropThumb)
            {
                this.HandleThumb(
                    0, 0, 0, 1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        private void HandleDrag(object sender, DragDeltaEventArgs args)
        {
            if (sender is DragThumb)
            {
                this.HandleThumb(
                    1, 1, 0, 0,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        


        private void AdornedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var element = sender as FrameworkElement;

            var adorners = AdornerLayer.GetAdornerLayer(element).GetAdorners(element);

            if (adorners == null || !adorners.Contains(this))
            {
                element.SizeChanged -= this.AdornedElement_SizeChanged;
                return;
            }

            //e.Handled = true;

            if(_isFirstSizeChange)
            {
                _isFirstSizeChange = false;
                return;
            }

            
            var interior = _cropMask.RectInterior;
            var interiorLeft = interior.Left;
            var interiorTop = interior.Top;
            var interiorWidth = interior.Width;
            var interiorHeight = interior.Height;
            var isCroppingResizeRequired = false;

            if (this.MaximumClippingWidth != null && this.MaximumClippingWidth > element.RenderSize.Width)
                this.MaximumClippingWidth = element.RenderSize.Width;

            if (this.MaximumClippingHeight != null && this.MaximumClippingHeight > element.RenderSize.Height)
                this.MaximumClippingHeight = element.RenderSize.Height;

            if (this.ScaleOnTargetResize)
            {
                var horizontalRatio = e.PreviousSize.Width == 0 ? 1.0 : e.NewSize.Width / e.PreviousSize.Width;
                var verticalRatio = e.PreviousSize.Height == 0 ? 1.0 : e.NewSize.Height / e.PreviousSize.Height;

                interiorLeft *= horizontalRatio;
                interiorTop *= verticalRatio;
                interiorWidth *= horizontalRatio;
                interiorHeight *= verticalRatio;

                isCroppingResizeRequired = true;
            }

            if (interiorLeft > element.RenderSize.Width)
            {
                interiorLeft = element.RenderSize.Width;
                interiorWidth = 0;
                isCroppingResizeRequired = true;
            }

            if (interiorTop > element.RenderSize.Height)
            {
                interiorTop = element.RenderSize.Height;
                interiorHeight = 0;
                isCroppingResizeRequired = true;
            }

            if (interiorLeft + interiorWidth > element.RenderSize.Width)
            {
                interiorWidth = Math.Max(0, element.RenderSize.Width - interiorLeft);
                isCroppingResizeRequired = true;
            }

            if (interiorTop + interiorHeight > element.RenderSize.Height)
            {
                interiorHeight = Math.Max(0, element.RenderSize.Height - interiorTop);
                isCroppingResizeRequired = true;
            }

            if (isCroppingResizeRequired)
            {
                _cropMask.RectInterior = new Rect(interiorLeft, interiorTop, interiorWidth, interiorHeight);
                this.SetThumbs(_cropMask.RectInterior);
            }
        }



        private void SetThumbs(Rect rc)
        {
            _bottomRightThumb.SetPos(rc.Right, rc.Bottom);
            _topLeftThumb.SetPos(rc.Left, rc.Top);
            _topRightThumb.SetPos(rc.Right, rc.Top);
            _bottomLeftThumb.SetPos(rc.Left, rc.Bottom);
            _topThumb.SetPos(rc.Left + rc.Width / 2, rc.Top);
            _bottomThumb.SetPos(rc.Left + rc.Width / 2, rc.Bottom);
            _leftThumb.SetPos(rc.Left, rc.Top + rc.Height / 2);
            _rightThumb.SetPos(rc.Right, rc.Top + rc.Height / 2);
            _dragThumb.SetPos(rc.Left, rc.Top);
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rcExterior = new Rect(0, 0, this.AdornedElement.RenderSize.Width, this.AdornedElement.RenderSize.Height);
            _cropMask.RectExterior = rcExterior;
            Rect rcInterior = _cropMask.RectInterior;
            _cropMask.Arrange(rcExterior);

            this.SetThumbs(rcInterior);
            _thumbsCanvas.Arrange(rcExterior);
            return finalSize;
        }



        public BitmapSource GetCropPreviewSource()
        {
            Thickness margin = this.GetAdornerMargin();
            Rect rcInterior = _cropMask.RectInterior;

            Point pxFromSize = this.UnitsToPixels(rcInterior.Width, rcInterior.Height);

            // It appears that CroppedBitmap indexes from the upper left of the margin whereas RenderTargetBitmap renders the
            // control exclusive of the margin.  Hence our need to take the margins into account here...

            Point pxFromPos = this.UnitsToPixels(rcInterior.Left + margin.Left, rcInterior.Top + margin.Top);
            Point pxWhole = this.UnitsToPixels(this.AdornedElement.RenderSize.Width + margin.Left, this.AdornedElement.RenderSize.Height + margin.Left);
            pxFromSize.X = Math.Max(Math.Min(pxWhole.X - pxFromPos.X, pxFromSize.X), 0);
            pxFromSize.Y = Math.Max(Math.Min(pxWhole.Y - pxFromPos.Y, pxFromSize.Y), 0);
            if (pxFromSize.X == 0 || pxFromSize.Y == 0)
            {
                return null;
            }
            Int32Rect rcFrom = new Int32Rect(pxFromPos.X, pxFromPos.Y, pxFromSize.X, pxFromSize.Y);

            RenderTargetBitmap rtb = new RenderTargetBitmap(pxWhole.X, pxWhole.Y, s_dpiX, s_dpiY, PixelFormats.Default);
            rtb.Render(this.AdornedElement);
            return new CroppedBitmap(rtb, rcFrom);
        }



        private Thickness GetAdornerMargin()
        {
            Thickness thick = new Thickness(0);
            if (this.AdornedElement is FrameworkElement)
            {
                thick = ((FrameworkElement)this.AdornedElement).Margin;
            }
            return thick;
        }

        private void BuildCorner(ref CropThumb thumb, Cursor cursor)
        {
            if (thumb != null) return;

            thumb = new CropThumb(_thumbWidth);

            // Set some arbitrary visual characteristics.
            thumb.Cursor = cursor;

            _thumbsCanvas.Children.Add(thumb);
        }


        private void BuildDragThumb(ref DragThumb thumb)
        {
            if (thumb != null) return;

            thumb = new DragThumb();

            _thumbsCanvas.Children.Add(thumb);
        }


        private Point UnitsToPixels(double x, double y)
        {
            return new Point((int)(x * s_dpiX / 96), (int)(y * s_dpiY / 96));
        }



        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return _visuals.Count; } }
        protected override Visual GetVisualChild(int index) { return _visuals[index]; }



        class CropThumb : Thumb
        {

            int _size;

            internal CropThumb(int size)
                : base()
            {
                _size = size;
            }

            protected override Visual GetVisualChild(int index)
            {
                return null;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawRoundedRectangle(Brushes.White, new Pen(Brushes.Black, 1), new Rect(new Size(_size, _size)), 1, 1);
            }

            internal void SetPos(double x, double y)
            {
                Canvas.SetTop(this, y - _size / 2);
                Canvas.SetLeft(this, x - _size / 2);
            }

        }

        class DragThumb : Thumb
        {
            private static BitmapSource s_moveImage = BitmapImageEx.LoadAsFrozen("Resources/Images/Move_16.png");

            private const double c_size = 16;

            public DragThumb()
            {
                this.Cursor = Cursors.ScrollAll;
            }

            protected override Visual GetVisualChild(int index)
            {
                return null;
            }


            protected override void OnRender(DrawingContext drawingContext)
            {
                var rect = new Rect(new Size(c_size, c_size));
                drawingContext.DrawImage(s_moveImage, rect);
            }

            internal void SetPos(double x, double y)
            {
                Canvas.SetTop(this, y - c_size - 2);
                Canvas.SetLeft(this, x - c_size - 2);
            }
        }

    }

}

