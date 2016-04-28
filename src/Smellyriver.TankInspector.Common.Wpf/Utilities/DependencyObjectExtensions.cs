using System;
using System.ComponentModel;
using System.Windows;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class DependencyObjectExtensions
    {
        public static void AddPropertyChangedHandler<T>(this T target, DependencyProperty property, EventHandler handler)
        {
            var descriptor = DependencyPropertyDescriptor.FromProperty(property, typeof(T));
            if (descriptor == null)
                throw new ArgumentException();

            descriptor.AddValueChanged(target, handler);
        }

        public static void RemovePropertyChangedHandler<T>(this T target, DependencyProperty property, EventHandler handler)
        {
            var descriptor = DependencyPropertyDescriptor.FromProperty(property, typeof(T));
            if (descriptor == null)
                throw new ArgumentException();

            descriptor.RemoveValueChanged(target, handler);
        }
    }
}
