using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public abstract class CompositeTask : ITask
    {
        public abstract string Name { get; }

        protected abstract IEnumerable<TaskInfo> GetChildren();

        public virtual void Initialize(ITaskNode taskNode)
        {
            foreach (var task in this.GetChildren())
                taskNode.Enqueue(task);
        }


        public virtual void RunSynchronized(IProgressScope progress)
        {
            foreach (var child in this.GetChildren())
                child.Task.RunSynchronized(progress);
        }

        public virtual void PreProcess(IProgressScope progress)
        {
            
        }

        public virtual void PostProcess(IProgressScope progress)
        {
            
        }
    }
}
