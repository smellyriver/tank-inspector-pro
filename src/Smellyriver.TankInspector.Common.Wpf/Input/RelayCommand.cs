using System;

namespace Smellyriver.TankInspector.Common.Wpf.Input
{
    public class RelayCommand : RelayCommandBase
    {

        public static readonly Func<bool> AlwaysCanExecute = () => true;

        private Func<bool> _canExecute;
        private Action _action;


        public RelayCommand(Action action, Func<bool> canExecute)
        {
            _canExecute = canExecute;
            _action = action;
        }

        public RelayCommand(Action action)
            : this(action, RelayCommand.AlwaysCanExecute)
        {

        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public override void Execute(object parameter)
        {
            _action();
        }
    }

    public class RelayCommand<TArg> : RelayCommandBase
    {

        public static readonly Func<TArg, bool> AlwaysCanExecute = a => true;

        private Func<TArg, bool> _canExecute;
        private Action<TArg> _action;


        public RelayCommand(Action<TArg> action, Func<TArg, bool> canExecute)
        {
            _canExecute = canExecute;
            _action = action;
        }

        public RelayCommand(Action<TArg> action)
            : this(action, RelayCommand<TArg>.AlwaysCanExecute)
        {

        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute((TArg)parameter);
        }

        public override void Execute(object parameter)
        {
            _action((TArg)parameter);
        }
    }
}
