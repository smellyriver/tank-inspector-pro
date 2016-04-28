using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class Hull : Module, IArmoredObject, IInternalArmoredObject, IModelObject
    {
        public bool IsPrimaryArmorDefined { get { return IArmoredObjectImpl.GetIsPrimaryArmorDefined(this); } }
        public double FrontArmor { get { return IArmoredObjectImpl.GetFrontArmor(this); } }
        public double SideArmor { get { return IArmoredObjectImpl.GetSideArmor(this); } }
        public double RearArmor { get { return IArmoredObjectImpl.GetRearArmor(this); } }

        private readonly Dictionary<string, ArmorGroup> _armorGroups;
        Dictionary<string, ArmorGroup> IInternalArmoredObject.ArmorGroups { get { return _armorGroups; } }

        internal Hull(IXQueryable hullData)
            : base(hullData)
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
