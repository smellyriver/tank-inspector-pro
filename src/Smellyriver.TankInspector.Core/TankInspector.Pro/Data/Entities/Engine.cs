namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class Engine : Module
    {
        public double Power { get { return this.QueryDouble("power"); } }
        public double ChanceOnFire { get { return this.QueryDouble("fireStartingChance"); } }

        public Engine(IXQueryable engineData)
            : base(engineData)
        {
           
        }
    }
}
