using System.Reflection;
using System.Windows;

namespace Smellyriver.TankInspector.Pro.Globalization.Wpf
{
    public static class Loc
    {
        public static string GetCatalogName(DependencyObject obj)
        {
            return (string)obj.GetValue(CatalogNameProperty);
        }

        public static void SetCatalogName(DependencyObject obj, string value)
        {
            obj.SetValue(CatalogNameProperty, value);
        }

        public static readonly DependencyProperty CatalogNameProperty =
            DependencyProperty.RegisterAttached("CatalogName", 
                                                typeof(string), 
                                                typeof(Loc), 
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));



        public static Assembly GetAssembly(DependencyObject obj)
        {
            return (Assembly)obj.GetValue(AssemblyProperty);
        }

        public static void SetAssembly(DependencyObject obj, Assembly value)
        {
            obj.SetValue(AssemblyProperty, value);
        }

        public static readonly DependencyProperty AssemblyProperty =
            DependencyProperty.RegisterAttached("Assembly", 
                                                typeof(Assembly), 
                                                typeof(Loc), 
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));


    }
}
