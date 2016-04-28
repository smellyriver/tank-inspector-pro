using System;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public class RepositoryCommand : CommandBase<IRepositoryCommand, string>, IRepositoryCommand
    {
        public RepositoryCommand(Guid guid,
                                 string name,
                                 Action<string> execute,
                                 Func<string, bool> canExecute = null,
                                 string description = null,
                                 ImageSource icon = null,
                                 int priority = 0)
            : base(guid, name, execute, canExecute, description, icon, priority)
        {

        }

        public RepositoryCommand(Guid guid,
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
