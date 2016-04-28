using System;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Pro.Graphics;

namespace Smellyriver.TankInspector.Pro.CameraController
{
    public class CameraControllerVM : NotificationObject
    {
        private Camera _camera;
        public Camera Camera
        {
            get { return _camera; }
            set
            {
                if (_camera != null)
                {
                    _camera.RotationPitchChanged -= Camera_RotationPitchChanged;
                    _camera.RotationYawChanged -= Camera_RotationYawChanged;
                    _camera.RotationRollChanged -= Camera_RotationRollChanged;
                    _camera.PanningXChanged -= Camera_PanningXChanged;
                    _camera.PanningYChanged -= Camera_PanningYChanged;
                    _camera.PanningZChanged -= Camera_PanningZChanged;
                    _camera.FOVChanged -= Camera_FOVChanged;
                    _camera.ProjectionModeChanged -= Camera_ProjectionModeChanged;
                    _camera.ZoomChanged -= Camera_ZoomChanged;
                }
                _camera = value;
                if (_camera != null)
                {
                    _camera.RotationPitchChanged += Camera_RotationPitchChanged;
                    _camera.RotationYawChanged += Camera_RotationYawChanged;
                    _camera.RotationRollChanged += Camera_RotationRollChanged;
                    _camera.PanningXChanged += Camera_PanningXChanged;
                    _camera.PanningYChanged += Camera_PanningYChanged;
                    _camera.PanningZChanged += Camera_PanningZChanged;
                    _camera.FOVChanged += Camera_FOVChanged;
                    _camera.ProjectionModeChanged += Camera_ProjectionModeChanged;
                    _camera.ZoomChanged += Camera_ZoomChanged;
                }
                this.OnCameraChanged();
            }
        }




        private bool _settingProjectionMode;
        public bool IsOrthographicModeSelected
        {
            get { return this.Camera != null && this.Camera.ProjectionMode == ProjectionMode.Orthographic; }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                _settingProjectionMode = true;
                this.Camera.ProjectionMode = ProjectionMode.Orthographic;
                this.NotifyProjectionModePropertiesChanged();
                _settingProjectionMode = false;
            }
        }

        public bool IsPerspectiveModeSelected
        {
            get { return this.Camera != null && this.Camera.ProjectionMode == ProjectionMode.Perspective; }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                _settingProjectionMode = true;
                this.Camera.ProjectionMode = ProjectionMode.Perspective;
                this.NotifyProjectionModePropertiesChanged();
                _settingProjectionMode = false;
            }
        }

        private bool _settingFov;
        public double FOV
        {
            get { return this.Camera?.FOV ?? double.NaN; }
            set
            {
                _settingFov = true;
                this.Camera.FOV = value.Clamp(6.9, 132.1);
                this.RaisePropertyChanged(() => this.FOV);
                _equivalent135FocalLength = 18 / Math.Tan(this.Camera.FOV * Math.PI / 360);
                this.RaisePropertyChanged(() => this.Equivalent135FocalLength);
                _settingFov = false;
            }
        }

        private double _equivalent135FocalLength;
        public double Equivalent135FocalLength
        {
            get { return this.Camera == null ? double.NaN : _equivalent135FocalLength; }
            set
            {
                _settingFov = true;
                _equivalent135FocalLength = value.Clamp(8, 300);
                this.RaisePropertyChanged(() => this.Equivalent135FocalLength);
                this.FOV = Math.Atan(18 / _equivalent135FocalLength) * 360 / Math.PI;
                this.RaisePropertyChanged(() => this.FOV);
                _settingFov = false;
            }
        }

        private bool _settingRotationYaw;
        public double RotationYaw
        {
            get { return this.Camera?.RotationYaw ?? double.NaN; }
            set
            {
                _settingRotationYaw = true;
                this.Camera.RotationYaw = value;
                this.RaisePropertyChanged(() => this.RotationRoll);
                this.NotifyLocationRelatedPropertiesChanged();
                _settingRotationYaw = false;
            }
        }

        private bool _settingRotationPitch;
        public double RotationPitch
        {
            get { return this.Camera?.RotationPitch ?? double.NaN; }
            set
            {
                _settingRotationPitch = true;
                this.Camera.RotationPitch = value;
                this.RaisePropertyChanged(() => this.RotationPitch);
                this.NotifyLocationRelatedPropertiesChanged();
                _settingRotationPitch = false;
            }
        }

        private bool _settingRotationRoll;
        public double RotationRoll
        {
            get { return this.Camera?.RotationRoll ?? double.NaN; }
            set
            {
                _settingRotationRoll = true;
                this.Camera.RotationRoll = value;
                this.RaisePropertyChanged(() => this.RotationYaw);
                this.NotifyLocationRelatedPropertiesChanged();
                _settingRotationRoll = false;
            }
        }

        private bool _settingPanningX;
        public double PanningX
        {
            get { return this.Camera?.PanningX ?? double.NaN; }
            set
            {
                _settingPanningX = true;
                this.Camera.PanningX = value;
                this.RaisePropertyChanged(() => this.PanningX);
                this.NotifyLocationRelatedPropertiesChanged();
                _settingPanningX = false;
            }
        }

        private bool _settingPanningY;
        public double PanningY
        {
            get { return this.Camera?.PanningY ?? double.NaN; }
            set
            {
                _settingPanningY = true;
                this.Camera.PanningY = value;
                this.RaisePropertyChanged(() => this.PanningY);
                this.NotifyLocationRelatedPropertiesChanged();
                _settingPanningY = false;
            }
        }

        private bool _settingPanningZ;

        public double PanningZ
        {
            get { return this.Camera?.PanningZ ?? double.NaN; }
            set
            {
                _settingPanningZ = true;
                this.Camera.PanningZ = value;
                this.RaisePropertyChanged(() => this.PanningZ);
                this.NotifyLocationRelatedPropertiesChanged();
                _settingPanningZ = false;
            }
        }

        private bool IsPanned
        {
            get
            {
                return Math.Abs(this.PanningX) > .001
                    || Math.Abs(this.PanningY) > .001
                    || Math.Abs(this.PanningZ) > .001;
            }
        }

        public bool IsFrontViewSelected
        {
            get
            {
                return Math.Abs(this.RotationPitch) < .001
                    && Math.Abs(this.RotationRoll) < .001
                    && Math.Abs(this.RotationYaw - (-180)) < .001
                    && !this.IsPanned;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                this.RotationPitch = 0;
                this.RotationRoll = 0;
                this.RotationYaw = -180;
                this.ResetPanning();
            }
        }

        public bool IsRearViewSelected
        {
            get
            {
                return Math.Abs(this.RotationPitch) < .001
                    && Math.Abs(this.RotationRoll) < .001
                    && Math.Abs(this.RotationYaw) < .001
                    && !this.IsPanned;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                this.RotationPitch = 0;
                this.RotationRoll = 0;
                this.RotationYaw = 0;
                this.ResetPanning();
            }
        }

        public bool IsLeftViewSelected
        {
            get
            {
                return Math.Abs(this.RotationPitch) < .001
                    && Math.Abs(this.RotationRoll) < .001
                    && Math.Abs(this.RotationYaw - 90) < .001
                    && !this.IsPanned;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                this.RotationPitch = 0;
                this.RotationRoll = 0;
                this.RotationYaw = 90;
                this.ResetPanning();
            }
        }

        public bool IsRightViewSelected
        {
            get
            {
                return Math.Abs(this.RotationPitch) < .001
                    && Math.Abs(this.RotationRoll) < .001
                    && Math.Abs(this.RotationYaw - (-90)) < .001
                    && !this.IsPanned;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                this.RotationPitch = 0;
                this.RotationRoll = 0;
                this.RotationYaw = -90;
                this.ResetPanning();
            }
        }

        public bool IsTopViewSelected
        {
            get
            {
                return Math.Abs(this.RotationPitch - (-90)) < .001
                    && Math.Abs(this.RotationRoll) < .001
                    && Math.Abs(this.RotationYaw) < .001
                    && !this.IsPanned;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                this.RotationPitch = -90;
                this.RotationRoll = 0;
                this.RotationYaw = 0;
                this.ResetPanning();
            }
        }

        public bool IsBottomViewSelected
        {
            get
            {
                return Math.Abs(this.RotationPitch - 90) < .001
                    && Math.Abs(this.RotationRoll) < .001
                    && Math.Abs(this.RotationYaw) < .001
                    && !this.IsPanned;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                this.RotationPitch = 90;
                this.RotationRoll = 0;
                this.RotationYaw = 0;
                this.ResetPanning();
            }
        }

        private bool _settingZoom;

        public double LinearZoomValue
        {
            get { return this.Camera == null ? double.NaN : (Math.Log(this.Camera.Zoom, 2) + 1) * 50; }
            set
            {
                _settingZoom = true;
                this.Camera.Zoom = Math.Pow(2, value / 50 - 1);
                this.RaisePropertyChanged(() => this.LinearZoomValue);
                this.RaisePropertyChanged(() => this.ActualZoomValue);
                _settingZoom = false;
            }
        }

        public double ActualZoomValue
        {
            get { return this.Camera?.Zoom ?? double.NaN; }
        }

        private void ResetPanning()
        {
            this.PanningX = 0;
            this.PanningY = 0;
            this.PanningZ = 0;
        }

        private void NotifyProjectionModePropertiesChanged()
        {
            this.RaisePropertyChanged(() => this.IsOrthographicModeSelected);
            this.RaisePropertyChanged(() => this.IsPerspectiveModeSelected);
        }

        private void NotifyLocationRelatedPropertiesChanged()
        {
            this.RaisePropertyChanged(() => this.IsFrontViewSelected);
            this.RaisePropertyChanged(() => this.IsTopViewSelected);
            this.RaisePropertyChanged(() => this.IsLeftViewSelected);
            this.RaisePropertyChanged(() => this.IsBottomViewSelected);
            this.RaisePropertyChanged(() => this.IsRightViewSelected);
            this.RaisePropertyChanged(() => this.IsRearViewSelected);
        }


        private void OnCameraChanged()
        {
            this.NotifyProjectionModePropertiesChanged();
            this.RaisePropertyChanged(() => this.FOV);
            this.RaisePropertyChanged(() => this.Equivalent135FocalLength);
            this.RaisePropertyChanged(() => this.PanningX);
            this.RaisePropertyChanged(() => this.PanningY);
            this.RaisePropertyChanged(() => this.PanningZ);
            this.RaisePropertyChanged(() => this.RotationPitch);
            this.RaisePropertyChanged(() => this.RotationRoll);
            this.RaisePropertyChanged(() => this.RotationYaw);
            this.RaisePropertyChanged(() => this.LinearZoomValue);
            this.RaisePropertyChanged(() => this.ActualZoomValue);
            this.NotifyLocationRelatedPropertiesChanged();
        }

        void Camera_RotationYawChanged(object sender, EventArgs e)
        {
            if (_settingRotationYaw)
                return;

            this.RaisePropertyChanged(() => this.RotationYaw);
            this.NotifyLocationRelatedPropertiesChanged();
        }

        void Camera_RotationPitchChanged(object sender, EventArgs e)
        {
            if (_settingRotationPitch)
                return;

            this.RaisePropertyChanged(() => this.RotationPitch);
            this.NotifyLocationRelatedPropertiesChanged();
        }



        void Camera_ZoomChanged(object sender, EventArgs e)
        {
            if (_settingZoom)
                return;

            this.RaisePropertyChanged(() => this.LinearZoomValue);
            this.RaisePropertyChanged(() => this.ActualZoomValue);
        }

        void Camera_ProjectionModeChanged(object sender, EventArgs e)
        {
            if (_settingProjectionMode)
                return;

            this.NotifyProjectionModePropertiesChanged();
        }

        void Camera_FOVChanged(object sender, EventArgs e)
        {
            if (_settingFov)
                return;
            this.RaisePropertyChanged(() => this.FOV);
            this.RaisePropertyChanged(() => this.Equivalent135FocalLength);
        }

        private void Camera_PanningZChanged(object sender, EventArgs e)
        {
            if (_settingPanningZ)
                return;

            this.RaisePropertyChanged(() => this.PanningZ);
            this.NotifyLocationRelatedPropertiesChanged();
        }

        private void Camera_PanningYChanged(object sender, EventArgs e)
        {
            if (_settingPanningY)
                return;

            this.RaisePropertyChanged(() => this.PanningY);
            this.NotifyLocationRelatedPropertiesChanged();
        }

        private void Camera_PanningXChanged(object sender, EventArgs e)
        {
            if (_settingPanningX)
                return;

            this.RaisePropertyChanged(() => this.PanningX);
            this.NotifyLocationRelatedPropertiesChanged();
        }

        private void Camera_RotationRollChanged(object sender, EventArgs e)
        {
            if (_settingRotationRoll)
                return;

            this.RaisePropertyChanged(() => this.RotationRoll);
            this.NotifyLocationRelatedPropertiesChanged();
        }
    }
}
