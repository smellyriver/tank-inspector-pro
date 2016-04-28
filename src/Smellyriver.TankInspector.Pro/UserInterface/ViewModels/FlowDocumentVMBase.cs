using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Input;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Localization = Smellyriver.TankInspector.Pro.Globalization.Localization;
using WpfApplicationCommands = System.Windows.Input.ApplicationCommands;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    public abstract class FlowDocumentVMBase : NotificationObject
    {
        public static readonly string XpsFilter = Localization.Instance.L("document", "file_type_xps") + " (*.xps)|*.xps";
        public static readonly string RtfFilter = Localization.Instance.L("document", "file_type_rtf") + " (*.rtf)|*.rtf";
        public static readonly string TxtFilter = Localization.Instance.L("document", "file_type_txt") + " (*.txt)|*.txt";
        public static readonly string AllFilesFilter = Localization.Instance.L("common", "all_file_types_filter") + " (*.*)|*.*";

        public static readonly string UniversalFilter = string.Format("{0}|{1}|{2}|{3}",
                                                                      XpsFilter,
                                                                      RtfFilter,
                                                                      TxtFilter,
                                                                      AllFilesFilter);

        public abstract FlowDocument Document { get; protected set; }

        protected virtual string ExportFileName { get; set; }

        public FlowDocumentVMBase(CommandBindingCollection commandBindings)
        {
            commandBindings.Add(new CommandBinding(WpfApplicationCommands.SaveAs, this.ExecuteSaveAs));
            commandBindings.Add(new CommandBinding(FlowDocumentCommands.ExportXps, this.ExecuteExportXps));
            commandBindings.Add(new CommandBinding(FlowDocumentCommands.ExportRtf, this.ExecuteExportRtf));
            commandBindings.Add(new CommandBinding(FlowDocumentCommands.ExportTxt, this.ExecuteExportTxt));
        }


        private void ExecuteExportTxt(object sender, ExecutedRoutedEventArgs e)
        {
            this.Export(TxtFilter + "|" + AllFilesFilter, ".txt", ".txt");
        }

        private void ExecuteExportRtf(object sender, ExecutedRoutedEventArgs e)
        {
            this.Export(RtfFilter + "|" + AllFilesFilter, ".rtf", ".rtf");
        }

        private void ExecuteExportXps(object sender, ExecutedRoutedEventArgs e)
        {
            this.Export(XpsFilter + "|" + AllFilesFilter, ".xps", ".xps");
        }

        protected void Export(string filter, string defaultExtension, string forcedFileTypeExtension = null)
        {
            var fileName = this.ExportFileName;
            fileName = Path.ChangeExtension(fileName, defaultExtension);

            var result = DialogManager.Instance.ShowSaveFileDialog(title: this.L("common", "save_as"),
                                                                   fileName: ref fileName,
                                                                   filter: filter,
                                                                   filterIndex: 0,
                                                                   defaultExtensionName: defaultExtension,
                                                                   overwritePrompt: true,
                                                                   addExtension: true,
                                                                   checkPathExists: true);

            if (result == true)
            {
                this.ExportFileName = fileName;
                var extension = forcedFileTypeExtension ?? Path.GetExtension(fileName);
                if (extension == ".xps")
                {
                    this.Document.SaveAsXps(fileName);
                    return;
                }

                using (var file = File.Create(fileName))
                {
                    string dataFormat = DataFormats.XamlPackage;
                    if (extension == ".rtf")
                        dataFormat = DataFormats.Rtf;
                    else
                        dataFormat = DataFormats.Text;

                    var textRange = new TextRange(this.Document.ContentStart, this.Document.ContentEnd);
                    textRange.Save(file, dataFormat);
                }
            }
        }



        private void ExecuteSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            this.Export(UniversalFilter, ".xps");
        }
    }
}
