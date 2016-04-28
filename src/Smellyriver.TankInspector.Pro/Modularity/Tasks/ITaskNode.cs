namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public interface ITaskNode
    {
        void Enqueue(ITask task, double weight);
        void Enqueue(TaskInfo taskInfo);
    }
}
