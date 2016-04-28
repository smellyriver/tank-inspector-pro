namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public sealed class TankCommandManager : CommandManagerBase<ITankCommand>
    {
        public static TankCommandManager Instance { get; private set; }

        static TankCommandManager()
        {
            TankCommandManager.Instance = new TankCommandManager();
        }

        private TankCommandManager()
        {

        }
    }
}
