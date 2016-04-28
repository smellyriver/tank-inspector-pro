using System;
using System.Windows.Input;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.TurretController
{
    class TurretControllerVM : NotificationObject
    {

        private bool _initializingTankInstance;
        private TankInstance _tankInstance;

        public TankInstance TankInstance
        {
            get { return _tankInstance; }
            set
            {
                _initializingTankInstance = true;

                if (_tankInstance != null)
                    this.UnhandleTankInstanceEvents(_tankInstance);
                _tankInstance = value;

                if (_tankInstance != null)
                    this.HandleTankInstanceEvents(_tankInstance);

                this.RaisePropertyChanged(() => this.ElevationLimits);
                this.RaisePropertyChanged(() => this.DepressionLimits);
                this.RaisePropertyChanged(() => this.TurretYawLimits);

                this.RaisePropertyChanged(() => this.MinimumGunPitch);
                this.RaisePropertyChanged(() => this.MaximumGunPitch);
                this.RaisePropertyChanged(() => this.MinimumTurretYaw);
                this.RaisePropertyChanged(() => this.MaximumTurretYaw);

                this.RaisePropertyChanged(() => this.GunPitch);
                this.RaisePropertyChanged(() => this.TurretYaw);

                _initializingTankInstance = false;

            }
        }


        public GunPitchLimitsComponent ElevationLimits
        {
            get
            {
                return this.ForTankInstance(t => t.TankConfiguration.Gun.ElevationLimits);
            }
        }

        public GunPitchLimitsComponent DepressionLimits
        {
            get
            {
                return this.ForTankInstance(t => t.TankConfiguration.Gun.DepressionLimits);
            }
        }
        public TurretYawLimits TurretYawLimits
        {
            get
            {
                return this.ForTankInstance(t => t.TankConfiguration.Gun.YawLimits);
            }
        }


        public double TurretYaw
        {
            get { return this.ForTankInstance(t => t.Transform.TurretYaw); }
            set
            {
                if (_initializingTankInstance)
                    return;
                this.ForTankInstance(t => t.Transform.TurretYaw = value);
            }
        }

        public double GunPitch
        {
            get { return this.ForTankInstance(t => t.Transform.GunPitch); }
            set
            {
                if (_initializingTankInstance)
                    return;
                this.ForTankInstance(t => t.Transform.GunPitch = value);
            }
        }

        public double MinimumTurretYaw
        {
            get
            {
                var limits = this.TurretYawLimits;
                if (limits == null)
                    return 0;

                return limits.Left;
            }
        }

        public double MaximumTurretYaw
        {
            get
            {
                var limits = this.TurretYawLimits;
                if (limits == null)
                    return 0;

                return limits.Right;
            }
        }

        public double MinimumGunPitch
        {
            get
            {
                var limits = this.ElevationLimits;
                if (limits == null)
                    return 0;

                return limits.GetValue(this.TurretYaw);
            }
        }

        public double MaximumGunPitch
        {
            get
            {
                var limits = this.DepressionLimits;
                if (limits == null)
                    return 0;

                return limits.GetValue(this.TurretYaw);
            }
        }

        public double VehicleYaw
        {
            get
            {
                return this.ForTankInstance(t => t.Transform.VehicleYaw);
            }
        }

        public ICommand ResetCommand { get; private set; }


        public TurretControllerVM()
        {
            this.ResetCommand = new RelayCommand(this.Reset);
        }

        private void Reset()
        {
            this.TurretYaw = 0;
            this.GunPitch = 0;
        }

        private void HandleTankInstanceEvents(TankInstance tank)
        {
            tank.TankConfiguration.GunChanged += OnGunChanged;
            tank.Transform.TurretYawChanged += Transform_TurretYawChanged;
            tank.Transform.VehicleYawChanged += Transform_VehicleYawChanged;
            tank.Transform.GunPitchChanged += Transform_GunPitchChanged;
        }

        void Transform_GunPitchChanged(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(() => this.GunPitch);
        }

        void Transform_VehicleYawChanged(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(() => this.VehicleYaw);
            this.RaisePropertyChanged(() => this.MinimumGunPitch);
            this.RaisePropertyChanged(() => this.MaximumGunPitch);
        }

        void Transform_TurretYawChanged(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(() => this.TurretYaw);
        }

        private void UnhandleTankInstanceEvents(TankInstance tank)
        {
            tank.TankConfiguration.GunChanged -= OnGunChanged;
            tank.Transform.TurretYawChanged -= Transform_TurretYawChanged;
            tank.Transform.VehicleYawChanged -= Transform_VehicleYawChanged;
            tank.Transform.GunPitchChanged -= Transform_GunPitchChanged;
        }

        void OnGunChanged(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(() => this.ElevationLimits);
            this.RaisePropertyChanged(() => this.DepressionLimits);
            this.RaisePropertyChanged(() => this.TurretYawLimits);
            this.RaisePropertyChanged(() => this.MinimumGunPitch);
            this.RaisePropertyChanged(() => this.MaximumGunPitch);
            this.RaisePropertyChanged(() => this.MinimumTurretYaw);
            this.RaisePropertyChanged(() => this.MaximumTurretYaw);
        }

        private void ForTankInstance(Action<TankInstance> action)
        {
            if (this.TankInstance == null)
                return;

            action(this.TankInstance);
        }


        private T ForTankInstance<T>(Func<TankInstance, T> func)
        {
            if (this.TankInstance == null)
                return default(T);

            return func(this.TankInstance);
        }
    }
}
