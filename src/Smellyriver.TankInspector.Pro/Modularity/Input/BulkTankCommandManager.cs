namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public sealed class BulkTankCommandManager : CommandManagerBase<IBulkTankCommand>
    {
        public static BulkTankCommandManager Instance { get; private set; }

        static BulkTankCommandManager()
        {
            BulkTankCommandManager.Instance = new BulkTankCommandManager();
        }

        private BulkTankCommandManager()
        {

        }
    }
}
