using System.Windows;
using System.Windows.Controls;

namespace Smellyriver.TankInspector.Common.Wpf.Controls
{
    public class PanelService
    {
        public static Thickness GetChildrenMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ChildrenMarginProperty);
        }

        public static void SetChildrenMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(ChildrenMarginProperty, value);
        }

        public static readonly DependencyProperty ChildrenMarginProperty =
            DependencyProperty.RegisterAttached("ChildrenMargin", typeof(Thickness), typeof(PanelService), new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

        public static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Make sure this is put on a panel
            var panel = sender as Panel;

            if (panel == null) return;


            panel.Loaded += new RoutedEventHandler(panel_Loaded);

        }

        static void panel_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = sender as Panel;

            // Go over the children and set margin for them:
            foreach (var child in panel.Children)
            {
                var element = child as FrameworkElement;

                if (element == null) continue;

                element.Margin = PanelService.GetChildrenMargin(panel);
            }
        }


    }
}
