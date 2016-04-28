using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    class TaskNode : ITaskNode
    {
        private readonly Queue<InternalTaskInfo> _tasks;
        private readonly Dictionary<ITask, TaskNode> _taskNodes;

        public string Name { get; private set; }

        public TaskNode(string name)
        {
            this.Name = name;
            _tasks = new Queue<InternalTaskInfo>();
            _taskNodes = new Dictionary<ITask, TaskNode>();
        }

        public void Enqueue(TaskInfo taskInfo)
        {
            this.Enqueue(taskInfo.Task, taskInfo.Weight);
        }

        public void Enqueue(ITask task, double weight)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (weight < 0)
                throw new ArgumentException("weight");

            _tasks.Enqueue(new InternalTaskInfo(task, weight));
        }

        internal void Process(ProgressScope progress)
        {
            foreach (var taskInfo in _tasks)
            {
                var childProgress = progress.AddChildScope(taskInfo.Task.Name, taskInfo.Weight);
                App.BeginInvokeBackground(() => taskInfo.Task.PreProcess(childProgress));
                _taskNodes[taskInfo.Task].Process((ProgressScope)childProgress);
                App.BeginInvokeBackground(() => taskInfo.Task.PostProcess(childProgress));
                App.BeginInvokeBackground(childProgress.Dispose);
            }
        }

        internal void Initialize()
        {
            _taskNodes.Clear();
            foreach (var taskInfo in _tasks)
            {
                var taskNode = new TaskNode(taskInfo.Task.Name);
                taskInfo.Task.Initialize(taskNode);
                taskNode.Initialize();
                _taskNodes.Add(taskInfo.Task, taskNode);
            }
        }
    }
}
