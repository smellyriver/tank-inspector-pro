using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Smellyriver.TankInspector.Pro.Graphics.Scene;

namespace Smellyriver.TankInspector.Pro.Graphics.Controls
{
    public partial class InputHandler : UserControl
    {
        private Trackball _trackball;
        private DateTime _lastTime;

        public Transform3D Transform
        {
            get { return (Transform3D)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform3D), typeof(InputHandler), new PropertyMetadata(Transform3D.Identity));




        public double Pitch
        {
            get { return (double)GetValue(PitchProperty); }
            set { SetValue(PitchProperty, value); }
        }

        public static readonly DependencyProperty PitchProperty =
            DependencyProperty.Register("Pitch", typeof(double), typeof(InputHandler), new PropertyMetadata(0.0, InputHandler.OnPitchChanged));

        private static void OnPitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InputHandler)d).OnPitchChanged((double)e.NewValue);
        }




        public double Yaw
        {
            get { return (double)GetValue(YawProperty); }
            set { SetValue(YawProperty, value); }
        }

        public static readonly DependencyProperty YawProperty =
            DependencyProperty.Register("Yaw", typeof(double), typeof(InputHandler), new PropertyMetadata(0.0, InputHandler.OnYawChanged));

        private static void OnYawChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InputHandler)d).OnYawChanged((double)e.NewValue);
        }


        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(InputHandler), new PropertyMetadata(1.0, InputHandler.OnZoomChanged));

        private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InputHandler)d).OnZoomChanged((double)e.NewValue);
        }



        public CameraMode CameraMode
        {
            get { return (CameraMode)GetValue(CameraModeProperty); }
            set { SetValue(CameraModeProperty, value); }
        }

        public static readonly DependencyProperty CameraModeProperty =
            DependencyProperty.Register("CameraMode", typeof(CameraMode), typeof(InputHandler), new PropertyMetadata(CameraMode.TrackBall,
                (d, e) => ((InputHandler)d).OnCameraModeChanged((CameraMode)e.OldValue, (CameraMode)e.NewValue)));

        public Vector3D LookDirection
        {
            get { return (Vector3D)GetValue(LookDirectionProperty); }
            set { SetValue(LookDirectionProperty, value); }
        }

        public static readonly DependencyProperty LookDirectionProperty =
            DependencyProperty.Register("LookDirection", typeof(Vector3D), typeof(InputHandler), new PropertyMetadata(new Vector3D(0, 0, 1)));

        private Point _lastPoint;

        private bool _retrievingTrackballData;

        public InputHandler()
        {
            InitializeComponent();
            _trackball = new Trackball();
        }

        private void OnPitchChanged(double pitch)
        {
            if (_retrievingTrackballData)
                return;

            _trackball.PitchFactor = pitch;
        }

        private void OnYawChanged(double yaw)
        {
            if (_retrievingTrackballData)
                return;

            _trackball.YawFactor = yaw;
        }

        private void OnZoomChanged(double zoom)
        {
            if (_retrievingTrackballData)
                return;

            _trackball.ZoomFactor = zoom;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            var thisTime = DateTime.Now;
            _trackball.UpdateInertia((thisTime - _lastTime).TotalSeconds);
            _lastTime = thisTime;

            _retrievingTrackballData = true;
            this.Transform = _trackball.Transform;
            this.LookDirection = _trackball.LookDirection;
            this.Yaw = this.NormalizeAngle(_trackball.YawFactor);
            this.Pitch = this.NormalizeAngle(_trackball.PitchFactor);
            

            if (CameraMode == CameraMode.Sniper)
            {
                this.Zoom = _trackball.ZoomFactor * 2;
            }
            else
            {
                this.Zoom = _trackball.ZoomFactor;
            }

            _retrievingTrackballData = false;
        }

        private double NormalizeAngle(double angle)
        {
            angle = (angle + 180) % 360 - 180;

            while (angle < -180)
                angle += 360;

            while (angle > 180)
                angle -= 360;

            return angle;
        }

        private void CaptureBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CameraMode == CameraMode.TrackBall)
            {
                Mouse.Capture(this.CaptureBorder, CaptureMode.Element);
                this.CaptureBorder.MouseMove += CaptureBorder_MouseMove;
                _trackball.TrackStart(e.GetPosition(this.CaptureBorder), this.CaptureBorder.ActualWidth, this.CaptureBorder.ActualHeight);
            }
        }

        private void CaptureBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CameraMode == CameraMode.TrackBall)
            {
                Mouse.Capture(this.CaptureBorder, CaptureMode.None);
                this.CaptureBorder.MouseMove -= CaptureBorder_MouseMove;
                _trackball.TrackEnd();
            }
        }

        private void CaptureBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (CameraMode == CameraMode.TrackBall)
            {
                _trackball.Track(e.GetPosition(this.CaptureBorder), this.CaptureBorder.ActualWidth, this.CaptureBorder.ActualHeight);
            }
            else if (CameraMode == CameraMode.Sniper)
            {
                var point = e.GetPosition(this.CaptureBorder);

                if (!_isfirstSniperFrame)
                {
                    _trackball.TrackLook(new Point(point.X - _lastPoint.X, point.Y - _lastPoint.Y));
                }
                else
                {
                    _isfirstSniperFrame = false;
                }

                _lastPoint = point;

                var center = PointToScreen(new Point(this.CaptureBorder.ActualWidth * 0.5, this.CaptureBorder.ActualHeight * 0.5));

                int x = (int)center.X;
                int y = (int)center.Y;

                var pointToScreen = PointToScreen(point);
                var dx = pointToScreen.X - center.X;
                var dy = pointToScreen.Y - center.Y;

                if (Math.Abs(dx) > 100 || Math.Abs(dy) > 100)
                {
                    InputHandler.SetCursorPos(x, y);
                    _lastPoint.X -= dx;
                    _lastPoint.Y -= dy;
                }
            }
        }

        private void OnCameraModeChanged(CameraMode cameraMode1, CameraMode newMode)
        {
            if (newMode == CameraMode.Sniper)
            {
                _lastPoint = Mouse.GetPosition(this.CaptureBorder);
                _isfirstSniperFrame = true;
            }
        }

        private void CaptureBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            _trackball.Zoom(e.Delta);

        }

        /// <summary>   
        /// 设置鼠标的坐标   
        /// </summary>   
        /// <param name="x">横坐标</param>   
        /// <param name="y">纵坐标</param>   
        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);


        public bool _isfirstSniperFrame { get; set; }
    }
}
