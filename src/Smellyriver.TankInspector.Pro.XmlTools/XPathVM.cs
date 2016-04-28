using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.XPath;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.XmlTools
{
    class XPathVM : NotificationObject
    {
        private string _xPathQueryResultString;
        public string XPathQueryResultString
        {
            get { return _xPathQueryResultString; }
            private set
            {
                _xPathQueryResultString = value;
                this.RaisePropertyChanged(() => this.XPathQueryResultString);
            }
        }

        private object[] _xPathQueryResultItems;
        public object[] XPathQueryResultItems
        {
            get { return _xPathQueryResultItems; }
            set
            {
                _xPathQueryResultItems = value;
                this.RaisePropertyChanged(() => this.XPathQueryResultItems);
            }
        }



        private string _xPathQueryText;
        public string XPathQueryText
        {
            get { return _xPathQueryText; }
            set
            {
                _xPathQueryText = value;
                this.RaisePropertyChanged(() => this.XPathQueryText);
            }
        }

        private string _columnHeadersText;
        public string ColumnHeadersText
        {
            get { return _columnHeadersText; }
            set
            {
                _columnHeadersText = value;
                this.RaisePropertyChanged(() => this.ColumnHeadersText);
            }
        }


        public event EventHandler XPathQueryResultChanged;
        public event EventHandler ColumnHeaderFilterChanged;

        public ICommand ExecuteXPathQueryCommand { get; private set; }

        public ICommand UpdateColumnHeadersCommand { get; private set; }

        public XmlToolsVM Owner { get; }

        public XPathVM(XmlToolsVM owner)
        {
            this.Owner = owner;
            this.ExecuteXPathQueryCommand = new RelayCommand(this.ExecuteXPathQuery);
            this.UpdateColumnHeadersCommand = new RelayCommand(this.UpdateColumnHeaders);
        }

        private void UpdateColumnHeaders()
        {
            if (this.ColumnHeaderFilterChanged != null)
                this.ColumnHeaderFilterChanged(this, EventArgs.Empty);
        }

        private void ExecuteXPathQuery()
        {
            if (this.Owner.XmlViewer == null)
                return;

            var element = XElement.Load(this.Owner.XmlViewer.XmlText.ToStream());
            try
            {
                var queryResult = element.XPathEvaluate(this.XPathQueryText);
                //var queryResult = element.XPathSelectElements(this.XPathQueryText);

                if (queryResult is string || queryResult is bool || queryResult is double)
                {
                    this.XPathQueryResultString = queryResult.ToString();
                    this.XPathQueryResultItems = new[] { queryResult };
                    return;
                }

                var enumerableResult = (queryResult as IEnumerable).Cast<object>();

                if (queryResult == null || (enumerableResult != null && !enumerableResult.Any()))
                {
                    this.XPathQueryResultString = string.Format("<!-- {0} -->", this.L("techtree", "xpath_empty_query"));
                    this.XPathQueryResultItems = new object[0];
                }
                else
                {
                    var builder = new StringBuilder();

                    foreach (var resultElement in enumerableResult)
                    {
                        builder.AppendLine(resultElement.ToString());
                    }

                    this.XPathQueryResultString = builder.ToString();

                    this.XPathQueryResultItems = enumerableResult.ToArray();
                }

            }
            catch (XPathException ex)
            {
                this.XPathQueryResultString = string.Format("<!-- \n  {0}\n-->", this.L("techtree", "xpath_error", ex.Message));
            }
            finally
            {

                if (this.XPathQueryResultChanged != null)
                    this.XPathQueryResultChanged(this, EventArgs.Empty);
            }
        }


    }
}
