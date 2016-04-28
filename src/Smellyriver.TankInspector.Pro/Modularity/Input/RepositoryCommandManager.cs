namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public sealed class RepositoryCommandManager : CommandManagerBase<IRepositoryCommand>
    {
        public static RepositoryCommandManager Instance { get; private set; }

        static RepositoryCommandManager()
        {
            RepositoryCommandManager.Instance = new RepositoryCommandManager();
        }
    }
}
