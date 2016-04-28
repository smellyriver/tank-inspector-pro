using System.Windows.Input;
using Smellyriver.TankInspector.Pro.Globalization;

namespace Smellyriver.TankInspector.Pro.Input
{
    public static class FlowDocumentCommands
    {
        public static readonly RoutedUICommand ExportXps 
            = new RoutedUICommand(Localization.Instance.L("document", "export_to_xps_document_menu_item"), 
                                  "exportXpsDocument", 
                                  typeof(FlowDocumentCommands));
        public static readonly RoutedUICommand ExportRtf 
            = new RoutedUICommand(Localization.Instance.L("document", "export_to_rtf_document_menu_item"), 
                                  "exportRtfDocument", 
                                  typeof(FlowDocumentCommands));
        public static readonly RoutedUICommand ExportTxt 
            = new RoutedUICommand(Localization.Instance.L("document", "export_to_text_file_menu_item"), 
                                  "exportTextFile", 
                                  typeof(FlowDocumentCommands));
    }
}
