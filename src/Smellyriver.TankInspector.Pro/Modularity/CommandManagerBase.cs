using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public abstract class CommandManagerBase<TCommand>
        where TCommand : ICommand
    {
        

        private readonly Dictionary<Guid, TCommand> _commands;

        public IEnumerable<TCommand> Commands
        {
            get { return _commands.Values.OrderBy(c => c.Priority); }
        }

        public CommandManagerBase()
        {
            _commands = new Dictionary<Guid, TCommand>();
        }

        public void Register(TCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            TCommand existedCommand;
            if (_commands.TryGetValue(command.Guid, out existedCommand))
            {
                this.LogWarning("a command with Guid '{0}' (Name='{1}') is already existed. it will be replaced by command with Guid '{2}' (Name='{3}')",
                               existedCommand.Guid, existedCommand.Name, command.Guid, command.Name);
            }

            _commands[command.Guid] = command;
        }

    }
}
