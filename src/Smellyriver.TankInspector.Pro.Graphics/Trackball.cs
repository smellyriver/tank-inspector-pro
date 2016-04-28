//---------------------------------------------------------------------------
//
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Limited Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx
// All other rights reserved.
//
// This file is part of the 3D Tools for Windows Presentation Foundation
// project.  For more information, see:
// 
// http://CodePlex.com/Wiki/View.aspx?ProjectName=3DTools
//
// The following article discusses the mechanics behind this
// trackball implementation: http://viewport3d.com/trackball.htm
//
// Reading the article is not required to use this sample code,
// but skimming it might be useful.
//
//---------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media.Media3D;
using Smellyriver.TankInspector.Common.Wpf;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    /// <summary>
    ///     Trackball is a utility class which observes the mouse events
    ///     on a specified FrameworkElement and produces a Transform3D
    ///     with the resultant rotation and scale.
    /// 
    ///     Example Usage:
    /// 
    ///         Trackball trackball = new Trackball();
    ///         trackball.EventSource = myElement;
    ///         myViewport3D.Camera.Transform = trackball.Transform;
    /// 
    ///     Because Viewport3Ds only raise events when the mouse is over the
    ///     rendered 3D geometry (as opposed to not when the mouse is within
    ///     the layout bounds) you usually want to use another element as 
    ///     your EventSource.  For example, a transparent border placed on
    ///     top of your Viewport3D works well:
    ///     
    ///         <Grid>
    ///           <ColumnDefinition />
    ///           <RowDefinition />
    ///           <Viewport3D Name="myViewport" ClipToBounds="True" Grid.Row="0" Grid.Column="0" />
    ///           <Border Name="myElement" Background="Transparent" Grid.Row="0" Grid.Column="0" />
    ///         </Grid>
    ///     
    ///     NOTE: The Transform property may be shared by multiple Cameras
    ///           if you want to have auxilary views following the trackball.
    /// 
    ///           It can also be useful to share the Transform property with
    ///           models in the scene that you want to move with the camera.
    ///           (For example, the Trackport3D's headlight is implemented
    ///           this way.)
    /// 
    ///           You may also use a Transform3DGroup to combine the
    ///           Transform property with additional Transforms.
    /// </summary> 
    public class Trackball : DependencyNotificationObject
    {
        //private FrameworkElement _eventSource;
        private Point _previousPosition2D;
        private Vector _vPreviousPosition = new Vector(0, 1);
        private Vector _hPreviousPosition = new Vector(0, 1);

        private Transform3DGroup _transform;
        private AxisAngleRotation3D _yawRotation = new AxisAngleRotation3D();
        private AxisAngleRotation3D _pitchRotation = new AxisAngleRotation3D();

        private InertiaValue _yawInertia = new InertiaValue(-145.31973875630851, double.MaxValue, double.MinValue, 1, 4.0, double.MaxValue);
        private InertiaValue _pitchInertia = new InertiaValue(-37.364639593462712, double.MaxValue, double.MinValue, 2, 6.0, double.MaxValue);
        private InertiaValue _zoomInertia = new InertiaValue(1.4, 2, 0.5, 10, 0.5, 2);


        private Vector3D _lookDirection = new Vector3D(0, 0, 1);


        public class InertiaValue
        {
            private double _inertia;
            private double _max;
            private double _min;
            private double _impulse;
            private double _damping;
            private double _maxImpulse;

            private double _value;
            public double Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    _impulse = 0;
                }
            }

            public InertiaValue(double initial, double max, double min, double inertia, double damping, double maxImpulse)
            {
                Value = initial;
                _max = max;
                _min = min;
                _inertia = Math.Abs(inertia);
                _damping = Math.Abs(damping);
                _maxImpulse = Math.Abs(maxImpulse);
            }

            public EventHandler ValueChanged;

            public void UpdateInertia(double delta)
            {
                if (_impulse > 0)
                {
                    _impulse -= delta * _damping / _inertia;
                    if (_impulse < 0)
                    {
                        _impulse = 0;
                        return;
                    }
                }
                else if (_impulse < 0)
                {
                    _impulse += delta * _damping / _inertia;
                    if (_impulse > 0)
                    {
                        _impulse = 0;
                        return;
                    }
                }

                _value += _impulse;

                if (_value > _max)
                {
                    _value = _max;
                    _impulse = 0;
                }
                else if (_value < _min)
                {
                    _value = _min;
                    _impulse = 0;
                }


                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs());
                }
            }

            public void Push(double impulse)
            {
                _impulse = impulse / _inertia;
                if (_impulse > _maxImpulse)
                    _impulse = _maxImpulse;
                else if (_impulse < -_maxImpulse)
                    _impulse = -_maxImpulse;
                UpdateInertia(0.0);
            }
        }


        private bool _hasTrackStarted;

        public Trackball()
        {
            _yawRotation.Axis = new Vector3D(0, 1, 0);
            _pitchRotation.Axis = new Vector3D(1, 0, 0);
            _transform = new Transform3DGroup();
            _transform.Children.Add(new RotateTransform3D(_yawRotation));
            _transform.Children.Add(new RotateTransform3D(_pitchRotation));
            _yawInertia.ValueChanged = Yaw;
            _pitchInertia.ValueChanged = Pitch;
        }

        public void UpdateInertia(double delta)
        {
            _yawInertia.UpdateInertia(delta);
            _pitchInertia.UpdateInertia(delta);
            _zoomInertia.UpdateInertia(delta);
        }

        /// <summary>
        ///     A transform to move the camera or scene to the trackball's
        ///     current orientation and scale.
        /// </summary>
        public Transform3D Transform
        {
            get { return _transform; }
        }

        public double PitchFactor
        {
            get { return _pitchInertia.Value; }
            set { _pitchInertia.Value = value; }
        }

        public double YawFactor
        {
            get { return _yawInertia.Value; }
            set { _yawInertia.Value = value; }
        }

        public double ZoomFactor
        {
            get { return _zoomInertia.Value; }
            set { _zoomInertia.Value = value; }
        }

        public Vector3D LookDirection
        {
            get { return _lookDirection; }
        }

        private Vector ProjectToRound(double width, double x)
        {
            if (x < 0) x = 0;
            if (x > width) x = width;

            x = x / (width / 2);

            x = x - 1;

            double z = 1 - x * x;

            return new Vector(x, z);
        }

        public void TrackStart(Point trackPosition, double trackWidth, double trackHeight)
        {
            _previousPosition2D = trackPosition;
            _hPreviousPosition = ProjectToRound(
                trackWidth,
                _previousPosition2D.X);

            _vPreviousPosition = ProjectToRound(
              trackHeight,
             _previousPosition2D.Y);

            _hasTrackStarted = true;
        }

        public void TrackEnd()
        {
            _hasTrackStarted = false;
        }

        public void Zoom(int delta)
        {
            _zoomInertia.Push((double)(delta) / 500);
        }

        public void Track(Point currentPosition, double width, double height)
        {
            if (_hasTrackStarted)
            {
                var hCurrentPosition = ProjectToRound(width, currentPosition.X);

                double hangle = Vector.AngleBetween(_hPreviousPosition, hCurrentPosition);

                _yawInertia.Push(hangle);

                _hPreviousPosition = hCurrentPosition;

                var vCurrentPosition = ProjectToRound(height, currentPosition.Y);

                double vangle = Vector.AngleBetween(_vPreviousPosition, vCurrentPosition);

                _pitchInertia.Push(vangle);

                _vPreviousPosition = vCurrentPosition;
            }

            _previousPosition2D = currentPosition;
        }

        public void TrackLook(Point delta)
        {
            double hangle = delta.X / 80.0;
            double vangle = delta.Y / 80.0;
            _lookDirection.X = hangle;
            _lookDirection.Y = vangle;
            _lookDirection.Z = _lookDirection.Z + 0.1;
        }

        private void Yaw(object sender, EventArgs e)
        {
            _yawRotation.Angle = _yawInertia.Value;
        }

        private void Pitch(object sender, EventArgs e)
        {
            _pitchRotation.Angle = _pitchInertia.Value;
        }
    }
}
