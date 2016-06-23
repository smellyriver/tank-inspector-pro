using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank.Scripting;
using Smellyriver.TankInspector.Pro.Gameplay;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.Utilities;
using TankEntity = Smellyriver.TankInspector.Pro.Data.Entities.Tank;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [SuppressMessage("ReSharper", "UseNullPropagation")]
    [SuppressMessage("ReSharper", "MergeConditionalExpression")]
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    [SuppressMessage("ReSharper", "DelegateSubtraction")]
    public class TankConfiguration : ConfigurationBase
    {

        private TankConfigurationInfo _tankConfigurationInfo;
        public TankConfigurationInfo TankConfigurationInfo
        {
            get { return _tankConfigurationInfo; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _tankConfigurationInfo = value;

                this.BeginInitialize();

                this.Turret = this.Tank.Turrets.FirstOrDefault(t => t.Key == _tankConfigurationInfo.TurretKey);
                this.Gun = this.Turret != null ? this.Turret.Guns.FirstOrDefault(g => g.Key == _tankConfigurationInfo.GunKey) : null;
                this.Ammunition = this.Gun != null ? this.Gun.Ammunition.First(s => s.Key == _tankConfigurationInfo.AmmunitionKey) : null;
                this.Chassis = this.Tank.Chassis.FirstOrDefault(c => c.Key == _tankConfigurationInfo.ChassisKey);
                this.Engine = this.Tank.Engines.FirstOrDefault(e => e.Key == _tankConfigurationInfo.EngineKey);
                this.Radio = this.Tank.Radios.FirstOrDefault(r => r.Key == _tankConfigurationInfo.RadioKey);
                this.Equipment1 = this.QueryEquipment(_tankConfigurationInfo.EquipmentKeys[0]);
                this.Equipment2 = this.QueryEquipment(_tankConfigurationInfo.EquipmentKeys[1]);
                this.Equipment3 = this.QueryEquipment(_tankConfigurationInfo.EquipmentKeys[2]);
                this.Consumable1 = this.QueryConsumable(_tankConfigurationInfo.ConsumableKeys[0]);
                this.Consumable2 = this.QueryConsumable(_tankConfigurationInfo.ConsumableKeys[1]);
                this.Consumable3 = this.QueryConsumable(_tankConfigurationInfo.ConsumableKeys[2]);

                this.EndInitialize();
            }
        }

        private ElementChangedEventHandler _moduleElementChanged;
        internal event ElementChangedEventHandler ModuleElementChanged
        {
            add { _moduleElementChanged += value; }
            remove { _moduleElementChanged -= value; }
        }


        private ElementChangedEventHandler _equipmentElementChanged;
        internal event ElementChangedEventHandler EquipmentElementChanged
        {
            add { _equipmentElementChanged += value; }
            remove { _equipmentElementChanged -= value; }
        }

        private ElementChangedEventHandler _consumableElementChanged;
        internal event ElementChangedEventHandler ConsumableElementChanged
        {
            add { _consumableElementChanged += value; }
            remove { _consumableElementChanged -= value; }
        }

        private ComponentChangedEventHandler _gunChanged;

        public event ComponentChangedEventHandler GunChanged
        {
            add { _gunChanged += value; }
            remove { _gunChanged -= value; }
        }

        private ComponentChangedEventHandler _turretChanged;

        public event ComponentChangedEventHandler TurretChanged
        {
            add { _turretChanged += value; }
            remove { _turretChanged -= value; }
        }

        private ComponentChangedEventHandler _engineChanged;

        public event ComponentChangedEventHandler EngineChanged
        {
            add { _engineChanged += value; }
            remove { _engineChanged -= value; }
        }

        private ComponentChangedEventHandler _chassisChanged;

        public event ComponentChangedEventHandler ChassisChanged
        {
            add { _chassisChanged += value; }
            remove { _chassisChanged -= value; }
        }

        private ComponentChangedEventHandler _radioChanged;

        public event ComponentChangedEventHandler RadioChanged
        {
            add { _radioChanged += value; }
            remove { _radioChanged -= value; }
        }

        private ComponentChangedEventHandler _ammunitionChanged;

        public event ComponentChangedEventHandler AmmunitionChanged
        {
            add { _ammunitionChanged += value; }
            remove { _ammunitionChanged -= value; }
        }

        private EquipmentChangedEventHandler _equipmentChanged;

        public event EquipmentChangedEventHandler EquipmentChanged
        {
            add { _equipmentChanged += value; }
            remove { _equipmentChanged -= value; }
        }

        private ConsumableChangedEventHandler _consumableChanged;

        public event ConsumableChangedEventHandler ConsumableChanged
        {
            add { _consumableChanged += value; }
            remove { _consumableChanged -= value; }
        }


        private XElement _gunElement;
        internal XElement GunElement
        {
            get { return _gunElement; }
            private set
            {
                _gunElement = value;
                this.OnModuleElementChanged(value);
            }
        }


        private Gun _gun;
        public Gun Gun
        {
            get { return _gun; }
            set { this.SetGun(value, true); }
        }

        private void SetGun(Gun value, bool validateTurret)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var oldValue = _gun;
            _gun = value;

            var gunElement = _gun.ToElement();
            gunElement.ExistedElement("shots").Remove();
            this.GunElement = gunElement;

            _tankConfigurationInfo.GunKey = _gun.Key;

            if (validateTurret && !this.Turret.Guns.Contains(_gun, KeyEqualityComparer<Gun>.Instance))
            {
                var turret = this.Tank.Turrets.First(t => t.Guns.Contains(_gun, KeyEqualityComparer<Gun>.Instance));
                this.SetTurret(turret, false);
            }

            this.UpdateModulesTotalWeight();

            if (_gunChanged != null)
                _gunChanged(this, new ComponentChangedEventArgs(oldValue, value));

            this.RaisePropertyChanged("Gun");
        }

        private XElement _turretElement;
        internal XElement TurretElement
        {
            get { return _turretElement; }
            private set
            {
                _turretElement = value;
                this.OnModuleElementChanged(value);
            }
        }


        private Turret _turret;
        public Turret Turret
        {
            get { return _turret; }
            set { this.SetTurret(value, true); }
        }

        private void SetTurret(Turret value, bool validateGun)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (_turret.KeyEquals(value))
                return;

            var oldValue = _turret;
            _turret = value;

            var turretElement = _turret.ToElement();
            turretElement.ExistedElement("guns").Remove();
            this.TurretElement = turretElement;

            _tankConfigurationInfo.TurretKey = _turret["@key"];

            if (validateGun && !_turret.Guns.Contains(this.Gun, KeyEqualityComparer<Gun>.Instance))
            {
                var gun = _turret.Guns.First();
                this.SetGun(gun, false);
            }

            this.UpdateModulesTotalWeight();

            if (_turretChanged != null)
                _turretChanged(this, new ComponentChangedEventArgs(oldValue, value));

            this.RaisePropertyChanged("Turret");
        }

        private XElement _chassisElement;
        internal XElement ChassisElement
        {
            get { return _chassisElement; }
            private set
            {
                _chassisElement = value;
                this.OnModuleElementChanged(value);
            }
        }

        private Chassis _chassis;
        public Chassis Chassis
        {
            get { return _chassis; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_chassis.KeyEquals(value))
                    return;

                var oldValue = _chassis;
                _chassis = value;

                this.ChassisElement = _chassis.ToElement();

                _tankConfigurationInfo.ChassisKey = _chassis["@key"];

                this.UpdateModulesTotalWeight();

                if (_chassisChanged != null)
                    _chassisChanged(this, new ComponentChangedEventArgs(oldValue, value));

                this.RaisePropertyChanged("Chassis");
            }
        }

        private XElement _radioElement;
        internal XElement RadioElement
        {
            get { return _radioElement; }
            private set
            {
                _radioElement = value;
                this.OnModuleElementChanged(value);
            }
        }

        private Radio _radio;
        public Radio Radio
        {
            get { return _radio; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_radio.KeyEquals(value))
                    return;

                var oldValue = _radio;
                _radio = value;

                this.RadioElement = _radio.ToElement();

                _tankConfigurationInfo.RadioKey = _radio["@key"];

                this.UpdateModulesTotalWeight();

                if (_radioChanged != null)
                    _radioChanged(this, new ComponentChangedEventArgs(oldValue, value));

                this.RaisePropertyChanged("Radio");
            }
        }

        private XElement _engineElement;
        internal XElement EngineElement
        {
            get { return _engineElement; }
            private set
            {
                _engineElement = value;
                this.OnModuleElementChanged(value);
            }
        }

        private Engine _engine;
        public Engine Engine
        {
            get { return _engine; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_engine.KeyEquals(value))
                    return;

                var oldValue = _engine;
                _engine = value;

                this.EngineElement = _engine.ToElement();

                _tankConfigurationInfo.EngineKey = _engine["@key"];

                this.UpdateModulesTotalWeight();

                if (_engineChanged != null)
                    _engineChanged(this, new ComponentChangedEventArgs(oldValue, value));

                this.RaisePropertyChanged("Engine");
            }
        }


        private XElement _ammunitonElement;
        internal XElement AmmunitionElement
        {
            get { return _ammunitonElement; }
            private set
            {
                _ammunitonElement = value;
                this.OnModuleElementChanged(value);
            }
        }

        private Shell _ammunition;
        public Shell Ammunition
        {
            get { return _ammunition; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_ammunition.KeyEquals(value))
                    return;

                var oldValue = _ammunition;
                _ammunition = value;

                this.AmmunitionElement = _ammunition.ToElement();

                _tankConfigurationInfo.AmmunitionKey = _ammunition.Key;

                if (_ammunitionChanged != null)
                    _ammunitionChanged(this, new ComponentChangedEventArgs(oldValue, value));

                this.RaisePropertyChanged("Ammunition");
            }
        }


        private readonly Equipment[] _equipments;
        public IEnumerable<Equipment> Equipments { get { return _equipments; } }

        public Equipment Equipment1
        {
            get { return _equipments[0]; }
            set
            {
                var oldValue = _equipments[0];
                if (this.SetEquipment(0, value))
                {
                    if (_equipmentChanged != null)
                        _equipmentChanged(this, new EquipmentChangedEventArgs(0, oldValue, value));

                    this.RaisePropertyChanged("Equipment1");
                }
            }
        }
        public Equipment Equipment2
        {
            get { return _equipments[1]; }
            set
            {
                var oldValue = _equipments[1];
                if (this.SetEquipment(1, value))
                {
                    if (_equipmentChanged != null)
                        _equipmentChanged(this, new EquipmentChangedEventArgs(1, oldValue, value));

                    this.RaisePropertyChanged("Equipment2");
                }
            }
        }
        public Equipment Equipment3
        {
            get { return _equipments[2]; }
            set
            {
                var oldValue = _equipments[2];
                if (this.SetEquipment(2, value))
                {
                    if (_equipmentChanged != null)
                        _equipmentChanged(this, new EquipmentChangedEventArgs(2, oldValue, value));

                    this.RaisePropertyChanged("Equipment3");
                }
            }
        }


        private readonly Consumable[] _consumables;
        public IEnumerable<Consumable> Consumables { get { return _consumables; } }

        public Consumable Consumable1
        {
            get { return _consumables[0]; }
            set
            {
                var oldValue = _consumables[0];
                if (this.SetConsumable(0, value))
                {
                    if (_consumableChanged != null)
                        _consumableChanged(this, new ConsumableChangedEventArgs(0, oldValue, value));

                    this.RaisePropertyChanged("Consumable1");
                }
            }
        }

        public Consumable Consumable2
        {
            get { return _consumables[1]; }
            set
            {
                var oldValue = _consumables[1];
                if (this.SetConsumable(1, value))
                {
                    if (_consumableChanged != null)
                        _consumableChanged(this, new ConsumableChangedEventArgs(1, oldValue, value));

                    this.RaisePropertyChanged("Consumable2");
                }
            }
        }
        public Consumable Consumable3
        {
            get { return _consumables[2]; }
            set
            {
                var oldValue = _consumables[2];
                if (this.SetConsumable(2, value))
                {
                    if (_consumableChanged != null)
                        _consumableChanged(this, new ConsumableChangedEventArgs(2, oldValue, value));

                    this.RaisePropertyChanged("Consumable3");
                }
            }
        }

        private readonly XElement _equipmentsElement;
        internal XElement EquipmentsElement { get { return _equipmentsElement; } }

        private readonly XElement _consumablesElement;
        internal XElement ConsumablesElement { get { return _consumablesElement; } }

        private bool _isInitializing;

        internal TankConfiguration(IRepository repository, TankEntity tank, ScriptHost scriptHost, TankConfigurationInfo configInfo)
            : base(repository, tank, scriptHost)
        {

            _equipmentsElement = new XElement("equipments");
            _consumablesElement = new XElement("consumables");

            _equipments = new Equipment[3];
            _consumables = new Consumable[3];

            if (configInfo == null)
            {
                _tankConfigurationInfo = new TankConfigurationInfo();
                this.LoadStockConfiguration();
            }
            else
                this.TankConfigurationInfo = configInfo;
        }

        private void BeginInitialize()
        {
            _isInitializing = true;
        }

        private void EndInitialize()
        {
            _isInitializing = false;
            this.UpdateModulesTotalWeight();
        }

        private Equipment QueryEquipment(string key)
        {
            if (key == null)
                return null;
            else
                return new Equipment(this.Repository.EquipmentDatabase.Query("equipment[@key='{0}']", key));
        }

        private Consumable QueryConsumable(string key)
        {
            if (key == null)
                return null;
            else
                return new Consumable(this.Repository.ConsumableDatabase.Query("consumable[@key='{0}']", key));
        }

        private bool SetEquipment(int index, Equipment equipment)
        {
            XElement element;
            if (equipment == null)
                element = null;
            else
            {
                element = equipment.ToElement();
                element.SetElementValue("weight", equipment.GetWeight(this.ModulesTotalWeight));
            }
            // ReSharper disable once CoVariantArrayConversion
            var result = this.SetAccessory(_equipments, _tankConfigurationInfo.EquipmentKeys, _equipmentsElement, index, equipment, element, "equipment");

            this.OnEquipmentElementChanged(element);

            return result;
        }

        private bool SetConsumable(int index, Consumable consumable)
        {
            var element = consumable != null ? consumable.ToElement() : null;

            // ReSharper disable once CoVariantArrayConversion
            var result = this.SetAccessory(_consumables, _tankConfigurationInfo.ConsumableKeys, _consumablesElement, index, consumable, element, "consumable");

            this.OnConsumableElementChanged(element);

            return result;
        }

        private bool SetAccessory(IXQueryable[] accessoryArray, string[] keyArray, XElement elements, int index, IXQueryable value, XElement valueElement, string prefix)
        {
            if (accessoryArray[index].KeyEquals(value))
                return false;

            var oldAccessory = accessoryArray[index];

            accessoryArray[index] = value;
            this.ScriptHost.SetScript(prefix + (index + 1).ToString(), AccessoryScript.Create(value));
            keyArray[index] = value != null ? value["@key"] : null;

            if (oldAccessory == null && value != null)
            {
                elements.Add(valueElement);
            }
            else if (oldAccessory != null)
            {
                var oldElement = elements.Elements().First(e => e.ExistedAttribute("key").Value == oldAccessory["@key"]);

                if (value == null)
                    oldElement.Remove();
                else
                    oldElement.ReplaceWith(valueElement);
            }

            return true;
        }

        private void OnModuleElementChanged(XElement element)
        {
            if (_moduleElementChanged != null)
                _moduleElementChanged(this, new ElementChangedEventArgs(element));
        }

        private void OnEquipmentElementChanged(XElement element)
        {
            if (_equipmentElementChanged != null)
                _equipmentElementChanged(this, new ElementChangedEventArgs(element));
        }

        private void OnConsumableElementChanged(XElement element)
        {
            if (_consumableElementChanged != null)
                _consumableElementChanged(this, new ElementChangedEventArgs(element));
        }

        private void UpdateModulesTotalWeight()
        {
            if (_isInitializing)
                return;

            this.ModulesTotalWeight = this.Tank.Hull.Weight
                + this.Engine.Weight
                + this.Gun.Weight
                + this.Chassis.Weight
                + this.Turret.Weight
                + this.Radio.Weight;
        }


        private double _modulesTotalWeight;

        private double ModulesTotalWeight
        {
            get { return _modulesTotalWeight; }
            set
            {
                _modulesTotalWeight = value;

                for (var i = 0; i < 3; ++i)
                {
                    if (_equipments[i] != null)
                    {
                        var element = _equipmentsElement.Elements().First(e => e.ExistedAttribute("key").Value == _equipments[i].Key);
                        element.SetElementValue("weight", _equipments[i].GetWeight(this.ModulesTotalWeight));
                        this.OnEquipmentElementChanged(element);
                    }
                }
            }
        }

        public void LoadStockConfiguration()
        {
            this.BeginInitialize();

            this.Chassis = this.Tank.Chassis.First();
            this.Engine = this.Tank.Engines.First();
            this.Turret = this.Tank.Turrets.First();
            this.Radio = this.Tank.Radios.First();
            this.Gun = this.Tank.Guns.First();
            this.Ammunition = this.Gun.Ammunition.First();


            this.EndInitialize();
        }

        public void LoadEliteConfiguration()
        {
            this.BeginInitialize();

            this.Chassis = this.Tank.Chassis.Last();
            this.Engine = this.Tank.Engines.Last();
            this.Turret = this.Tank.Turrets.Last();
            this.Radio = this.Tank.Radios.Last();
            var maxTier = this.Turret.Guns.AotSafeMax(g => g.Tier);
            this.Gun = this.Turret.Guns.Last(g => g.Tier == maxTier);
            this.Ammunition = this.Gun.Ammunition.First();

            this.EndInitialize();
        }

    }
}
