using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class Chassis : Module, IArmoredObject, IInternalArmoredObject, IModelObject
    {
        public double TraverseSpeed { get { return this.QueryDouble("rotationSpeed"); } }
        public double MaximumLoad { get { return this.QueryDouble("maxLoad"); } }

        private readonly Dictionary<string, ArmorGroup> _armorGroups;
        Dictionary<string, ArmorGroup> IInternalArmoredObject.ArmorGroups { get { return _armorGroups; } }

        public Chassis(IXQueryable chassisData)
            : base(chassisData)
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
