using System.Windows;
using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Design
{
    /// <summary>
    /// Provides attached dependency properties for automatically deferring focus from one UI element to another.
    /// </summary>
    public class FocusDeferral
    {
        public static readonly DependencyProperty DeferFocusOnKeydownProperty = DependencyProperty.RegisterAttached("DeferFocusOnKeydown", typeof(object), typeof(FocusDeferral), new UIPropertyMetadata(null, FocusDeferral.DeferFocusOnKeydownPropertySet));
        public static readonly DependencyProperty DeferFocusOnClickProperty = DependencyProperty.RegisterAttached("DeferFocusOnClick", typeof(object), typeof(FocusDeferral), new UIPropertyMetadata(null, FocusDeferral.DeferFocusOnClickPropertySet));

        public static object GetDeferFocusOnClick(UIElement obj)
        {
            return obj.GetValue(DeferFocusOnClickProperty);
        }

        public static void SetDeferFocusOnClick(UIElement obj, object value)
        {
            obj.SetValue(DeferFocusOnClickProperty, value);
        }

        public static object GetDeferFocusOnKeydown(UIElement obj)
        {
            return obj.GetValue(DeferFocusOnKeydownProperty);
        }

        public static void SetDeferFocusOnKeydown(UIElement obj, object value)
        {
            obj.SetValue(DeferFocusOnKeydownProperty, value);
        }

        private static void DeferFocusOnKeydownPropertySet(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)target;
            element.KeyDown -= FocusDeferral.Element_KeyDown;
            if (e.NewValue != null)
            {
                element.KeyDown += FocusDeferral.Element_KeyDown;
            }
        }

        private static void DeferFocusOnClickPropertySet(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)target;
            element.PreviewMouseUp -= FocusDeferral.Element_MouseDown;
            if (e.NewValue != null)
            {
                element.PreviewMouseUp += FocusDeferral.Element_MouseDown;
            }
        }

        private static void Element_KeyDown(object sender, KeyEventArgs e)
        {
            var sendingElement = (UIElement)sender;
            if (Keyboard.Modifiers != ModifierKeys.None) return; 
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right) return;

            var elementToFocus = FocusDeferral.GetDeferFocusOnKeydown(sendingElement) as UIElement;
            if (elementToFocus != null)
            {
                elementToFocus.Focus();
            }
        }

        private static void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var sendingElement = (UIElement) sender;
            if (!sendingElement.IsMouseDirectlyOver) return;

            var elementToFocus = FocusDeferral.GetDeferFocusOnClick(sendingElement) as UIElement;
            if (elementToFocus != null)
            {
                elementToFocus.Focus();
            }
        }
    }
}