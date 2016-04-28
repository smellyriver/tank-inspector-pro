namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class UnlockInfo
    {
        public IModuleUnlockTarget Target { get; private set; }
        public double Experience { get; private set; }

        internal UnlockInfo(IModuleUnlockTarget target, double experience)
        {
            this.Target = target;
            this.Experience = experience;
        }
    }
}
