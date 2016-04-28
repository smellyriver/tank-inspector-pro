using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace Smellyriver.TankInspector.Pro.XmlTools
{
    /// <summary>
    /// Interaction logic for XPathView.xaml
    /// </summary>
    public partial class XPathView : UserControl
    {
        internal XPathVM ViewModel
        {
            get { return this.DataContext as XPathVM; }
            set { this.DataContext = value; }
        }

        public XPathView()
        {
            InitializeComponent();
            this.DataContextChanged += XmlViewer_DataContextChanged;
        }

        void XmlViewer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVm = e.OldValue as XPathVM;
            if (oldVm != null)
            {
                oldVm.XPathQueryResultChanged -= ViewModel_XPathQueryResultChanged;
                oldVm.ColumnHeaderFilterChanged -= ViewModel_ColumnHeaderFilterChanged;
            }

            var newVm = e.NewValue as XPathVM;
            if (newVm != null)
            {
                this.UpdateQueryResult();
                newVm.XPathQueryResultChanged += ViewModel_XPathQueryResultChanged;
                newVm.ColumnHeaderFilterChanged += ViewModel_ColumnHeaderFilterChanged;
            }
        }

        void ViewModel_ColumnHeaderFilterChanged(object sender, EventArgs e)
        {
            this.UpdateColumnHeaders();
        }

        void ViewModel_XPathQueryResultChanged(object sender, EventArgs e)
        {
            this.UpdateQueryResult();
            this.UpdateColumnHeaders();
        }


        private void UpdateQueryResultGridDefault()
        {
            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Name";
            nameColumn.Binding = new Binding("Name");
            this.QueryResultGrid.Columns.Add(nameColumn);

            DataGridTextColumn textColumn = null;
            DataGridTextColumn contentColumn = null;
            Dictionary<string, DataGridTextColumn> columns = new Dictionary<string, DataGridTextColumn>();

            foreach (var item in this.ViewModel.XPathQueryResultItems)
            {
                var element = item as XElement;
                if (element != null)
                {

                    foreach (var attribute in element.Attributes())
                    {
                        var name = attribute.Name.ToString();
                        if (!columns.ContainsKey(name))
                        {
                            var column = new DataGridTextColumn();
                            column.Header = name;
                            column.Binding = new Binding(string.Format("Attribute[{0}].Value", name));
                            columns.Add(name, column);
                        }
                    }

                    foreach (var child in element.Nodes())
                    {
                        var childElement = child as XElement;
                        if (childElement != null)
                        {
                            var name = childElement.Name.ToString();
                            if (!columns.ContainsKey(name))
                            {
                                var column = new DataGridTextColumn();
                                column.Header = name;
                                column.Binding = new Binding(string.Format("Element[{0}].Value", name));
                                columns.Add(name, column);
                            }
                        }
                        else
                        {
                            var text = child as XText;
                            if (text != null && contentColumn == null)
                            {
                                contentColumn = new DataGridTextColumn();
                                contentColumn.Binding = new Binding("FirstNode.Value"); //todo
                                contentColumn.Header = "Content";
                            }
                        }
                    }

                }
                else
                {
                    var text = item as XText;
                    if (text != null && textColumn == null)
                    {
                        textColumn = new DataGridTextColumn();
                        textColumn.Binding = new Binding();
                        textColumn.Header = "Text";
                    }
                }

            }

            if (textColumn != null)
                this.QueryResultGrid.Columns.Add(textColumn);

            if (contentColumn != null)
                this.QueryResultGrid.Columns.Add(contentColumn);

            foreach (var column in columns.Values)
                this.QueryResultGrid.Columns.Add(column);

            this.QueryResultGrid.ItemsSource = this.ViewModel.XPathQueryResultItems;
        }

        private void UpdateQueryResultGridCustom(string columnDefinition)
        {
            var definitions = columnDefinition.Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var xpaths = new string[definitions.Length];
            foreach (var definition in definitions)
            {
                var column = new DataGridTextColumn();
                column.Header = definition;
                column.Binding = new Binding() { Converter = new XPathConverter(definition) };
                this.QueryResultGrid.Columns.Add(column);
            }

            this.QueryResultGrid.ItemsSource = this.ViewModel.XPathQueryResultItems;
        }

        private void UpdateColumnHeaders()
        {
            this.QueryResultGrid.Columns.Clear();

            if (this.ViewModel.XPathQueryResultItems == null)
            {
                this.QueryResultGrid.ItemsSource = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(this.ViewModel.ColumnHeadersText))
                this.UpdateQueryResultGridDefault();
            else
                this.UpdateQueryResultGridCustom(this.ViewModel.ColumnHeadersText);
        }

        private void UpdateQueryResult()
        {
            this.QueryResultText.Text = this.ViewModel.XPathQueryResultString;
        }
    }
}
