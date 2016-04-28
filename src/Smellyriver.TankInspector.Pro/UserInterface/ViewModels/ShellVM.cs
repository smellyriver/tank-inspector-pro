using System.Windows.Input;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    class ShellVM : NotificationObject
    {

        public MainMenuVM MainMenu { get; private set; }

        public MainDockingViewVM MainDockingView { get; private set; }

        public ApplicationSettingsVM ApplicationSettings { get; private set; }

        public StatusVM Status { get; private set; }


        private ModelSettingsVM _activeModelSettings;
        public ModelSettingsVM ActiveModelSettings
        {
            get { return _activeModelSettings; }
            private set
            {
                _activeModelSettings = value;
                this.RaisePropertyChanged(() => this.ActiveModelSettings);
            }
        }

        private readonly CommandBindingCollection _commandBindings;

        public ShellVM(CommandBindingCollection commandBindings)
        {
            _commandBindings = commandBindings;

            this.MainMenu = new MainMenuVM(this);
            this.MainDockingView = new MainDockingViewVM(this, commandBindings);
            this.Status = new StatusVM(this);
            this.ApplicationSettings = new ApplicationSettingsVM(this);
            
        }
        
    }
}
