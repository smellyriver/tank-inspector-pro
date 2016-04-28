namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public class TurretYawLimits : XQueryableWrapper
    {

        public double Left { get { return this.QueryDouble("left"); } }
        public double Right { get { return this.QueryDouble("right"); } }

        public double Range { get { return this.Right - this.Left; } }

        public TurretYawLimits(IXQueryable data)
            : base(data)
        {

        }
    }
}
