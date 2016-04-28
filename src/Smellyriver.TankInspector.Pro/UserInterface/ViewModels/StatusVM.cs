using System.Collections.Specialized;
using System.ComponentModel;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    class StatusVM : NotificationObject
    {

        public bool IsBackgroundTaskCountShown
        {
            get { return this.BackgroundTaskCount > 1; }
        }

        public bool IsProgressShown
        {
            get { return StatusManager.Instance.HasActiveTaskInProgress ; }
        }

        public int BackgroundTaskCount
        {
            get { return StatusManager.Instance.TaskManagers.Count; }
        }

        public string StatusText
        {
            get { return StatusManager.Instance.ActiveTaskStatusMessage; }
        }

        public double Progress
        {
            get { return StatusManager.Instance.ActiveTaskProgress; }
        }

        public bool IsIndeterminate
        {
            get { return StatusManager.Instance.ActiveTaskIsIndeterminate;}
        }

        public ShellVM Shell { get; private set; }

        public StatusVM(ShellVM shell)
        {
            this.Shell = shell;
            StatusManager.Instance.PropertyChanged += StatusManager_PropertyChanged;
            StatusManager.Instance.TaskManagers.CollectionChanged += TaskManagers_CollectionChanged;
        }

        void TaskManagers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.BackgroundTaskCount);
            this.RaisePropertyChanged(() => this.IsBackgroundTaskCountShown);
        }

        void StatusManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "HasActiveTaskInProgress":
                    this.RaisePropertyChanged(() => this.IsProgressShown);
                    break;
                case "ActiveTaskProgress":
                    this.RaisePropertyChanged(() => this.Progress);
                    break;
                case "ActiveTaskStatusMessage":
                    this.RaisePropertyChanged(() => this.StatusText);
                    break;
                case "ActiveTaskIsIndeterminate":
                    this.RaisePropertyChanged(() => this.IsIndeterminate);
                    break;
            }
        }
    }
}
