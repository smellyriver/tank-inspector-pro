using System;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public abstract class CommandBase<TCommand, TArgument>
        where TCommand : ICommand
    {

        private static readonly Func<TArgument, bool> s_alwaysExecutable = o => true;

        private readonly Guid _guid;
        public Guid Guid
        {
            get { return _guid; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly string _description;
        public string Description
        {
            get { return _description; }
        }

        private readonly int _priority;
        public int Priority
        {
            get { return _priority; }
        }

        private readonly ImageSource _icon;
        public ImageSource Icon
        {
            get { return _icon; }
        }

        private Func<TArgument, bool> _canExecute;
        public bool CanExecute(object parameter)
        {
            return _canExecute((TArgument)parameter);
        }

        public event EventHandler CanExecuteChanged;

        private Action<TArgument> _execute;
        public void Execute(object parameter)
        {
            _execute((TArgument)parameter);
        }

        public CommandBase(Guid guid,
                           string name,
                           Action<TArgument> execute,
                           Func<TArgument, bool> canExecute = null,
                           string description = null,
                           ImageSource icon = null,
                           int priority = 0)
        {
            _guid = guid;
            _name = name;
            _execute = execute;
            _canExecute = canExecute ?? s_alwaysExecutable;
            _description = description;
            _icon = icon;
            _priority = priority;
        }

        public CommandBase(Guid guid,
                           string name,
                           ICommand command,
                           string description = null,
                           ImageSource icon = null,
                           int priority = 0)
            : this(guid,
                   name,
                   k => command.Execute(k),
                   k => command.CanExecute(k),
                   description,
                   icon,
                   priority)
        {
            command.CanExecuteChanged += command_CanExecuteChanged;
        }

        void command_CanExecuteChanged(object sender, EventArgs e)
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, e);
        }
    }
}
