using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Design
{
    public static class RichTextBoxWrapping
    {
        public static readonly DependencyProperty TextWrappingBasedOnTextProperty = DependencyProperty.RegisterAttached("TextWrappingBasedOnText", typeof(string), typeof(RichTextBoxWrapping), new UIPropertyMetadata(string.Empty, RichTextBoxWrapping.Remeasure));
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.RegisterAttached("TextWrapping", typeof(TextWrapping), typeof(RichTextBoxWrapping), new UIPropertyMetadata(TextWrapping.NoWrap, RichTextBoxWrapping.Remeasure));

        public static string GetTextWrappingBasedOnText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextWrappingBasedOnTextProperty);
        }

        public static void SetTextWrappingBasedOnText(DependencyObject obj, string value)
        {
            obj.SetValue(TextWrappingBasedOnTextProperty, value);
        }

        public static TextWrapping GetTextWrapping(DependencyObject obj)
        {
            return (TextWrapping)obj.GetValue(TextWrappingProperty);
        }

        public static void SetTextWrapping(DependencyObject obj, TextWrapping value)
        {
            obj.SetValue(TextWrappingProperty, value);
        }

        private static void Remeasure(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = target as RichTextBox;
            if (richTextBox == null) return;

            var flowDocument = richTextBox.Document;
            if (flowDocument == null) return;

            if (RichTextBoxWrapping.GetTextWrapping(target) == TextWrapping.NoWrap)
            {
                var text = RichTextBoxWrapping.GetTextWrappingBasedOnText(target) ?? string.Empty;
                var formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, flowDocument.FlowDirection, new Typeface(flowDocument.FontFamily, flowDocument.FontStyle, flowDocument.FontWeight, flowDocument.FontStretch), flowDocument.FontSize, flowDocument.Foreground);
                var width = Math.Min(formattedText.Width + 100, 70000);
                flowDocument.PageWidth = width;
                richTextBox.MinWidth = width;
            }
            else
            {
                flowDocument.PageWidth = double.NaN;
                richTextBox.MinWidth = 0;
                richTextBox.Width = double.NaN;
            }
        }
    }
}
