using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Model;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{

    class ModelSettingsVM : NotificationObject
    {
        private readonly ModelSettings _settings;

        public ModelSettingsVM(ModelSettings settings)
        {
            _settings = settings;
        }

        public bool IsUndamagedModelSelected
        {
            get { return _settings.ModelType == (int)ModelType.Undamaged; }
            set
            {
                _settings.ModelType = (int)ModelType.Undamaged;
                this.NotifyModelTypeSettingChanged();
            }
        } 

        public bool IsCollisionModelSelected
        {
            get { return _settings.ModelType == (int)ModelType.Collision; }
            set
            {
                _settings.ModelType = (int)ModelType.Collision;
                this.NotifyModelTypeSettingChanged();
            }
        }

        public bool IsDestroyedModelSelected
        {
            get { return _settings.ModelType == (int)ModelType.Damaged; }
            set
            {
                _settings.ModelType = (int)ModelType.Damaged;
                this.NotifyModelTypeSettingChanged();
            }
        }

        public bool IsExplodedModelSelected
        {
            get { return _settings.ModelType == (int)ModelType.Exploded; }
            set
            {
                _settings.ModelType = (int)ModelType.Exploded;
                this.NotifyModelTypeSettingChanged();
            }
        }

        public bool IsHDModelSelected
        {
            get { return _settings.UseHDModel; }
            set
            {
                _settings.UseHDModel = value;
                this.RaisePropertyChanged(() => this.IsHDModelSelected);
            }
        }

        public bool IsSolidModeSelected
        {
            get { return !_settings.WireframeMode; }
            set
            {
                _settings.WireframeMode = false;
                this.NotifyWireframeModeSettingChanged();
            }
        }

        public bool IsWireframeModeSelected
        {
            get { return _settings.WireframeMode; }
            set
            {
                _settings.WireframeMode = true;
                this.NotifyWireframeModeSettingChanged();
            }
        }

        public bool ShowChassis
        {
            get { return _settings.ShowChassis; }
            set
            {
                _settings.ShowChassis = value;
                this.RaisePropertyChanged(() => this.ShowChassis);
            }
        }

        public bool ShowHull
        {
            get { return _settings.ShowHull; }
            set
            {
                _settings.ShowHull = value;
                this.RaisePropertyChanged(() => this.ShowHull);
            }
        }

        public bool ShowGun
        {
            get { return _settings.ShowGun; }
            set
            {
                _settings.ShowGun = value;
                this.RaisePropertyChanged(() => this.ShowGun);
            }
        }

        public bool ShowTurret
        {
            get { return _settings.ShowTurret; }
            set
            {
                _settings.ShowTurret = value;
                this.RaisePropertyChanged(() => this.ShowTurret);
            }
        }

        public bool IsNormalTextureModeSelected
        {
            get { return !_settings.GridTextureMode; }
            set
            {
                _settings.GridTextureMode = false;
                this.NotifyGridTextureModeSettingChanged();
            }
        }

        public bool IsGridTextureModeSelected
        {
            get { return _settings.GridTextureMode; }
            set
            {
                _settings.GridTextureMode = true;
                this.NotifyGridTextureModeSettingChanged();
            }
        }

        public bool IsOfficialTextureSourceSelected
        {
            get { return !_settings.UseModTexture; }
            set
            {
                _settings.UseModTexture = false;
                this.NotifyModTextureModeSettingChanged();
            }
        }

        public bool IsModTextureSourceModeSelected
        {
            get { return _settings.UseModTexture; }
            set
            {
                _settings.UseModTexture = true;
                this.NotifyModTextureModeSettingChanged();
            }
        }

        public bool ShowCamouflage
        {
            get { return _settings.ShowCamouflage; }
            set
            {
                _settings.ShowCamouflage = value;
                this.RaisePropertyChanged(() => this.ShowCamouflage);
            }
        }

        private void NotifyModelTypeSettingChanged()
        {
            this.RaisePropertyChanged(() => this.IsUndamagedModelSelected);
            this.RaisePropertyChanged(() => this.IsCollisionModelSelected);
            this.RaisePropertyChanged(() => this.IsDestroyedModelSelected);
            this.RaisePropertyChanged(() => this.IsExplodedModelSelected);
        }
        private void NotifyWireframeModeSettingChanged()
        {
            this.RaisePropertyChanged(() => this.IsSolidModeSelected);
            this.RaisePropertyChanged(() => this.IsWireframeModeSelected);
        }
        private void NotifyGridTextureModeSettingChanged()
        {
            this.RaisePropertyChanged(() => this.IsNormalTextureModeSelected);
            this.RaisePropertyChanged(() => this.IsGridTextureModeSelected);
        }

        private void NotifyModTextureModeSettingChanged()
        {
            this.RaisePropertyChanged(() => this.IsOfficialTextureSourceSelected);
            this.RaisePropertyChanged(() => this.IsModTextureSourceModeSelected);
        }
    }
}
