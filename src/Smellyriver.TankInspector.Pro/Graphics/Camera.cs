using System;
using System.Runtime.Serialization;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    [DataContract]
    public class Camera : NotificationObject
    {
        public event EventHandler PanningXChanged;

        [DataMember(Name = "PanningX")]
        private double _panningX;
        public double PanningX
        {
            get { return _panningX; }
            set
            {
                if (_panningX == value)
                    return;

                _panningX = value;
                this.RaisePropertyChanged(() => this.PanningX);

                if (this.PanningXChanged != null)
                    this.PanningXChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler PanningYChanged;

        [DataMember(Name = "PanningY")]
        private double _panningY;
        public double PanningY
        {
            get { return _panningY; }
            set
            {
                if (_panningY == value)
                    return;

                _panningY = value;
                this.RaisePropertyChanged(() => this.PanningY);

                if (this.PanningYChanged != null)
                    this.PanningYChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler PanningZChanged;

        [DataMember(Name = "PanningZ")]
        private double _panningZ;
        public double PanningZ
        {
            get { return _panningZ; }
            set
            {
                if (_panningZ == value)
                    return;

                _panningZ = value;
                this.RaisePropertyChanged(() => this.PanningZ);

                if (this.PanningZChanged != null)
                    this.PanningZChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler RotationYawChanged;

        [DataMember(Name = "RotationYaw")]
        private double _rotationYaw;
        public double RotationYaw
        {
            get { return _rotationYaw; }
            set
            {
                if (_rotationYaw == value)
                    return;

                _rotationYaw = value;
                this.RaisePropertyChanged(() => this.RotationYaw);

                if (this.RotationYawChanged != null)
                    this.RotationYawChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler RotationPitchChanged;

        [DataMember(Name = "RotationPitch")]
        private double _rotationPitch;
        public double RotationPitch
        {
            get { return _rotationPitch; }
            set
            {
                if (_rotationPitch == value)
                    return;

                _rotationPitch = value;
                this.RaisePropertyChanged(() => this.RotationPitch);

                if (this.RotationPitchChanged != null)
                    this.RotationPitchChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler RotationRollChanged;

        [DataMember(Name = "RotationRoll")]
        private double _rotationRoll;
        public double RotationRoll
        {
            get { return _rotationRoll; }
            set
            {
                if (_rotationRoll == value)
                    return;

                _rotationRoll = value;
                this.RaisePropertyChanged(() => this.RotationRoll);

                if (this.RotationRollChanged != null)
                    this.RotationRollChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler ZoomChanged;

        [DataMember(Name = "Zoom")]
        private double _zoom;
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (_zoom == value)
                    return;

                _zoom = value;
                this.RaisePropertyChanged(() => this.Zoom);

                if (this.ZoomChanged != null)
                    this.ZoomChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler ProjectionModeChanged;

        [DataMember(Name = "ProjectionMode")]
        private ProjectionMode _projectionMode;
        public ProjectionMode ProjectionMode
        {
            get { return _projectionMode; }
            set
            {
                if (_projectionMode == value)
                    return;

                _projectionMode = value;
                this.RaisePropertyChanged(() => this.ProjectionMode);

                if (this.ProjectionModeChanged != null)
                    this.ProjectionModeChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler FOVChanged;

        [DataMember(Name = "FOV")]
        private double _fov;
        public double FOV
        {
            get { return _fov; }
            set
            {
                if (_fov == value)
                    return;

                _fov = value;
                this.RaisePropertyChanged(() => this.FOV);

                if (this.FOVChanged != null)
                    this.FOVChanged(this, EventArgs.Empty);
            }
        }

        public Camera()
        {
            this.ProjectionMode = ProjectionMode.Perspective;
            this.FOV = 45;
            this.Zoom = 1.0;
        }
    }
}
