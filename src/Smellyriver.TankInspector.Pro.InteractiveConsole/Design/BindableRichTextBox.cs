using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Design
{
    public static class BindableRichTextBox
    {
        public static readonly DependencyProperty BindableDocumentProperty = DependencyProperty.RegisterAttached("BindableDocument", typeof(FlowDocument), typeof(BindableRichTextBox), new UIPropertyMetadata(null, BindableRichTextBox.BindableDocumentPropertySet));

        public static FlowDocument GetBindableDocument(RichTextBox obj)
        {
            return (FlowDocument)obj.GetValue(BindableDocumentProperty);
        }

        public static void SetBindableDocument(RichTextBox obj, FlowDocument value)
        {
            obj.SetValue(BindableDocumentProperty, value);
        }

        private static void BindableDocumentPropertySet(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (RichTextBox)target;
            textBox.Document = e.NewValue as FlowDocument;
        }
    }
}
