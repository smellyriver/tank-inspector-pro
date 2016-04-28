using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml.Linq;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using StatBehaviors = Smellyriver.TankInspector.Pro.StatsInspector.Document.Stat;

namespace Smellyriver.TankInspector.Pro.StatsInspector
{
    class StatTemplate
    {
        public static StatTemplate Load(string filename)
        {
            var template = new StatTemplate(filename);
            if (!template._isValid)
                return null;

            return template;
        }

        public string Filename { get; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }

        private bool _isValid;

        private string _resourceElementString;

        private StatTemplate(string filename)
        {
            this.Filename = filename;
            this.LoadFileInfo();
        }

        private void LoadFileInfo()
        {
            _isValid = true;

            XElement fileElement;
            try
            {
                fileElement = XElement.Load(this.Filename);
            }
            catch (Exception ex)
            {
                _isValid = false;
                this.LogError("error loading template '{0}': {1}", this.Filename, ex.Message);
                return;
            }

            if (fileElement == null)
            {
                _isValid = false;
                return;
            }

            var nameAttribute = fileElement.Attribute("Template.Name");
            this.Name = (nameAttribute == null || string.IsNullOrWhiteSpace(nameAttribute.Value))
                      ? this.L("stats_inspector", "untitled_template_name")
                      : this.L(nameAttribute.Value);

            var descriptionAttribute = fileElement.Attribute("Template.Description");
            if (descriptionAttribute != null)
                this.Description = this.L(descriptionAttribute.Value);

            var authorAttribute = fileElement.Attribute("Template.Author");
            this.Author = authorAttribute == null
                        ? this.L("stats_inspector", "anonymous_template_author")
                        : this.L(authorAttribute.Value);

            var rootName = fileElement.Name;
            var resourcesElement = fileElement.Element(fileElement.Name + ".Resources");
            if (resourcesElement == null)
                return;

            resourcesElement.Name = XName.Get("ResourceDictionary", resourcesElement.Name.NamespaceName);

            var nsAttributes = fileElement.Attributes().Where(a => a.Name.NamespaceName == "http://www.w3.org/2000/xmlns/");
            foreach (var ns in nsAttributes)
                resourcesElement.Add(new XAttribute(ns));

            _resourceElementString = resourcesElement.ToString();
        }

        private FlowDocument LoadTemplate()
        {
            try
            {
                return (FlowDocument)XamlReader.Load(File.OpenRead(this.Filename));
            }
            catch (Exception ex)
            {
                this.LogError("error loading template '{0}': {1}", this.Filename, ex.Message);
                return null;
            }
        }

        public void BeginCreateDocument(TankInstance tank, IProgressScope progress, Action<CreateDocumentResult> callback)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    progress.ReportStatusMessage(this.L("stat_inspector", "status_analysing"));
                    progress.ReportIsIndetermine();

                    this.LogInfo("creating document from template file '{0}'", this.Filename);
                    var document = this.LoadTemplate();
                    progress.ReportProgress(0);

                    var decendants = LogicalTreeHelperEx.GetDecendants<TextElement>(document).ToArray();
                    var statVms = new Dictionary<IStat, StatVM>();

                    var completedDecendant = 0;

                    foreach (var decendant in decendants)
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                            {
                                var key = StatBehaviors.GetKey(decendant);
                                if (key != null)
                                {
                                    var stat = StatsProviderManager.Instance.GetStat(key);
                                    if (stat == null)
                                    {
                                        this.LogWarning("unknown stat: key='{0}'", key);
                                        return;
                                    }

                                    var statVm = statVms.GetOrCreate(stat, () => new StatVM(stat, tank));

                                    decendant.DataContext = statVm;

                                    if (decendant is IAddChild)
                                        this.ApplyTemplate(decendant);

                                    ++completedDecendant;
                                    progress.ReportProgress((double)completedDecendant / decendants.Length);
                                }
                            }), DispatcherPriority.Background);
                    }

                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                        {
                            progress.ReportIsIndetermine();
                            callback(new CreateDocumentResult(document, statVms.Values));
                            progress.ReportProgress(1.0);
                        }), DispatcherPriority.Background);
                }), DispatcherPriority.Background);

        }

        private IEnumerable GetStatTemplate(string statTemplateKey)
        {
            var resources = (ResourceDictionary)XamlReader.Parse(_resourceElementString);
            if (resources.Contains(statTemplateKey))
            {
                var template = (IEnumerable)resources[statTemplateKey];
                return template;
            }
            else
            {
                this.LogError("invalid stat template");
                return new object[0];
            }
        }

        private void ApplyTemplate<TDecendant>(TDecendant decendant)
            where TDecendant : TextElement, IAddChild
        {
            var statTemplateKey = StatBehaviors.GetTemplateKey(decendant);

            if (statTemplateKey == null)
                return;

            var statTemplate = this.GetStatTemplate(statTemplateKey);

            if (statTemplate == null)
                return;

            StatBehaviors.ApplyTemplate(decendant, statTemplate);
        }
    }
}
