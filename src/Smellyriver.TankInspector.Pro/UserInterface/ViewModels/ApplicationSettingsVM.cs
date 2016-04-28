using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    
    class ApplicationSettingsVM : NotificationObject
    {

        public bool ShowFPS
        {
            get { return ApplicationSettings.Default.ShowFPS; }
            set
            {
                ApplicationSettings.Default.ShowFPS = value;
                ApplicationSettings.Default.Save();
                this.RaisePropertyChanged(() => this.ShowFPS);
            }
        }

        public bool ShowTriangleCount
        {
            get { return ApplicationSettings.Default.ShowTriangleCount; }
            set
            {
                ApplicationSettings.Default.ShowTriangleCount = value;
                ApplicationSettings.Default.Save();
                this.RaisePropertyChanged(() => this.ShowTriangleCount);
            }
        }

        public bool ShowStatusBar
        {
            get { return ApplicationSettings.Default.ShowStatusBar; }
            set
            {
                ApplicationSettings.Default.ShowStatusBar = value;
                ApplicationSettings.Default.Save();
                this.RaisePropertyChanged(() => this.ShowStatusBar);
            }
        }

        public ShellVM Shell { get; private set; }

        public ApplicationSettingsVM(ShellVM shell)
        {
            this.Shell = shell;
        }
    }
}
