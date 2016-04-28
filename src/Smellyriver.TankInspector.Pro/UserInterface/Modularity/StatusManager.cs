using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity
{
    public class StatusManager : NotificationObject
    {
        public static StatusManager Instance { get; private set; }

        static StatusManager()
        {
            StatusManager.Instance = new StatusManager();
        }

        internal ObservableCollection<TaskManager> TaskManagers { get; }

        private readonly Timer _statusUpdateTimer;
        private TaskManager _activeTaskManager;

        private bool _hasActiveTaskInProgress;
        public bool HasActiveTaskInProgress
        {
            get { return _hasActiveTaskInProgress; }
            private set
            {
                _hasActiveTaskInProgress = value;
                this.RaisePropertyChanged(() => this.HasActiveTaskInProgress);
            }
        }


        private double _activeTaskProgress;
        public double ActiveTaskProgress
        {
            get { return _activeTaskProgress; }
            private set
            {
                _activeTaskProgress = value;
                this.RaisePropertyChanged(() => this.ActiveTaskProgress);
            }
        }

        private string _activeTaskStatusMessage;
        public string ActiveTaskStatusMessage
        {
            get { return _activeTaskStatusMessage; }
            private set
            {
                _activeTaskStatusMessage = value;
                this.RaisePropertyChanged(() => this.ActiveTaskStatusMessage);
            }
        }

        private bool _activeTaskIsIndeterminate;
        public bool ActiveTaskIsIndeterminate
        {
            get { return _activeTaskIsIndeterminate; }
            private set
            {
                _activeTaskIsIndeterminate = value;
                this.RaisePropertyChanged(() => this.ActiveTaskIsIndeterminate);
            }
        }


        private StatusManager()
        {
            this.TaskManagers = new ObservableCollection<TaskManager>();
            _statusUpdateTimer = new Timer(2000);
            _statusUpdateTimer.AutoReset = true;
            _statusUpdateTimer.Elapsed += _statusUpdateTimer_Elapsed;
            _statusUpdateTimer.Start();
        }

        void _statusUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.HasActiveTaskInProgress = _activeTaskManager != null;
            if (_activeTaskManager != null)
            {
                this.ActiveTaskProgress = _activeTaskManager.Progress;
                this.ActiveTaskStatusMessage = _activeTaskManager.PrimaryStatusMessage;
                this.ActiveTaskIsIndeterminate = _activeTaskManager.IsIndeterminate;
            }
            else
            {
                this.ActiveTaskProgress = 1.0;
                this.ActiveTaskStatusMessage = this.L("shell", "state_ready");
                this.ActiveTaskIsIndeterminate = false;
            }
        }

        public void AddTask(ITask task)
        {
            var taskManager = TaskManager.RunTask(task);
            this.TaskManagers.Add(taskManager);
            taskManager.IsIndetermineChanged += taskManager_IsIndetermineChanged;
            taskManager.ProgressChanged += taskManager_ProgressChanged;
            taskManager.StatusMessageChanged += taskManager_StatusMessageChanged;
        }

        private void RemoveTaskManager(TaskManager taskManager)
        {
            this.TaskManagers.Remove(taskManager);
            taskManager.IsIndetermineChanged -= taskManager_IsIndetermineChanged;
            taskManager.ProgressChanged -= taskManager_ProgressChanged;
            taskManager.StatusMessageChanged -= taskManager_StatusMessageChanged;

            if (_activeTaskManager == taskManager)
                _activeTaskManager = this.TaskManagers.FirstOrDefault();
        }

        void taskManager_StatusMessageChanged(object sender, EventArgs e)
        {
            _activeTaskManager = (TaskManager)sender;
        }

        void taskManager_ProgressChanged(object sender, EventArgs e)
        {
            var taskManager = (TaskManager)sender;
            _activeTaskManager = taskManager;

            if (ProgressScope.ProgressEquals(taskManager.Progress, 1.0))
                this.RemoveTaskManager(taskManager);
        }

        void taskManager_IsIndetermineChanged(object sender, EventArgs e)
        {
            _activeTaskManager = (TaskManager)sender;
        }
    }
}
