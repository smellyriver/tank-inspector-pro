using System;
using System.Threading.Tasks;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Pro.UserInterface.ViewModels;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public class LoadingManager
    {
        public static LoadingManager Instance { get; private set; }

        static LoadingManager()
        {
            LoadingManager.Instance = new LoadingManager();
        }

        private readonly TaskManager _taskManager;

        internal SplashVM SplashVM { get; set; }

        private LoadingManager()
        {
            _taskManager = new TaskManager("loading");
            _taskManager.ProgressChanged += _taskManager_ProgressChanged;
            _taskManager.StatusMessageChanged += _taskManager_StatusMessageChanged;
            _taskManager.IsIndetermineChanged += _taskManager_IsIndetermineChanged;
        }

        public void Enqueue(ITask task)
        {
            _taskManager.Enqueue(task);
        }

        public Task BeginLoad()
        {
            var processOperation = _taskManager.Process();
            return Task.Factory.StartNew(() => processOperation.Wait());
        }

        void _taskManager_IsIndetermineChanged(object sender, EventArgs e)
        {
            this.SplashVM.LoadingIsIndeterminate = _taskManager.IsIndeterminate;
        }

        void _taskManager_StatusMessageChanged(object sender, EventArgs e)
        {
            this.SplashVM.LoadingStatusText = _taskManager.StatusMessage;
        }

        void _taskManager_ProgressChanged(object sender, EventArgs e)
        {
            this.SplashVM.LoadingProgress = _taskManager.Progress;
        }
    }
}
