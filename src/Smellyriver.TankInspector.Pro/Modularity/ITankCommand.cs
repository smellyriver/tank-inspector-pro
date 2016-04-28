using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public interface ITankCommand : ICommand
    {
        Guid Guid { get; }
        string Name { get; }
        string Description { get; }
        int Priority { get; }
        ImageSource Icon { get; }
    }
}
