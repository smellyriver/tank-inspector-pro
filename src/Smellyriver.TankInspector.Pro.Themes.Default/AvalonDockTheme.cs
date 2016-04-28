using System;
using ADTheme = Xceed.Wpf.AvalonDock.Themes.Theme;

namespace Smellyriver.TankInspector.Pro.Themes.Default
{
    public class AvalonDockTheme : ADTheme
    {
        public override Uri GetResourceUri()
        {
            return new Uri(
                "/Smellyriver.TankInspector.Pro.Themes.Default;component/AvalonDockTheme.xaml",
                UriKind.Relative);
        }
    }
}
