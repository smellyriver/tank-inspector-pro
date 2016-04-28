using System.Windows;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    interface IAvailableStatsTreeItem
    {
        string Name { get; }
        string Description { get; }
        Visibility Visibility { get; }
    }
}
