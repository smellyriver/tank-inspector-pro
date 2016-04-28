namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public sealed class NationCommandManager : CommandManagerBase<INationCommand>
    {
        public static NationCommandManager Instance { get; private set; }

        static NationCommandManager()
        {
            NationCommandManager.Instance = new NationCommandManager();
        }

    }
}
