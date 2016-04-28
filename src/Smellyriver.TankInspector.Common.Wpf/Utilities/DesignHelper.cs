using System.ComponentModel;
using System.Windows;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class DesignHelper
    {
        private static readonly DependencyObject s_dependencyObject = new DependencyObject();

        public static bool IsInDesignTime
        {
            get { return DesignerProperties.GetIsInDesignMode(s_dependencyObject); }
        }
    }
}
