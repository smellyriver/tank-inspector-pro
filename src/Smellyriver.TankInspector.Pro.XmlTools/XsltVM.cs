using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;

namespace Smellyriver.TankInspector.Pro.XmlTools
{
    class XsltVM : NotificationObject
    {
        public XmlToolsVM Owner { get; }

        private string _xslFilePath;
        public string XslFilePath
        {
            get { return _xslFilePath; }
            set
            {
                _xslFilePath = value;
                this.RaisePropertyChanged(() => this.XslFilePath);
                this.TransformCommand.NotifyCanExecuteChanged();
            }
        }

        private string _outputFilePath;
        public string OutputFilePath
        {
            get { return _outputFilePath; }
            set
            {
                _outputFilePath = value;
                this.RaisePropertyChanged(() => this.OutputFilePath);
                this.TransformCommand.NotifyCanExecuteChanged();
            }
        }

        private bool _openOutputFileAfterTransform;
        public bool OpenOutputFileAfterTransform
        {
            get { return _openOutputFileAfterTransform; }
            set
            {
                _openOutputFileAfterTransform = value;
                this.RaisePropertyChanged(() => this.OpenOutputFileAfterTransform);
            }
        }

        public RelayCommand BrowseXslFileCommand { get; private set; }
        public RelayCommand BrowseOutputFileCommand { get; private set; }
        public RelayCommand TransformCommand { get; }

        public XsltVM(XmlToolsVM owner)
        {
            this.Owner = owner;

            this.BrowseXslFileCommand = new RelayCommand(this.BrowseXslFile);
            this.BrowseOutputFileCommand = new RelayCommand(this.BrowseOutputFile);
            this.TransformCommand = new RelayCommand(this.Transform, this.CanTransform);
        }

        private bool CanTransform()
        {
            return File.Exists(this.XslFilePath) && !string.IsNullOrWhiteSpace(this.OutputFilePath);
        }

        private void Transform()
        {
            var xslt = new XslCompiledTransform();

            try
            {
                xslt.Load(this.XslFilePath);
            }
            catch (XsltException ex)
            {
                DialogManager.Instance.ShowMessageAsync(this.L("xml_tools", "error_loading_xsl_file_message_title"), ex.Message);
                return;
            }

            try
            {
                var outputSettings = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    ConformanceLevel = ConformanceLevel.Fragment,
                };

                using (var writer = XmlWriter.Create(this.OutputFilePath, outputSettings))
                {
                    using (var stream = this.Owner.XmlViewer.XmlText.ToStream())
                    {
                        using (var reader = XmlReader.Create(stream))
                        {
                            xslt.Transform(reader, writer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.Instance.ShowMessageAsync(this.L("xml_tools", "error_processing_xsl_transform_message_title"), ex.Message);
                return;
            }


            if (this.OpenOutputFileAfterTransform)
            {
                try
                {
                    var processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = this.OutputFilePath;
                    processStartInfo.UseShellExecute = true;
                    processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    Process.Start(processStartInfo);
                }
                catch (Exception ex)
                {
                    DialogManager.Instance.ShowMessageAsync(this.L("xml_tools", "error_opening_output_file_message_title"), ex.Message);
                    return;
                }
            }
            else
            {
                DialogManager.Instance.ShowMessageAsync(this.L("xml_tools", "transform_complete_message_title"),
                                                        this.L("xml_tools", "transform_complete_message", this.OutputFilePath));
            }

        }

        private void BrowseOutputFile()
        {
            var fileName = this.OutputFilePath;
            var filter = string.Format("{0}(*.*)|*.*", this.L("common", "all_file_types_filter"));

            if (DialogManager.Instance.ShowSaveFileDialog(title: this.L("xml_tools", "save_transform_output_as_dialog_title"),
                                                          fileName: ref fileName,
                                                          filter: filter) == true)
            {
                this.OutputFilePath = fileName;
            }
        }

        private void BrowseXslFile()
        {
            var fileName = this.XslFilePath;

            var filter = string.Format("{0}(*.xsl)|*.xsl|{1}(*.*)|*.*", 
                                       this.L("xml_tools", "xsl_file_type_filter"),
                                       this.L("common", "all_file_types_filter"));

            if (DialogManager.Instance.ShowOpenFileDialog(title: this.L("xml_tools", "open_xsl_file_dialog_title"),
                                                          fileName: ref fileName,
                                                          filter: filter) == true)
            {
                this.XslFilePath = fileName;
            }
        }
    }
}
