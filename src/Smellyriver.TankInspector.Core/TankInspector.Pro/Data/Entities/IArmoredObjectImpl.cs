using System.Linq;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    static class IArmoredObjectImpl
    {
        public static bool GetIsPrimaryArmorDefined<T>(T armoredObject)
            where T : TankObject, IArmoredObject
        {
            return armoredObject.QueryValue("primaryArmor") != null;
        }

        public static double GetFrontArmor<T>(T armoredObject)
            where T : TankObject, IArmoredObject
        {
            return armoredObject.QueryDouble("primaryArmor/front");
        }

        public static double GetSideArmor<T>(T armoredObject)
            where T : TankObject, IArmoredObject
        {
            return armoredObject.QueryDouble("primaryArmor/side");
        }

        public static double GetRearArmor<T>(T armoredObject)
            where T : TankObject, IArmoredObject
        {
            return armoredObject.QueryDouble("primaryArmor/rear");
        }

        public static ArmorGroup GetArmorGroup<T>(T armoredObject, string key)
            where T : TankObject, IArmoredObject, IInternalArmoredObject
        {
            return armoredObject.ArmorGroups.GetOrCreate(key, () => IArmoredObjectImpl.ReadArmorGroup(armoredObject, key));
        }

        private static ArmorGroup ReadArmorGroup<T>(T armoredObject, string key)
             where T : TankObject, IArmoredObject, IInternalArmoredObject
        {
            var data = armoredObject.Query("armor/armor[@key='{0}']", key);
            if (data == null)
                return null;

            return new ArmorGroup(data);
        }

        public static double GetThickestArmor<T>(T armoredObject, bool spaced)
            where T : TankObject, IArmoredObject, IInternalArmoredObject
        {
            var values = IArmoredObjectImpl.GetArmorValues(armoredObject, spaced);

            if (values.Length == 0)
                return 0;
            else
                return values.AotSafeMax();
        }

        public static double GetThinnestArmor<T>(T armoredObject, bool spaced)
            where T : TankObject, IArmoredObject, IInternalArmoredObject
        {
            var values = IArmoredObjectImpl.GetArmorValues(armoredObject, spaced);

            if (values.Length == 0)
                return double.MaxValue;
            else
                return values.AotSafeMin();
        }

        public static double[] GetArmorValues<T>(T armoredObject, bool spaced)
            where T : TankObject, IArmoredObject, IInternalArmoredObject
        {
            var vehicleDamageFactor = spaced ? 0.0 : 1.0;
            return armoredObject.QueryMany("armor/armor")
                                .Where(t => t.QueryDouble("vehicleDamageFactor") == vehicleDamageFactor)
                                .Select(t => t.QueryDouble("thickness", -1))
                                .Where(a => a != -1)
                                .ToArray();
        }
    }
}
