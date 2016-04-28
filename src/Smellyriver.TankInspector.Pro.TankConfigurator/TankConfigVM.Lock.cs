namespace Smellyriver.TankInspector.Pro.TankConfigurator
{
    partial class TankConfigVM
    {

        private bool _isGunLocked;
        public bool IsGunLocked
        {
            get { return _isGunLocked; }
            set
            {
                _isGunLocked = value;
                this.RaisePropertyChanged(() => this.IsGunLocked);
                this.UpdateModuleAvailabilities();
            }
        }

        private bool _isTurretLocked;
        public bool IsTurretLocked
        {
            get { return _isTurretLocked; }
            set
            {
                _isTurretLocked = value;
                this.RaisePropertyChanged(() => this.IsTurretLocked);
                this.UpdateModuleAvailabilities();
            }
        }


        private bool _isChassisLocked;
        public bool IsChassisLocked
        {
            get { return _isChassisLocked; }
            set
            {
                _isChassisLocked = value;
                this.RaisePropertyChanged(() => this.IsChassisLocked);
                this.UpdateModuleAvailabilities();
            }
        }

    }
}
