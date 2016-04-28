using System;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.TankConfigurator
{
    partial class TankConfigVM
    {

        private double _totalWeight;
        public double TotalWeight
        {
            get { return _totalWeight; }
            set
            {
                _totalWeight = value;
                this.RaisePropertyChanged(() => this.TotalWeight);
            }
        }

        public double MaxLoad
        {
            get { return ((Chassis)this.SelectedChassis.Model).MaximumLoad; }
        }


        private ComponentVM _selectedTurret;
        public ComponentVM SelectedTurret
        {
            get { return _selectedTurret; }
            set
            {
                _selectedTurret = value;
                if (value != null)
                    this.SetConfiguration(() => _configuration.Turret = (Turret)value.Model);

                this.RaisePropertyChanged(() => this.SelectedTurret);
                if (value != null)
                    this.OnTurretSelected();
            }
        }

        private ComponentVM _selectedGun;
        public ComponentVM SelectedGun
        {
            get { return _selectedGun; }
            set
            {
                _selectedGun = value;
                if (value != null)
                    this.SetConfiguration(() => _configuration.Gun = (Gun)value.Model);

                this.RaisePropertyChanged(() => this.SelectedGun);
                if (value != null)
                    this.OnGunSelected();
            }
        }

        private ComponentVM _selectedChassis;
        public ComponentVM SelectedChassis
        {
            get { return _selectedChassis; }
            set
            {
                _selectedChassis = value;
                if (value != null)
                    this.SetConfiguration(() => _configuration.Chassis = (Chassis)value.Model);

                this.RaisePropertyChanged(() => this.SelectedChassis);
                if (value != null)
                    this.OnChassisSelected();
            }
        }

        private ComponentVM _selectedEngine;
        public ComponentVM SelectedEngine
        {
            get { return _selectedEngine; }
            set
            {
                _selectedEngine = value;
                if (value != null)
                    this.SetConfiguration(() => _configuration.Engine = (Engine)value.Model);

                this.RaisePropertyChanged(() => this.SelectedEngine);
                if (value != null)
                    this.OnEngineSelected();
            }
        }

        private ComponentVM _selectedRadio;
        public ComponentVM SelectedRadio
        {
            get { return _selectedRadio; }
            set
            {
                _selectedRadio = value;
                if (value != null)
                    this.SetConfiguration(() => _configuration.Radio = (Radio)value.Model);

                this.RaisePropertyChanged(() => this.SelectedRadio);
                if (value != null)
                    this.OnRadioSelected();
            }
        }

        private ComponentVM _selectedAmmunition;
        public ComponentVM SelectedAmmunition
        {
            get { return _selectedAmmunition; }
            set
            {
                _selectedAmmunition = value;
                if (value != null)
                    this.SetConfiguration(() => _configuration.Ammunition = (Shell)value.Model);

                this.RaisePropertyChanged(() => this.SelectedAmmunition);
                if (value != null)
                    this.OnAmmunitionSelected();
            }
        }

        private readonly ComponentVM[] _selectedEquipments;
        private readonly ComponentVM[] _selectedConsumables;

        public ComponentVM SelectedEquipment1
        {
            get { return _selectedEquipments[0]; }
            set
            {
                _selectedEquipments[0] = value;
                this.SetConfiguration(() => _configuration.Equipment1 = value == null ? (Equipment)null : (Equipment)value.Model);

                this.OnEquipmentSelected(0);
                this.RaisePropertyChanged(() => this.SelectedEquipment1);
            }
        }

        public ComponentVM SelectedEquipment2
        {
            get { return _selectedEquipments[1]; }
            set
            {
                _selectedEquipments[1] = value;
                this.SetConfiguration(() => _configuration.Equipment2 = value == null ? (Equipment)null : (Equipment)value.Model);

                this.OnEquipmentSelected(1);
                this.RaisePropertyChanged(() => this.SelectedEquipment2);
            }
        }
        public ComponentVM SelectedEquipment3
        {
            get { return _selectedEquipments[2]; }
            set
            {
                _selectedEquipments[2] = value;
                this.SetConfiguration(() => _configuration.Equipment3 = value == null ? (Equipment)null : (Equipment)value.Model);

                this.OnEquipmentSelected(2);
                this.RaisePropertyChanged(() => this.SelectedEquipment3);
            }
        }
        public ComponentVM SelectedConsumable1
        {
            get { return _selectedConsumables[0]; }
            set
            {
                _selectedConsumables[0] = value;
                this.SetConfiguration(() => _configuration.Consumable1 = value == null ? null : (Consumable)value.Model);

                this.OnConsumableSelected(0);
                this.RaisePropertyChanged(() => this.SelectedConsumable1);
            }
        }


        public ComponentVM SelectedConsumable2
        {
            get { return _selectedConsumables[1]; }
            set
            {
                _selectedConsumables[1] = value;
                this.SetConfiguration(() => _configuration.Consumable2 = value == null ? null : (Consumable)value.Model);

                this.OnConsumableSelected(1);
                this.RaisePropertyChanged(() => this.SelectedConsumable2);
            }
        }
        public ComponentVM SelectedConsumable3
        {
            get { return _selectedConsumables[2]; }
            set
            {
                _selectedConsumables[2] = value;
                this.SetConfiguration(() => _configuration.Consumable3 = value == null ? null : (Consumable)value.Model);

                this.OnConsumableSelected(2);
                this.RaisePropertyChanged(() => this.SelectedConsumable3);
            }
        }

        public bool IsStockConfigurationEnabled
        {
            get
            {
                if (!_isInitialized)
                    return false;

                return this.AvailableGuns.Length > 0
                    || this.AvailableChassis.Length > 0
                    || this.AvailableEngines.Length > 0
                    || this.AvailableRadios.Length > 0
                    || this.AvailableTurrets.Length > 0;
            }
        }

        public bool IsStockConfigurationSelected
        {
            get
            {
                if (!_isInitialized)
                    return false;

                return this.SelectedGun == this.AvailableGuns[0]
                    && this.SelectedChassis == this.AvailableChassis[0]
                    && this.SelectedEngine == this.AvailableEngines[0]
                    && this.SelectedRadio == this.AvailableRadios[0]
                    && this.SelectedTurret == this.AvailableTurrets[0];
            }
            set
            {
                this.BeginInitialize();
                _configuration.LoadStockConfiguration();
                this.EndInitialize();
            }
        }

        public bool IsEliteConfigurationSelected
        {
            get
            {
                if (!_isInitialized)
                    return false;

                return this.SelectedGun == _eliteGunVM
                    && this.SelectedChassis == this.AvailableChassis.Last()
                    && this.SelectedEngine == this.AvailableEngines.Last()
                    && this.SelectedRadio == this.AvailableRadios.Last()
                    && this.SelectedTurret == this.AvailableTurrets.Last();
            }
            set
            {
                this.BeginInitialize();
                _configuration.LoadEliteConfiguration();
                this.EndInitialize();
            }
        }


        private ComponentVM GetSelectedModule(string moduleName)
        {
            switch (moduleName)
            {
                case "chassis":
                    return this.SelectedChassis as ComponentVM;
                case "engine":
                    return this.SelectedEngine as ComponentVM;
                case "gun":
                    return this.SelectedGun as ComponentVM;
                case "radio":
                    return this.SelectedRadio as ComponentVM;
                case "turret":
                    return this.SelectedTurret as ComponentVM;
                case "shell":
                    return this.SelectedAmmunition as ComponentVM;
                default:
                    throw new NotSupportedException();
            }

        }



        private void OnWeightedModuleSelected()
        {
            this.UpdateTotalWeight();
            this.TryUpgradeChassis();

            this.UpdateModuleAvailabilities();
        }

        private void OnTurretSelected()
        {
            if (_isInitializing)
                return;

            // replace adaptable guns with data from current turret
            foreach (var gun in this.AvailableGuns)
            {
                var updatedGun = ((Turret)this.SelectedTurret.Model)
                                 .Guns
                                 .FirstOrDefault(g => g.Equals((Gun)gun.Model));

                if (updatedGun != null)
                    gun.Model = updatedGun;
            }

            this.TryAdaptGun();

            this.OnWeightedModuleSelected();
            this.NotifyBulkConfigurationChanged();
        }

        private void OnGunSelected()
        {
            if (this.SelectedGun != null)
                this.UpdateAvailableAmmunitions();

            if (_isInitializing)
                return;

            this.TryAdaptTurret();
            this.OnWeightedModuleSelected();
            this.NotifyBulkConfigurationChanged();
        }

        private void OnAmmunitionSelected()
        {
            if (_isInitializing)
                return;
        }

        private void OnEngineSelected()
        {
            if (_isInitializing)
                return;

            this.OnWeightedModuleSelected();
            this.NotifyBulkConfigurationChanged();
        }
        private void OnRadioSelected()
        {
            if (_isInitializing)
                return;

            this.OnWeightedModuleSelected();
            this.NotifyBulkConfigurationChanged();
        }

        private void OnChassisSelected()
        {
            if (_isInitializing)
                return;

            this.RaisePropertyChanged(() => this.MaxLoad);
            this.OnWeightedModuleSelected();
            this.NotifyBulkConfigurationChanged();
        }

        private void SetEquipment(int index, ComponentVM equipment)
        {
            switch (index)
            {
                case 0:
                    this.SelectedEquipment1 = equipment;
                    break;
                case 1:
                    this.SelectedEquipment2 = equipment;
                    break;
                case 2:
                    this.SelectedEquipment3 = equipment;
                    break;
            }
        }

        private void SetConsumable(int index, ComponentVM Consumable)
        {
            switch (index)
            {
                case 0:
                    this.SelectedConsumable1 = Consumable;
                    break;
                case 1:
                    this.SelectedConsumable2 = Consumable;
                    break;
                case 2:
                    this.SelectedConsumable3 = Consumable;
                    break;
            }
        }

        private void OnEquipmentSelected(int index)
        {
            var equipment = _selectedEquipments[index];
            if (equipment != null)
            {
                for (var i = 0; i < _selectedEquipments.Length; ++i)
                {
                    if (i != index && _selectedEquipments[i] == _selectedEquipments[index])
                    {
                        this.SetEquipment(i, null);
                    }
                }
            }

            this.OnWeightedModuleSelected();

        }

        private void OnConsumableSelected(int index)
        {
            var consumable = _selectedConsumables[index];
            if (consumable != null)
            {
                for (var i = 0; i < _selectedConsumables.Length; ++i)
                {
                    if (i != index)
                    {
                        if (_selectedConsumables[i] == null)
                            continue;

                        if (_selectedConsumables[i] == consumable
                            || !Accessory.IsCompatible((Accessory)consumable.Model, (Accessory)_selectedConsumables[i].Model))
                            this.SetConsumable(i, null);
                    }
                }
            }

        }

        private void UpdateTotalWeight()
        {
            if (_isInitializing)
                return;

            var vehicleWeight = _configuration.Tank.Hull.Weight
                + TankConfigVM.GetModuleWeight(this.SelectedTurret)
                + TankConfigVM.GetModuleWeight(this.SelectedGun)
                + TankConfigVM.GetModuleWeight(this.SelectedChassis)
                + TankConfigVM.GetModuleWeight(this.SelectedEngine)
                + TankConfigVM.GetModuleWeight(this.SelectedRadio);

            var weight = vehicleWeight;
            if (this.SelectedEquipment1 != null)
                weight += ((Equipment)this.SelectedEquipment1.Model).GetWeight(vehicleWeight);
            if (this.SelectedEquipment2 != null)
                weight += ((Equipment)this.SelectedEquipment2.Model).GetWeight(vehicleWeight);
            if (this.SelectedEquipment3 != null)
                weight += ((Equipment)this.SelectedEquipment3.Model).GetWeight(vehicleWeight);

            this.TotalWeight = weight;
        }

        private void TryUpgradeChassis()
        {
            if (_isInitializing)
                return;

            if (this.MaxLoad >= this.TotalWeight)
                return;

            for (var i = this.AvailableChassis.IndexOf(this.SelectedChassis) + 1; i < this.AvailableChassis.Length; ++i)
            {
                var chassis = this.AvailableChassis[i];
                if (this.HasSufficientLoad(chassis))
                {
                    this.SelectedChassis = chassis;
                    break;
                }
            }
        }

        private void TryAdaptTurret()
        {
            if (this.IsTurretLocked)
                return;

            if (IsCompatible(this.SelectedTurret, this.SelectedGun))
                return;

            foreach (var turret in this.AvailableTurrets)
            {
                if (IsCompatible(turret, this.SelectedGun))
                    this.SelectedTurret = turret;
            }
        }

        private void TryAdaptGun()
        {
            if (this.IsGunLocked)
                return;

            if (IsCompatible(this.SelectedTurret, this.SelectedGun))
            {
                this.SelectedGun = this.SelectedGun;
                return;
            }

            foreach (var gun in this.AvailableGuns)
            {
                if (IsCompatible(this.SelectedTurret, gun))
                    this.SelectedGun = gun;
            }
        }

        private bool HasSufficientLoad(ComponentVM chassis)
        {
            var newWeight = this.TotalWeight - TankConfigVM.GetModuleWeight(this.SelectedChassis) + TankConfigVM.GetModuleWeight(chassis);

            return ((Chassis)chassis.Model).MaximumLoad >= newWeight;
        }

        private bool IsAvailableForCurrentChassis(ComponentVM module)
        {
            if (module.Model is Chassis)
                return this.HasSufficientLoad(module);

            var newWeight = this.TotalWeight - TankConfigVM.GetModuleWeight(this.GetSelectedModule(module.Model.ElementName)) + TankConfigVM.GetModuleWeight(module);

            return this.MaxLoad - newWeight >= 0;
        }


        private void NotifyBulkConfigurationChanged()
        {
            if (_isInitializing)
                return;

            this.RaisePropertyChanged(() => this.IsStockConfigurationSelected);
            this.RaisePropertyChanged(() => this.IsEliteConfigurationSelected);
        }


        private ComponentVM FindSimilarAmmunition(ComponentVM reference)
        {
            var isKinetic = ((Shell)reference.Model).IsKinetic;

            var sameCurrencyType = this.AvailableAmmunition.Where(s => ((Shell) s.Model).Currency == ((Shell)reference.Model).Currency).ToArray();

            ComponentVM ammunition;

            if (sameCurrencyType.Length == 0)
                ammunition = this.AvailableAmmunition[0];
            else if (sameCurrencyType.Length == 1)
                ammunition = sameCurrencyType[0];
            else
            {
                var sameType = sameCurrencyType.Where(s => ((Shell)s.Model).IsKinetic == isKinetic).ToArray();
                if (sameType.Length == 0)
                    ammunition = sameCurrencyType[0];
                else
                    ammunition = sameType.Last();
            }

            return ammunition;
        }

    }
}
