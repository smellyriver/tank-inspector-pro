using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Utilities;
using ComponentEntity = Smellyriver.TankInspector.Pro.Data.Entities.Component;

namespace Smellyriver.TankInspector.Pro.TankConfigurator
{
    partial class TankConfigVM : NotificationObject
    {
        private static double GetModuleWeight(ComponentVM module)
        {
            if (module == null)
                return 0.0;
            return ((Module)module.Model).Weight;
        }

        private double MaxLoadFactor
        {
            get
            {
                foreach (var equipment in _selectedEquipments)
                {
                    if (equipment == null)
                        continue;
                    var factorString = equipment.Model["script/chassisMaxLoadFactor"];
                    if (factorString != null)
                        return double.Parse(factorString, CultureInfo.InvariantCulture);
                }

                return 1.0;
            }

        }


        private TankConfiguration _configuration;
        public TankConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                if (_configuration != null)
                    _configuration.PropertyChanged -= Configuration_PropertyChanged;

                _configuration = value;

                if (_configuration != null)
                    _configuration.PropertyChanged += Configuration_PropertyChanged;

                this.RaisePropertyChanged(() => this.Configuration);

                if (_configuration != null)
                    this.InitializeModules();
            }
        }

        public IRepository Repository { get; set; }

        private bool _isInitialized;
        private bool _isInitializing;
        private bool _isSettingConfiguration;

        public TankConfigVM()
        {
            _selectedEquipments = new ComponentVM[3];
            _selectedConsumables = new ComponentVM[3];
            _moduleVmLookup = new Dictionary<ComponentEntity, ComponentVM>();

            TankConfiguratorSettings.Default.PropertyChanged += OnTankConfiguratorSettingChanged;
        }

        private void SetConfiguration(Action setter)
        {
            _isSettingConfiguration = true;
            setter();
            _isSettingConfiguration = false;
        }

        private void InitializeModules()
        {

            this.BeginInitialize();
            _moduleVmLookup.Clear();

            this.UpdateAvailableChassis();
            this.UpdateAvailableEngines();
            this.UpdateAvailableRadios();

            this.UpdateAvailableTurrets();
            this.UpdateAvailableGuns();

            this.UpdateAvailableAmmunitions();

            this.UpdateAvailableEquipments();
            this.UpdateAvailableConsumables();

            this.EndInitialize();


        }

        private void BeginInitialize()
        {
            _isInitializing = true;
            _isInitialized = false;
        }

        private void EndInitialize()
        {
            _isInitializing = false;
            _isInitialized = true;

            this.UpdateTotalWeight();
            this.UpdateModuleAvailabilities();
            this.NotifyBulkConfigurationChanged();

            this.RaisePropertyChanged(() => this.IsStockConfigurationEnabled);
        }

        private bool IsCompatible(Turret turret, Gun gun)
        {
            return turret.Guns.Contains(gun);
        }

        private bool IsCompatible(ComponentVM turret, ComponentVM gun)
        {
            return IsCompatible((Turret)turret.Model, (Gun)gun.Model);
        }

        void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isSettingConfiguration)
                return;

            var property = typeof(TankConfigVM).GetProperty("Selected" + e.PropertyName);
            if (property == null)
                return;

            property.SetValue(this, _moduleVmLookup[(ComponentEntity)_configuration.GetPropertyValue(e.PropertyName)], null);
        }
    }
}
