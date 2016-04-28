using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.Repository
{
    partial class RepositoryManager
    {
        internal class MarkerManagerImpl
        {
            private readonly List<Color> _availableColors;
            public IEnumerable<Color> AvailableColors { get { return _availableColors; } }

            public MarkerManagerImpl()
            {
                _availableColors = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static)
                                                 .Where(p => p.PropertyType == typeof(Color))
                                                 .Select(p => p.GetValue(null, null))
                                                 .Cast<Color>()
                                                 .Where(c => c.GetLuminance() <= 0.4)
                                                 .ToList();
            }

            public Color ApplyForColor(Color? color = null)
            {
                if (color != null)
                {
                    if (_availableColors.Contains(color.Value))
                    {
                        _availableColors.Remove(color.Value);
                        return color.Value;
                    }
                }

                var result = _availableColors.GetRandomElement();
                _availableColors.Remove(result);
                return result;
            }
        }
    }
}
