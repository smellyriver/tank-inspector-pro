using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class Turret : Module, IArmoredObject, IInternalArmoredObject, IModelObject
    {
        private Gun[] _guns;

        public IEnumerable<Gun> Guns
        {
            get
            {
                if (_guns == null)
                    _guns = this.QueryMany("guns/gun").Select(g => new Gun(g)).ToArray();
                return _guns;
            }
        }


        public bool IsPrimaryArmorDefined { get { return IArmoredObjectImpl.GetIsPrimaryArmorDefined(this); } }
        public double FrontArmor { get { return IArmoredObjectImpl.GetFrontArmor(this); } }
        public double SideArmor { get { return IArmoredObjectImpl.GetSideArmor(this); } }
        public double RearArmor { get { return IArmoredObjectImpl.GetRearArmor(this); } }

        private readonly Dictionary<string, ArmorGroup> _armorGroups;
        Dictionary<string, ArmorGroup> IInternalArmoredObject.ArmorGroups { get { return _armorGroups; } }



        public Turret(IXQueryable turretData)
                : base(turretData)
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
