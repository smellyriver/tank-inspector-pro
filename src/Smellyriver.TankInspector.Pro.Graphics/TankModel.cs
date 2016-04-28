using System;
using SharpDX;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    class TankModel : NotificationObject
    {

        private Vector3 _turretPosition;
        public Vector3 TurretPosition
        {
            get { return _turretPosition; }
            set
            {
                _turretPosition = value;
                this.RaisePropertyChanged(() => this.TurretPosition);
            }
        }

        private Vector3 _gunPosition;
        public Vector3 GunPosition
        {
            get { return _gunPosition; }
            set
            {
                _gunPosition = value;
                this.RaisePropertyChanged(() => this.GunPosition);
            }
        }

        private Vector3 _hullPosition;
        public Vector3 HullPosition
        {
            get { return _hullPosition; }
            set
            {
                _hullPosition = value;
                this.RaisePropertyChanged(() => this.HullPosition);
            }
        }

        private ModuleModel _hullModel;
        public ModuleModel HullModel
        {
            get { return _hullModel; }
            set
            {
                _hullModel = value;
                this.RaisePropertyChanged(() => this.HullModel);

                if (this.HullChanged != null)
                    this.HullChanged(this, EventArgs.Empty);
            }
        }

        private ModuleModel _turretModel;
        public ModuleModel TurretModel
        {
            get { return _turretModel; }
            set
            {
                _turretModel = value;
                this.RaisePropertyChanged(() => this.TurretModel);


                if (this.TurretChanged != null)
                    this.TurretChanged(this, EventArgs.Empty);
            }
        }

        private ModuleModel _gunModel;
        public ModuleModel GunModel
        {
            get { return _gunModel; }
            set
            {
                _gunModel = value;
                this.RaisePropertyChanged(() => this.GunModel);


                if (this.GunChanged != null)
                    this.GunChanged(this, EventArgs.Empty);
            }
        }

        private ModuleModel _chassisModel;
        public ModuleModel ChassisModel
        {
            get { return _chassisModel; }
            set
            {
                _chassisModel = value;
                this.RaisePropertyChanged(() => this.ChassisModel);

                if (this.ChassisChanged != null)
                    this.ChassisChanged(this, EventArgs.Empty);
            }
        }


        public event EventHandler HullChanged;
        public event EventHandler GunChanged;
        public event EventHandler TurretChanged;
        public event EventHandler ChassisChanged;

        public TankInstance TankInstance { get; }

        public LocalGameClient GameClient { get { return this.TankInstance.Repository as LocalGameClient; } }

        public CamouflageInfo Camouflage { get; private set; }

        public TankModel(TankInstance tankInstance)
        {
            this.TankInstance = tankInstance;
            this.Camouflage = new CamouflageInfo(tankInstance);

            this.LoadHullModel();
            this.LoadChassisModel();
            this.LoadTurretModel();
            this.LoadGunModel();

            this.TankInstance.TankConfiguration.TurretChanged += TankConfiguration_TurretChanged;
            this.TankInstance.TankConfiguration.GunChanged += TankConfiguration_GunChanged;
            this.TankInstance.TankConfiguration.ChassisChanged += TankConfiguration_ChassisChanged;
        }

        private void LoadHullModel()
        {
            this.HullModel = new ModuleModel(this.TankInstance, this.TankInstance.Tank.Hull);
            this.TurretPosition = this.TankInstance.Tank.Hull.GetTurretPosition();
        }

        private void LoadChassisModel()
        {
            this.ChassisModel = new ModuleModel(this.TankInstance, this.TankInstance.TankConfiguration.Chassis);
            this.HullPosition = this.TankInstance.TankConfiguration.Chassis.GetHullPosition();
        }

        private void LoadGunModel()
        {
            this.GunModel = new ModuleModel(this.TankInstance, this.TankInstance.TankConfiguration.Gun);
        }

        private void LoadTurretModel()
        {
            this.TurretModel = new ModuleModel(this.TankInstance, this.TankInstance.TankConfiguration.Turret);
            this.GunPosition = this.TankInstance.TankConfiguration.Turret.GetGunPosition();
        }

        void TankConfiguration_ChassisChanged(object sender, EventArgs e)
        {
            this.LoadChassisModel();
        }

        void TankConfiguration_GunChanged(object sender, EventArgs e)
        {
            this.LoadGunModel();
        }

        void TankConfiguration_TurretChanged(object sender, EventArgs e)
        {
            this.LoadTurretModel();
        }


    }
}
