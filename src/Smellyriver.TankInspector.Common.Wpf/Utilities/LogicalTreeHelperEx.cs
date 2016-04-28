using System.Collections.Generic;
using System.Windows;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class LogicalTreeHelperEx
    {
        public static IEnumerable<T> GetDecendants<T>(DependencyObject parent)
            where T : DependencyObject
        {
            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is T)
                    yield return (T)child;

                if (child is DependencyObject)
                {
                    foreach (var grandChild in GetDecendants<T>((DependencyObject)child))
                        yield return grandChild;
                }
            }
        }
    }
}
