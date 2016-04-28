using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class Gun : Module, IArmoredObject, IInternalArmoredObject, IModelObject
    {
        private Shell[] _ammunition;

        public IEnumerable<Shell> Ammunition
        {
            get
            {
                if (_ammunition == null)
                    _ammunition = this.QueryMany("shots/shell").Select(s => new Shell(s)).ToArray();
                return _ammunition;
            }
        }

        private readonly Dictionary<string, ArmorGroup> _armorGroups;
        Dictionary<string, ArmorGroup> IInternalArmoredObject.ArmorGroups { get { return _armorGroups; } }

        private GunPitchLimitsComponent _elevationLimits;

        public GunPitchLimitsComponent ElevationLimits
        {
            get
            {
                if (_elevationLimits == null)
                    _elevationLimits = new GunPitchLimitsComponent(this.Query("elevation"));
                return _elevationLimits;
            }
        }

        private GunPitchLimitsComponent _depressionLimits;

        public GunPitchLimitsComponent DepressionLimits
        {
            get
            {
                if (_depressionLimits == null)
                    _depressionLimits = new GunPitchLimitsComponent(this.Query("depression"));
                return _depressionLimits;
            }
        }

        private TurretYawLimits _yawLimits;
        public TurretYawLimits YawLimits
        {
            get
            {
                if (_yawLimits == null)
                    _yawLimits = new TurretYawLimits(this.Query("turretYawLimits"));
                return _yawLimits;
            }
        }

        public Gun(IXQueryable gunData)
            : base(gunData)
        {
            _armorGroups = new Dictionary<string, ArmorGroup>();
        }

        public ArmorGroup GetArmorGroup(string key)
        {
            return IArmoredObjectImpl.GetArmorGroup(this, key);
        }
        public double GetThickestArmor(bool spaced)
        {
            return IArmoredObjectImpl.GetThickestArmor(this, spaced);
        }

        public double GetThinnestArmor(bool spaced)
        {
            return IArmoredObjectImpl.GetThinnestArmor(this, spaced);
        }

        public double[] GetArmorValues(bool spaced)
        {
            return IArmoredObjectImpl.GetArmorValues(this, spaced);
        }

        public string GetModelPath(ModelType type)
        {
            return IModelObjectImpl.GetModelPath(this, type);
        }
    }
}
