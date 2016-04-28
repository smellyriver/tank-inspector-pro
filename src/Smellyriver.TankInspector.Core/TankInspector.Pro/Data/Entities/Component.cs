namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public abstract class Component : TankObject
    {
        protected Component(IXQueryable data)
            : base(data)
        {

        }
    }
}
