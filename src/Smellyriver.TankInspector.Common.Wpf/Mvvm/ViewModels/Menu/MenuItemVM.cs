using System.Windows.Input;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu
{
    public class MenuItemVM : MenuItemContainerVM
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        private ImageSource _icon;
        public ImageSource Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                this.RaisePropertyChanged(() => this.Icon);
            }
        }

        private ICommand _command;
        public ICommand Command
        {
            get { return _command; }
            set
            {
                _command = value;
                this.RaisePropertyChanged(() => this.Command);
            }
        }

        private object _commandParameter;
        public object CommandParameter
        {
            get { return _commandParameter; }
            set
            {
                _commandParameter = value;
                this.RaisePropertyChanged(() => this.CommandParameter);
            }
        }

        private bool _isCheckable;
        public bool IsCheckable
        {
            get { return _isCheckable; }
            set
            {
                _isCheckable = value;
                this.RaisePropertyChanged(() => this.IsCheckable);
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                this.RaisePropertyChanged(() => this.IsChecked);
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                this.RaisePropertyChanged(() => this.IsEnabled);
            }
        }

        public MenuItemVM(string name)
            : this(name, null, null)
        {

        }

        public MenuItemVM(string name, ICommand command)
            : this(name, command, null)
        {

        }

        public MenuItemVM(string name, ICommand command, object commandParameter)
        {
            this.Name = name;
            this.Command = command;
            this.CommandParameter = commandParameter ?? this;
            this.IsEnabled = true;
        }
    }
}
