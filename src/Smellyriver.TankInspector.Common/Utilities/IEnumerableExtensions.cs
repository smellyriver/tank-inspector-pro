using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class IEnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }
    }
}
