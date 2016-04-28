using System;

namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public struct TaskInfo
    {
        public static TaskInfo FromAction(string name, double weight, Action<IProgressScope> action)
        {
            return new TaskInfo(new ActionTask(name, action), weight);
        }

        public double Weight { get; set; }
        public ITask Task { get; set; }

        public TaskInfo(ITask task, double weight)
            : this()
        {
            this.Task = task;
            this.Weight = weight;
        }
    }
}
