using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public interface ICommandManager<TCommand>
        where TCommand : ICommand
    {
        IEnumerable<TCommand> Commands { get; }
        void Register(TCommand command);
    }
}
