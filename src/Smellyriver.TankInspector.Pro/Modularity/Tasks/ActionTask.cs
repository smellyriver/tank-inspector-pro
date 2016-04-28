using System;

namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public class ActionTask : ITask
    {

        public static ITask Create(string name, Action<IProgressScope> action)
        {
            return new ActionTask(name, action);
        }

        public static ITask Create(string name, Action action)
        {
            return new ActionTask(name, p => action());
        }

        public string Name { get; }

        private readonly Action<IProgressScope> _action;
        internal ActionTask(string name, Action<IProgressScope> action)
        {
            this.Name = name;
            _action = action;
        }

        public void Initialize(ITaskNode taskScope)
        {

        }

        public void PreProcess(IProgressScope progress)
        {
            _action(progress);
        }

        public void PostProcess(IProgressScope progress)
        {

        }

        public void RunSynchronized(IProgressScope progress)
        {
            _action(progress);
        }
    }
}
