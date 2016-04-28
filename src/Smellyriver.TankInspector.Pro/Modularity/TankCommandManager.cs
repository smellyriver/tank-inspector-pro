using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    [Export(typeof(ITankCommandManager))]
    public class TankCommandManager : ITankCommandManager
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        private readonly Dictionary<Guid, ITankCommand> _commands;

        public IEnumerable<ITankCommand> Commands
        {
            get { return _commands.Values.OrderBy(c => c.Priority); }
        }

        public TankCommandManager()
        {
            _commands = new Dictionary<Guid, ITankCommand>();
        }

        public void Register(ITankCommand command)
        {
            if(command == null)
                throw new ArgumentNullException("command");

            ITankCommand existedCommand;
            if(_commands.TryGetValue(command.Guid, out existedCommand))
            {
                log.WarnFormat("a tank command with Guid '{0}' (Name='{1}') is already existed. it will be replaced by command with Guid '{2}' (Name='{3}')",
                               existedCommand.Guid, existedCommand.Name, command.Guid, command.Name);
            }

            _commands[command.Guid] = command;
        }

    }
}
