using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.IO.XmlDecoding;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;

namespace Smellyriver.TankInspector.Pro.XmlViewerService
{
    class XmlViewerVM : NotificationObject, IXmlViewer
    {

        public event EventHandler ContentChanged;



        string IXmlViewer.XmlText
        {
            get { return this.Content; }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            private set
            {
                _content = value;
                this.RaisePropertyChanged(() => this.Content);
                this.OnContentChanged();
            }
        }

        private string _savedContent;
        private string SavedContent
        {
            get { return _savedContent; }
            set
            {
                _savedContent = value;
                this.RaisePropertyChanged(() => this.IsContentChanged);
            }
        }


        public bool IsContentChanged
        {
            get { return this.Content == _savedContent; }
        }

        public bool ReadOnly
        {
            get { return _savePath == null; }
        }

        private string _savePath;
        private string SavePath
        {
            get { return _savePath; }
            set
            {
                _savePath = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _defaultSaveAsPath;

        private readonly XmlViewerService _service;

        private EncodeType _encodeType;

        private readonly Dispatcher _dispatcher;

        public XmlViewerVM(XmlViewerService service, CommandBindingCollection commandBindings, Stream stream)
            : this(service, commandBindings, new BigworldXmlReader(stream))
        {
        }

        public XmlViewerVM(XmlViewerService service, CommandBindingCollection commandBindings, string path)
            : this(service, commandBindings, new BigworldXmlReader(path))
        {
            if (!UnifiedPath.IsInPackage(path))
            {
                this.SavePath = path;
                _defaultSaveAsPath = path;
            }
            else
            {
                var gameClient = RepositoryManager.Instance.FindOwner(path);
                if (gameClient != null)
                    _defaultSaveAsPath = gameClient.GetCorrespondedModPath(path);
            }
        }


        private XmlViewerVM(XmlViewerService service, CommandBindingCollection commandBindings, BigworldXmlReader reader)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            _service = service;
            _encodeType = reader.EncodeType;

            var document = XDocument.Load(reader);
            document.TrimText();

            this.Content = document.ToString();
            this.SavedContent = this.Content;

            commandBindings.Add(new CommandBinding(ApplicationCommands.Save, this.ExecuteSave, this.CanExecuteSave));
            commandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, this.ExecuteSaveAs));
        }

        private void ExecuteSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveAs();
        }

        private void CanExecuteSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !this.ReadOnly;
        }

        private void ExecuteSave(object sender, ExecutedRoutedEventArgs e)
        {
            if (_encodeType != EncodeType.Plain)
            {
                var settings = new DialogSettings()
                {
                    AffirmativeButtonText = this.L("xml_viewer_service", "save_plain_file_button"),
                    NegativeButtonText = this.L("common", "save_as_with_ellipsis"),
                    FirstAuxiliaryButtonText = this.L("common", "cancel"),
                };

                DialogManager.Instance.ShowMessageAsync(this.L("xml_viewer_service", "overwrite_encoded_file_prompt_title"),
                                                        this.L("xml_viewer_service", "overwrite_encoded_file_prompt_message"),
                                                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                                                        settings)
                                                        .ContinueWith(t =>
                                                        {
                                                            if (t.Result == MessageDialogResult.Affirmative)
                                                                _dispatcher.AutoInvoke(this.Save);
                                                            else if(t.Result == MessageDialogResult.Negative)
                                                                _dispatcher.AutoInvoke(this.SaveAs);
                                                        });
            }
            else
                this.Save();
        }

        private void SaveAs()
        {
            string fileName = _defaultSaveAsPath;
            var result = DialogManager.Instance.ShowSaveFileDialog(title: this.L("common", "save_as"),
                                                                   fileName: ref fileName,
                                                                   filter: _service.GetFilter(),
                                                                   filterIndex: 0,
                                                                   defaultExtensionName: ".xml",
                                                                   overwritePrompt: true,
                                                                   addExtension: true,
                                                                   checkPathExists: true);
        if (result == true)
            {
                this.SavePath = fileName;
                _defaultSaveAsPath = fileName;
                this.Save();
            }
        }

        private void Save()
        {
            File.WriteAllText(this.SavePath, this.Content);
            this.SavedContent = this.Content;
        }

        public void UpdateContent(string content)
        {
            _content = content;
        }

        private void OnContentChanged()
        {
            if (this.ContentChanged != null)
                this.ContentChanged(this, EventArgs.Empty);

            this.RaisePropertyChanged(() => this.IsContentChanged);
        }

    }
}
