using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Gameplay;

namespace Smellyriver.TankInspector.Pro.TankConfigurator
{
    partial class TankConfigVM
    {

        private ComponentVM[] _availableTurrets;
        public ComponentVM[] AvailableTurrets
        {
            get { return _availableTurrets; }
            set
            {
                _availableTurrets = value;
                this.RaisePropertyChanged(() => this.AvailableTurrets);
            }
        }

        private ComponentVM[] _availableGuns;
        public ComponentVM[] AvailableGuns
        {
            get { return _availableGuns; }
            set
            {
                _availableGuns = value;
                this.RaisePropertyChanged(() => this.AvailableGuns);
            }
        }

        private ComponentVM[] _availableChassis;
        public ComponentVM[] AvailableChassis
        {
            get { return _availableChassis; }
            set
            {
                _availableChassis = value;
                this.RaisePropertyChanged(() => this.AvailableChassis);
            }
        }

        private ComponentVM[] _availableEngines;
        public ComponentVM[] AvailableEngines
        {
            get { return _availableEngines; }
            set
            {
                _availableEngines = value;
                this.RaisePropertyChanged(() => this.AvailableEngines);
            }
        }

        private ComponentVM[] _availableRadios;
        public ComponentVM[] AvailableRadios
        {
            get { return _availableRadios; }
            set
            {
                _availableRadios = value;
                this.RaisePropertyChanged(() => this.AvailableRadios);
            }
        }

        private ComponentVM[] _availableAmmunition;
        public ComponentVM[] AvailableAmmunition
        {
            get { return _availableAmmunition; }
            set
            {
                _availableAmmunition = value;
                this.RaisePropertyChanged(() => this.AvailableAmmunition);
            }
        }


        private ComponentVM[] _availableConsumables;
        public ComponentVM[] AvailableConsumables
        {
            get { return _availableConsumables; }
            set
            {
                _availableConsumables = value;
                this.RaisePropertyChanged(() => this.AvailableConsumables);
            }
        }


        private ComponentVM[] _availableEquipments;
        public ComponentVM[] AvailableEquipments
        {
            get { return _availableEquipments; }
            set
            {
                _availableEquipments = value;
                this.RaisePropertyChanged(() => this.AvailableEquipments);
            }
        }

        private IEnumerable<ComponentVM> AvailableWeightedModules
        {
            get
            {
                return ((IEnumerable<ComponentVM>)this.AvailableTurrets)
                           .Union(this.AvailableGuns)
                           .Union(this.AvailableChassis)
                           .Union(this.AvailableEngines)
                           .Union(this.AvailableRadios);
            }
        }

        private ComponentVM _eliteGunVM;

        private readonly Dictionary<Component, ComponentVM> _moduleVmLookup;

        private void UpdateAvailableConsumables()
        {
            // WARNING: do not set this.AvailableConsumables directly, otherwise the ComboBoxes' data source will be updated,
            // and since the previously selected items in the ComboBoxes are not existed in the new data source, they will
            // be cleared. After that, the clear operation will be synchronized back to _configuration.
            _availableConsumables = this.Repository
                                        .ConsumableDatabase
                                        .QueryMany("consumable")
                                        .Select(c => new Consumable(c))
                                        .Where(c => c.CanBeUsedBy(_configuration.Tank))
                                        .Select(CreateConsumableVM)
                                        .ToArray();

            this.SelectedConsumable1 = _configuration.Consumable1 == null ? null : (ComponentVM)_moduleVmLookup[_configuration.Consumable1];
            this.SelectedConsumable2 = _configuration.Consumable2 == null ? null : (ComponentVM)_moduleVmLookup[_configuration.Consumable2];
            this.SelectedConsumable3 = _configuration.Consumable3 == null ? null : (ComponentVM)_moduleVmLookup[_configuration.Consumable3];

            this.RaisePropertyChanged(() => this.AvailableConsumables);
        }

        private void UpdateAvailableEquipments()
        {
            // WARNING: do not set this.AvailableEquipments directly, otherwise the ComboBoxes' data source will be updated,
            // and since the previously selected items in the ComboBoxes are not existed in the new data source, they will
            // be cleared. After that, the clear operation will be synchronized back to _configuration.
            _availableEquipments = this.Repository
                                       .EquipmentDatabase
                                       .QueryMany("equipment")
                                       .Select(e => new Equipment(e))
                                       .Where(c => c.CanBeUsedBy(_configuration.Tank))
                                       .Select(CreateEquipmentVM)
                                       .ToArray();

            this.SelectedEquipment1 = _configuration.Equipment1 == null ? null : (ComponentVM)_moduleVmLookup[_configuration.Equipment1];
            this.SelectedEquipment2 = _configuration.Equipment2 == null ? null : (ComponentVM)_moduleVmLookup[_configuration.Equipment2];
            this.SelectedEquipment3 = _configuration.Equipment3 == null ? null : (ComponentVM)_moduleVmLookup[_configuration.Equipment3];

            this.RaisePropertyChanged(() => this.AvailableEquipments);
        }

        private ComponentVM CreateEquipmentVM(Equipment equipment)
        {
            var vm = new ComponentVM(equipment, equipment.Currency == Currency.Gold);
            vm.Icon = equipment.GetIcon(this.Repository);
            _moduleVmLookup[equipment] = vm;

            return vm;
        }

        private ComponentVM CreateConsumableVM(Consumable consumable)
        {
            var vm = new ComponentVM(consumable, consumable.Currency == Currency.Gold);
            vm.Icon = consumable.GetIcon(this.Repository);
            _moduleVmLookup[consumable] = vm;

            return vm;
        }


        private ComponentVM CreateShellVM(Shell shell)
        {
            var vm = new ComponentVM(shell, shell.Currency == Currency.Gold);
            vm.Icon = shell.GetIcon(this.Repository);
            _moduleVmLookup[shell] = vm;

            return vm;
        }

        private void UpdateAvailableAmmunitions()
        {
            var previousAmmunition = this.SelectedAmmunition;
            this.AvailableAmmunition = ((Gun)this.SelectedGun.Model)
                                       .Ammunition
                                       .Select(this.CreateShellVM)
                                       .ToArray();

            if (previousAmmunition != null)
                this.SelectedAmmunition = FindSimilarAmmunition(previousAmmunition);
            else
            {
                var selectedAmmunition = this.AvailableAmmunition.FirstOrDefault(
                    a => a.Model.KeyEquals(_configuration.Ammunition));
                this.SelectedAmmunition = selectedAmmunition ?? this.AvailableAmmunition[0];
            }
        }

        private ComponentVM[] CreateModuleVMs(IEnumerable<Component> components)
        {
            var vms = components.Select(r => new ComponentVM(r, object.Equals(r, components.Last()))).ToArray();
            foreach (var vm in vms)
                _moduleVmLookup[vm.Model] = vm;

            return vms;
        }

        private void UpdateAvailableRadios()
        {
            this.AvailableRadios = this.CreateModuleVMs(this.Configuration.Tank.Radios);
            this.SelectedRadio = (ComponentVM)_moduleVmLookup[_configuration.Radio];


            this.UpdateModuleAvailabilities();
        }

        private void UpdateAvailableEngines()
        {
            this.AvailableEngines = this.CreateModuleVMs(this.Configuration.Tank.Engines);
            this.SelectedEngine = (ComponentVM)_moduleVmLookup[_configuration.Engine];


            this.UpdateModuleAvailabilities();
        }

        private void UpdateAvailableChassis()
        {
            this.AvailableChassis = this.CreateModuleVMs(this.Configuration.Tank.Chassis);
            this.SelectedChassis = (ComponentVM)_moduleVmLookup[_configuration.Chassis];

            this.UpdateModuleAvailabilities();
        }

        private void UpdateAvailableTurrets()
        {
            this.AvailableTurrets = this.CreateModuleVMs(this.Configuration.Tank.Turrets);
            this.SelectedTurret = (ComponentVM)_moduleVmLookup[_configuration.Turret];

            this.UpdateModuleAvailabilities();
        }

        private void UpdateAvailableGuns()
        {
            var maxTier = this.Configuration.Tank.Guns.Max(g => g.Tier);
            var eliteGun = this.Configuration.Tank.Guns.Last(g => g.Tier == maxTier);

            _eliteGunVM = new ComponentVM(eliteGun, true);

            this.AvailableGuns = this.Configuration.Tank.Guns.Select(g => g == eliteGun ? _eliteGunVM : new ComponentVM(g, false))
                                                             .ToArray();

            foreach (var gun in this.AvailableGuns)
                _moduleVmLookup[gun.Model] = gun;

            var selectedGun = this.AvailableGuns.FirstOrDefault(
                g => g.Model.KeyEquals(_configuration.Gun));
            this.SelectedGun = selectedGun ?? this.AvailableGuns[0];

            this.UpdateModuleAvailabilities();
        }

        private void UpdateModuleAvailabilities()
        {
            if (_isInitializing)
                return;

            // initialize
            foreach (var module in this.AvailableWeightedModules)
                module.IsEnabled = true;

            if (this.IsTurretLocked)
            {
                foreach (var gun in this.AvailableGuns)
                {
                    gun.IsEnabled = gun.IsEnabled
                                 && IsCompatible(this.SelectedTurret, gun);
                }
            }

            if (this.IsGunLocked)
            {
                foreach (var turret in this.AvailableTurrets)
                {
                    turret.IsEnabled = turret.IsEnabled
                                    && IsCompatible(turret, this.SelectedGun);
                }
            }

            if (this.IsChassisLocked)
            {
                foreach (var module in this.AvailableWeightedModules)
                {
                    module.IsEnabled = module.IsEnabled
                                    && this.IsAvailableForCurrentChassis(module);
                }
            }
        }

    }
}
