using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.UserInterface.ViewModels;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Smellyriver.TankInspector.Pro.UserInterface
{
    /// <summary>
    /// Interaction logic for MainDockingView.xaml
    /// </summary>
    public partial class MainDockingView : UserControl
    {

        private static readonly string s_layoutConfigFile = ApplicationPath.GetConfigPath("layout.config");

        internal MainDockingViewVM ViewModel
        {
            get { return this.DataContext as MainDockingViewVM; }
            set { this.DataContext = value; }
        }

        public MainDockingView()
        {
            this.AddPropertyChangedHandler(DataContextProperty, OnDataContextChanged);

            using (Diagnostics.PotentialExceptionRegion)
            {
                InitializeComponent();
            }
            this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (this.ViewModel != null)
                this.ViewModel.DocumentsRestored += ViewModel_DocumentsRestored;
        }


        void ViewModel_DocumentsRestored(object sender, EventArgs e)
        {
            var serializer = new XmlLayoutSerializer(this.DockingManager);

            serializer.LayoutSerializationCallback += LayoutSerializationCallback;


            if (File.Exists(s_layoutConfigFile))
                serializer.Deserialize(s_layoutConfigFile);

            // remove empty panes (i.e. a module is removed)
            var emptyLayoutContents = this.DockingManager
                                          .Layout.Descendents()
                                          .OfType<LayoutContent>()
                                          .Where(c => c.Content == null)
                                          .ToArray();

            foreach (var emptyLayoutContent in emptyLayoutContents)
                emptyLayoutContent.Parent.RemoveChild(emptyLayoutContent);

            this.ViewModel.ActiveDocument = this.DockingManager.ActiveContent as DocumentVM;
        }


        void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            this.ViewModel.CloseTemporaryDocuments();
            this.ViewModel.SaveDocumentRestorationInfo();

            var serializer = new XmlLayoutSerializer(this.DockingManager);
            serializer.Serialize(s_layoutConfigFile);
        }

        void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            // do not delete me
        }

        private void DockingManager_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            var document = (DocumentVM)e.Document.Content;
            if (document != null)
                this.ViewModel.CloseDocument(document);

            if (this.ViewModel.Documents.Count == 0)
                this.ViewModel.ActiveDocument = null;

            e.Cancel = true;
        }

        private void DockingManager_ActiveContentChanged(object sender, EventArgs e)
        {
            var activeDocument = this.DockingManager.ActiveContent as DocumentVM;
            if (activeDocument != null || this.DockingManager.ActiveContent == null)
            {
                this.ViewModel.ActiveDocument = activeDocument;
            }
        }



    }
}
