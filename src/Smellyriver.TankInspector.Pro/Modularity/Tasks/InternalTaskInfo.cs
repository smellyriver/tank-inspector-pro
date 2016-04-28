namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{

    internal struct InternalTaskInfo
    {
        public double Weight { get; set; }
        public ITask Task { get; set; }

        public InternalTaskInfo(ITask task, double weight)
            : this()
        {
            this.Task = task;
            this.Weight = weight;
        }
    }
}
