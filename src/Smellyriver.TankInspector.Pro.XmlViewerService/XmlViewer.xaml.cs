using System;
using System.Windows;
using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.XmlViewerService
{
    public partial class XmlViewer : UserControl
    {

        internal XmlViewerVM ViewModel
        {
            get { return this.DataContext as XmlViewerVM; }
            set { this.DataContext = value; }
        }

        public XmlViewer()
        {
            InitializeComponent();
            this.DataContextChanged += XmlViewer_DataContextChanged;

        }

        void XmlViewer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVm = e.OldValue as XmlViewerVM;
            if(oldVm!=null)
                oldVm.ContentChanged -= ViewModel_ContentChanged;

            var newVm = e.NewValue as XmlViewerVM;
            if (newVm != null)
            {
                this.TextEditor.Text = newVm.Content;
                newVm.ContentChanged += ViewModel_ContentChanged;
            }
        }

        void ViewModel_ContentChanged(object sender, EventArgs e)
        {
            this.TextEditor.Text = this.ViewModel.Content;
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            this.ViewModel.UpdateContent(this.TextEditor.Text);
        }

        
    }
}
