namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class Radio : Module
    {
        public double SignalDistance { get { return this.QueryDouble("distance"); } }

        public Radio(IXQueryable radioData)
            : base(radioData)
        {

        }
    }
}
