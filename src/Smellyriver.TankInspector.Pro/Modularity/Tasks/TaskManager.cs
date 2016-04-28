using System;
using System.Windows.Threading;

namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public class TaskManager
    {
        public static TaskManager RunTask(ITask task)
        {
            var taskManager = new TaskManager(task.Name);
            taskManager.Enqueue(task);
            taskManager.Process();
            return taskManager;
        }

        private readonly TaskNode _rootNode;

        public bool IsProcessing { get; private set; }

        public string Name { get; }

        public event EventHandler ProgressChanged;
        public event EventHandler StatusMessageChanged;
        public event EventHandler IsIndetermineChanged;

        public double Progress { get; private set; }
        public string StatusMessage { get; private set; }
        public string PrimaryStatusMessage { get; private set; }
        public bool IsIndeterminate { get; private set; }
        

        public DispatcherOperation CurrentOperation { get; private set; }

        public TaskManager(string name)
        {
            this.Name = name;
            _rootNode = new TaskNode(this.Name);
        }

        public void Enqueue(ITask task)
        {
            if (this.IsProcessing)
                throw new InvalidOperationException("cannot enqueue task while processing");

            _rootNode.Enqueue(task, 10.0);
        }


        public DispatcherOperation Process()
        {
            this.Initialize();

            this.IsProcessing = true;

            var progressScope = new RootProgressScope(this.Name);
            progressScope.ProgressChanged += progressScope_ProgressChanged;
            progressScope.StatusMessageChanged += progressScope_StatusMessageChanged;
            progressScope.IndetermineChanged += progressScope_IndetermineChanged;

            _rootNode.Process(progressScope);

            this.CurrentOperation = App.BeginInvokeBackground(() =>
                {
                    progressScope.Dispose(); 
                    this.IsProcessing = false;

                    progressScope.ProgressChanged -= progressScope_ProgressChanged;
                    progressScope.StatusMessageChanged -= progressScope_StatusMessageChanged;
                    progressScope.IndetermineChanged -= progressScope_IndetermineChanged;

                    this.CurrentOperation = null;
                });

            return this.CurrentOperation;
        }

        private void progressScope_StatusMessageChanged(object sender, EventArgs e)
        {
            var progress = (ProgressScope)sender;
            this.StatusMessage = progress.StatusMessage;
            this.PrimaryStatusMessage = progress.PrimaryStatusMessage;

            if (this.StatusMessageChanged != null)
                this.StatusMessageChanged(this, e);
        }

        private void progressScope_IndetermineChanged(object sender, EventArgs e)
        {
            this.IsIndeterminate = ((ProgressScope)sender).IsIndeterminate;
            if (this.IsIndetermineChanged != null)
                this.IsIndetermineChanged(this, e);
        }

        void progressScope_ProgressChanged(object sender, EventArgs e)
        {
            this.Progress = ((ProgressScope)sender).Progress;
            if (this.ProgressChanged != null)
                this.ProgressChanged(this, e);
        }

        private void Initialize()
        {
            _rootNode.Initialize();
        }
    }
}
