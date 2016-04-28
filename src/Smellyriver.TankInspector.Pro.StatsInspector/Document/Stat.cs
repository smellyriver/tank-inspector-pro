using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Smellyriver.TankInspector.Pro.StatsInspector.Document
{
    public static class Stat
    {
        

        public static string GetKey(DependencyObject obj)
        {
            return (string)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, string value)
        {
            obj.SetValue(KeyProperty, value);
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(string), typeof(Stat), new PropertyMetadata(Stat.OnKeyChanged));

        private static readonly Binding s_shouldShowBinding = new Binding("ShouldShow");

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as TextElement;
            if(element!=null)
            {
                element.SetBinding(Template.ShouldShowProperty, s_shouldShowBinding);
            }
        }



        public static string GetTemplateKey(DependencyObject obj)
        {
            return (string)obj.GetValue(TemplateKeyProperty);
        }

        public static void SetTemplateKey(DependencyObject obj, string value)
        {
            obj.SetValue(TemplateKeyProperty, value);
        }

        public static readonly DependencyProperty TemplateKeyProperty =
            DependencyProperty.RegisterAttached("TemplateKey", typeof(string), typeof(Stat), new PropertyMetadata(null));

        internal static void ApplyTemplate(IAddChild decendant, IEnumerable items)
        {
            foreach (var item in items)
                decendant.AddChild(item);
        }
    }
}
