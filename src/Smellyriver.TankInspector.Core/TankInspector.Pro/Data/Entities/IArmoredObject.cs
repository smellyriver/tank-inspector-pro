namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public interface IArmoredObject 
    {
        ArmorGroup GetArmorGroup(string key);
        double GetThickestArmor(bool spaced);
        double GetThinnestArmor(bool spaced);
        double[] GetArmorValues(bool spaced);
    }
}
