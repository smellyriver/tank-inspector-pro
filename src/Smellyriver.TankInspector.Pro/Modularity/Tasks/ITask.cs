namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public interface ITask
    {
        string Name { get; }
        void Initialize(ITaskNode taskScope);
        void PreProcess(IProgressScope progress);
        void PostProcess(IProgressScope progress);
        void RunSynchronized(IProgressScope progress);
    }
}
