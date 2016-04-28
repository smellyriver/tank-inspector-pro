using System.Windows;
using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.StatsInspector.Document
{
    public static class View
    {

        public static FlowDocumentReaderViewingMode GetSuggestedViewingMode(DependencyObject obj)
        {
            return (FlowDocumentReaderViewingMode)obj.GetValue(SuggestedViewingModeProperty);
        }

        public static void SetSuggestedViewingMode(DependencyObject obj, FlowDocumentReaderViewingMode value)
        {
            obj.SetValue(SuggestedViewingModeProperty, value);
        }

        public static readonly DependencyProperty SuggestedViewingModeProperty =
            DependencyProperty.RegisterAttached("SuggestedViewingMode", typeof(FlowDocumentReaderViewingMode), typeof(View), new PropertyMetadata(FlowDocumentReaderViewingMode.TwoPage));


    }
}
