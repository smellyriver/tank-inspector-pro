using System;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public class TankCommand : CommandBase<ITankCommand, TankUnikey>, ITankCommand
    {
        public TankCommand(Guid guid, 
                           string name, 
                           Action<TankUnikey> execute, 
                           Func<TankUnikey, bool> canExecute = null, 
                           string description = null, 
                           ImageSource icon = null, 
                           int priority = 0)
            : base(guid, name, execute, canExecute, description, icon, priority)
        {

        }

        public TankCommand(Guid guid,
                           string name,
                           ICommand command,
                           string description = null,
                           ImageSource icon = null,
                           int priority = 0)
            : base(guid, name, command, description, icon, priority)
        {

        }

    }
}
