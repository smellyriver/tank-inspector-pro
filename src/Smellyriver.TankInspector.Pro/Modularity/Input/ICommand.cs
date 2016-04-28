using System;
using System.Windows.Media;
using IWpfCommand = System.Windows.Input.ICommand;

namespace Smellyriver.TankInspector.Pro.Modularity.Input
{
    public interface ICommand : IWpfCommand
    {
        Guid Guid { get; }
        string Name { get; }
        string Description { get; }
        int Priority { get; }
        ImageSource Icon { get; }
    }
}
