using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.TurretController
{
    /// <summary>
    /// Interaction logic for TurretController.xaml
    /// </summary>
    public partial class TurretControl : UserControl
    {

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

        internal TurretYawLimits TurretYawLimits
        {
            get { return (TurretYawLimits)GetValue(TurretYawLimitsProperty); }
            set { SetValue(TurretYawLimitsProperty, value); }
        }

        public static readonly DependencyProperty TurretYawLimitsProperty =
            DependencyProperty.Register("TurretYawLimits", typeof(TurretYawLimits), typeof(TurretControl), new PropertyMetadata(null, TurretControl.OnTraverseChanged));

        internal GunPitchLimitsComponent ElevationLimits
        {
            get { return (GunPitchLimitsComponent)GetValue(ElevationLimitsProperty); }
            set { SetValue(ElevationLimitsProperty, value); }
        }

        public static readonly DependencyProperty ElevationLimitsProperty =
            DependencyProperty.Register("ElevationLimits", typeof(GunPitchLimitsComponent), typeof(TurretControl), new PropertyMetadata(null, TurretControl.OnTraverseChanged));

        internal GunPitchLimitsComponent DepressionLimits
        {
            get { return (GunPitchLimitsComponent)GetValue(DepressionLimitsProperty); }
            set { SetValue(DepressionLimitsProperty, value); }
        }

        public static readonly DependencyProperty DepressionLimitsProperty =
            DependencyProperty.Register("DepressionLimits", typeof(GunPitchLimitsComponent), typeof(TurretControl), new PropertyMetadata(null, TurretControl.OnTraverseChanged));


        public double TurretTraverseSpeed
        {
            get { return (double)GetValue(TurretTraverseSpeedProperty); }
            set { SetValue(TurretTraverseSpeedProperty, value); }
        }

        public static readonly DependencyProperty TurretTraverseSpeedProperty =
            DependencyProperty.Register("TurretTraverseSpeed", typeof(double), typeof(TurretControl), new PropertyMetadata(30.0));



        public double GunTraverseSpeed
        {
            get { return (double)GetValue(GunTraverseSpeedProperty); }
            set { SetValue(GunTraverseSpeedProperty, value); }
        }

        public static readonly DependencyProperty GunTraverseSpeedProperty =
            DependencyProperty.Register("GunTraverseSpeed", typeof(double), typeof(TurretControl), new PropertyMetadata(30.0));




        public bool UseRealTraverseMode
        {
            get { return (bool)GetValue(UseRealTraverseModeProperty); }
            set { SetValue(UseRealTraverseModeProperty, value); }
        }

        public static readonly DependencyProperty UseRealTraverseModeProperty =
            DependencyProperty.Register("UseRealTraverseMode", typeof(bool), typeof(TurretControl), new PropertyMetadata(true, TurretControl.OnUseRealTraverseModeChanged));

        private static void OnUseRealTraverseModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue != (bool)e.OldValue)
                ((TurretControl)d).OnUseRealTraverseModeChanged();
        }

        public double VehicleYaw
        {
            get { return (double)GetValue(VehicleYawProperty); }
            set { SetValue(VehicleYawProperty, value); }
        }

        public static readonly DependencyProperty VehicleYawProperty =
            DependencyProperty.Register("VehicleYaw", typeof(double), typeof(UserControl), new PropertyMetadata(0.0, TurretControl.OnVehicleYawChanged));

        private static void OnVehicleYawChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TurretControl)d).UpdateIndicators();
        }

        public TankFigureType TankFigureType
        {
            get { return (TankFigureType)GetValue(TankFigureTypeProperty); }
            set { SetValue(TankFigureTypeProperty, value); }
        }

        public static readonly DependencyProperty TankFigureTypeProperty =
            DependencyProperty.Register("TankFigureType", typeof(TankFigureType), typeof(TurretControl), new PropertyMetadata(TankFigureType.Tank, TurretControl.OnTankFigureTypeChanged));

        private static void OnTankFigureTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TurretControl)d).UpdateTankFigure();
        }

        private static readonly DependencyProperty GunPitchProperty
            = DependencyProperty.Register("GunPitch", typeof(double), typeof(TurretControl), new FrameworkPropertyMetadata(0.0, TurretControl.OnGunPitchChanged));

        public double GunPitch
        {
            get { return (double)GetValue(GunPitchProperty); }
            set { SetValue(GunPitchProperty, value); }
        }

        public static readonly DependencyProperty TurretYawProperty
            = DependencyProperty.Register("TurretYaw", typeof(double), typeof(TurretControl), new FrameworkPropertyMetadata(0.0, TurretControl.OnTurretYawChanged));

        private static void OnTurretYawChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TurretControl)d;
            control._destinationTurretYaw = control.TurretYaw;
            control.UpdateIndicators();
        }

        private static void OnGunPitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TurretControl)d;
            control._destinationGunPitch = control.GunPitch;
            control.UpdateIndicators();
        }

        public double TurretYaw
        {
            get { return (double)GetValue(TurretYawProperty); }
            set { SetValue(TurretYawProperty, value); }
        }

        private static void OnTraverseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TurretControl)d).OnTraverseChanged();
        }


        private double _previousGunPitch;
        private double _previousTurretYaw;

        private double _destinationGunPitch;
        private double _destinationTurretYaw;

        private DateTime _previousUpdateTime;

        private bool _internalSettingTraverse;


        Func<double, double> _elevationRadiusConverter;
        Func<double, double> _depressionRadiusConverter;
        Func<double, double> _inverseVerticalTraverseConverter;

        private bool _isMouseInside = false;

        public TurretControl()
        {
            InitializeComponent();
            this.VehicleYaw = 90;
            this.BoundaryCanvas.SizeChanged += BoundaryCanvas_SizeChanged;
            this.UpdateTankFigure();
            this.OnUseRealTraverseModeChanged();
            this.Loaded += TurretControl_Loaded;
        }

        void TurretControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateIndicators();
        }

        private void UpdateTankFigure()
        {
            switch (this.TankFigureType)
            {
                case TankFigureType.Tank:
                    this.HullFigure.Source = BitmapImageEx.LoadAsFrozen("Resources/Images/Figures/TankHull.png");
                    this.TurretFigure.Source = BitmapImageEx.LoadAsFrozen("Resources/Images/Figures/TankTurret.png");
                    break;
                case TankFigureType.TankDestroyer:
                    this.HullFigure.Source = BitmapImageEx.LoadAsFrozen("Resources/Images/Figures/TDHull.png");
                    this.TurretFigure.Source = BitmapImageEx.LoadAsFrozen("Resources/Images/Figures/TDGun.png");
                    break;
                case TankFigureType.SelfPropelledGun:
                    this.HullFigure.Source = BitmapImageEx.LoadAsFrozen("Resources/Images/Figures/SPGHull.png");
                    this.TurretFigure.Source = BitmapImageEx.LoadAsFrozen("Resources/Images/Figures/SPGGun.png");
                    break;
                default:
                    throw new ArgumentException();
            }

        }

        void BoundaryCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateTraverseBoundary();
        }

        private void OnUseRealTraverseModeChanged()
        {
            if (this.UseRealTraverseMode)
            {
                CompositionTarget.Rendering += UpdateTraverse;
                _previousUpdateTime = DateTime.Now;
            }
            else
            {
                CompositionTarget.Rendering -= UpdateTraverse;
                this.UpdateTraverse();
            }
        }

        private void UpdateTraverse(object sender, EventArgs e)
        {
            this.UpdateTraverse();
        }

        private void UpdateTraverse()
        {
            if (UseRealTraverseMode)
            {
                var now = DateTime.Now;
                var deltaTime = now - _previousUpdateTime;
                var turretYaw = GunTraverseHelper.RotateAngle(
                    this.TurretYaw,
                    _destinationTurretYaw,
                    deltaTime.TotalSeconds * this.TurretTraverseSpeed,
                    this.TurretYawLimits == null ? 180.0 : this.TurretYawLimits.Right,
                    this.TurretYawLimits == null ? 180.0 : this.TurretYawLimits.Left);

                double elevation, depression;
                if (this.ElevationLimits == null)
                {
                    elevation = 0;
                    depression = 0;
                }
                else
                {
                    elevation = this.ElevationLimits.GetValue(turretYaw);
                    depression = this.DepressionLimits.GetValue(turretYaw);
                }

                var gunPitch = GunTraverseHelper.RotateAngle(
                    this.GunPitch + 180,
                    _destinationGunPitch + 180,
                    deltaTime.TotalSeconds * this.GunTraverseSpeed,
                    elevation,
                    depression) - 180;

                this.ClampRotation(ref turretYaw, ref gunPitch);

                _internalSettingTraverse = true;
                this.TurretYaw = turretYaw;
                this.GunPitch = gunPitch;
                _internalSettingTraverse = false;

                _previousUpdateTime = now;
            }
            else
            {
                _internalSettingTraverse = true;
                this.GunPitch = _destinationGunPitch;
                this.TurretYaw = _destinationTurretYaw;
                _internalSettingTraverse = false;
            }
        }


        private void OnTraverseChanged()
        {
            if (this.ElevationLimits == null || this.DepressionLimits == null)
                return;


            this.UpdateTraverseBoundary();
            this.SetRotation(_destinationTurretYaw, _destinationGunPitch, true); // limit the rotation within new traverse boundary

            if (this.TurretYawLimits == null || this.TurretYawLimits.Range >= 180)
                this.TankFigureType = TankFigureType.Tank;
            else
            {
                if (this.ElevationLimits != null && this.ElevationLimits.GetMaxValue() >= 45)
                    this.TankFigureType = TankFigureType.SelfPropelledGun;
                else
                    this.TankFigureType = TankFigureType.TankDestroyer;
            }
        }

        private void GetDimentions(out double size, out double radius, out Point center)
        {
            size = Math.Min(this.Root.ActualWidth, this.Root.ActualHeight);
            radius = size / 2;
            center = new Point(radius, radius);
        }

        private void UpdateTraverseBoundary()
        {
            if (this.ElevationLimits == null)
            {
                this.BoundaryCanvas.Children.Clear();
                return;
            }

            double size;
            double radius;
            Point center;
            this.GetDimentions(out size, out radius, out center);
            if (size == 0)
                return;

            var oneThirdsRadius = radius / 3;

            var maxElevation = this.ElevationLimits.GetMaxValue();
            var maxDepression = this.DepressionLimits.GetMaxValue();
            var maxTraverse = -maxElevation + maxDepression;

            // elevation/outer figure

            _elevationRadiusConverter = r => (-r + maxDepression) / maxTraverse * oneThirdsRadius * 2 + oneThirdsRadius;
            _depressionRadiusConverter = r => (maxDepression - r) / maxTraverse * oneThirdsRadius * 2 + oneThirdsRadius;
            _inverseVerticalTraverseConverter = r => -((r - oneThirdsRadius) / (oneThirdsRadius * 2) * maxTraverse - maxDepression);

            var elevationGeometry = GunTraverseHelper.CreateGeometry(this.ElevationLimits, this.TurretYawLimits, center, _elevationRadiusConverter);
            var depressionGeometry = GunTraverseHelper.CreateGeometry(this.DepressionLimits, this.TurretYawLimits, center, _depressionRadiusConverter);

            var combinedGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, elevationGeometry, depressionGeometry);

            var figure = new Path { Data = combinedGeometry };

            var borderStyle = this.FindResource("Border") as Style;
            var delimiterStyle = this.FindResource("Delimiter") as Style;

            figure.Style = borderStyle;

            var delimiterCircle = new Ellipse();
            var delimiterCircleSize = _depressionRadiusConverter(0) * 2;
            delimiterCircle.Width = delimiterCircleSize;
            delimiterCircle.Height = delimiterCircleSize;
            delimiterCircle.Style = delimiterStyle;

            Canvas.SetLeft(delimiterCircle, (size - delimiterCircleSize) / 2);
            Canvas.SetTop(delimiterCircle, (size - delimiterCircleSize) / 2);

            this.BoundaryCanvas.Children.Clear();

            this.BoundaryCanvas.Children.Add(delimiterCircle);
            this.BoundaryCanvas.Children.Add(figure);

            Canvas.SetLeft(this.YawDirectionLine, center.X);
            Canvas.SetTop(this.YawDirectionLine, center.Y);
        }

        private void UpdateRotation()
        {
            var position = Mouse.GetPosition(this.Root);
            double size;
            double radius;
            Point center;
            this.GetDimentions(out size, out radius, out center);
            double distance, degree;
            GunTraverseHelper.CartesianToPolar(center, position, out distance, out degree);
            degree -= this.VehicleYaw;
            var turretYaw = GunTraverseHelper.NormalizeAngle(degree, -180);
            var gunPitch = _inverseVerticalTraverseConverter(distance);

            this.SetRotation(turretYaw, gunPitch);
        }

        private void ClampRotation(ref double turretYaw, ref double gunPitch)
        {
            if (this.TurretYawLimits != null)
            {
                if (turretYaw < 0)
                    turretYaw = Math.Max(turretYaw, this.TurretYawLimits.Left);
                else
                    turretYaw = Math.Min(turretYaw, this.TurretYawLimits.Right);
            }

            if (this.ElevationLimits != null)
                gunPitch = gunPitch.Clamp(this.ElevationLimits.GetValue(turretYaw),
                                          this.DepressionLimits.GetValue(turretYaw));
        }

        private void SetRotation(double turretYaw, double gunPitch, bool overrideRealTraverse = false)
        {
            this.ClampRotation(ref turretYaw, ref gunPitch);

            _destinationTurretYaw = turretYaw;
            _destinationGunPitch = gunPitch;

            if (overrideRealTraverse)
            {
                _internalSettingTraverse = true;
                this.TurretYaw = _destinationTurretYaw;
                this.GunPitch = _destinationGunPitch;
                _internalSettingTraverse = false;
            }
            else if (!UseRealTraverseMode)
                this.UpdateTraverse();

            // otherwise it will be automatically updated by the timer
        }


        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.SetRotation(_previousTurretYaw, _previousGunPitch);

            _isMouseInside = false;
        }

        private void TankFigure_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_isMouseInside && this.UseRealTraverseMode)
            {
                this.SyncGunPointers();
                _isMouseInside = true;
            }
        }

        private void SyncGunPointers()
        {
            _previousGunPitch = _destinationGunPitch;
            _previousTurretYaw = _destinationTurretYaw;
            this.SetGunPointerPos(this.OriginalGunPointer, _previousGunPitch, _previousTurretYaw);
        }

        private void UpdateIndicators()
        {
            double _;
            Point center;
            this.GetDimentions(out _, out _, out center);

            this.SetGunPointerPos(this.GunPointer, center, _destinationGunPitch, _destinationTurretYaw);
            if (!_internalSettingTraverse)
                this.SyncGunPointers();
            this.SetGunPointerPos(this.OriginalGunPointer, center, _destinationGunPitch, _destinationTurretYaw);

            this.GunFigureRotateTransform.Angle = this.TurretYaw + this.VehicleYaw;
        }

        private void SetGunPointerPos(FrameworkElement gunPointer, double pitch, double yaw)
        {
            double _;
            Point center;
            this.GetDimentions(out _, out _, out center);
            this.SetGunPointerPos(gunPointer, center, pitch, yaw);
        }


        private Point CalculateGunPointerPos(Point center, double pitch, double yaw)
        {
            if (_elevationRadiusConverter == null)
                return new Point();

            var distance = this.GunPitch < 0 ? _elevationRadiusConverter(pitch) : _depressionRadiusConverter(pitch);

            return GunTraverseHelper.PolarToCartesian(center, distance, -yaw - this.VehicleYaw);
        }

        private void SetGunPointerPos(FrameworkElement gunPointer, Point center, double pitch, double yaw)
        {
            var gunPointerPosition = this.CalculateGunPointerPos(center, pitch, yaw);
            Canvas.SetLeft(gunPointer, gunPointerPosition.X - gunPointer.ActualWidth / 2);
            Canvas.SetTop(gunPointer, gunPointerPosition.Y - gunPointer.ActualHeight / 2);
        }

        private void HitTestPlaceHolder_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                this.UpdateRotation();
        }

        private void HitTestPlaceHolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.SyncGunPointers();

            this.HitTestPlaceHolder.CaptureMouse();
        }

        private void HitTestPlaceHolder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.HitTestPlaceHolder.IsMouseCaptured)
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    this.SetRotation(0, 0);
                }

                this.HitTestPlaceHolder.ReleaseMouseCapture();
            }
        }

        private void HitTestPlaceHolder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var delta = e.Delta / 120.0;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                delta *= 360;

            var gunPitch = _destinationGunPitch + delta;
            this.ClampRotation(ref _destinationTurretYaw, ref gunPitch);

            double _;
            Point center;
            this.GetDimentions(out _, out _, out center);

            var destinationGunPointerPos = this.CalculateGunPointerPos(center, gunPitch, _destinationTurretYaw);
            var cursorPos = this.DynamicObjectContainer.PointToScreen(destinationGunPointerPos);
            TurretControl.SetCursorPos((int)Math.Round(cursorPos.X), (int)Math.Round(cursorPos.Y));
        }

    }
}
