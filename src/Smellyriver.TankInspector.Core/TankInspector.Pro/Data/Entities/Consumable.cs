namespace Smellyriver.TankInspector.Pro.Data.Entities
{

    public sealed class Consumable : Accessory
    {
        public Consumable(IXQueryable consumableData)
            : base(consumableData)
        {

        }
    }
}
