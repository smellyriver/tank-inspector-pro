using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface.ViewModels;
using Smellyriver.TankInspector.Common.Utilities;
using StatBehaviors = Smellyriver.TankInspector.Pro.StatsInspector.Document.Stat;

namespace Smellyriver.TankInspector.Pro.StatsInspector
{
    class StatsDocumentVM : FlowDocumentVMBase, ITankConfigurable, ICrewConfigurable, ICustomizationConfigurable
    {


        private static readonly StatValueModeVM[] s_statValueModes = StatValueModeVM.StatValueModes;

        TankConfiguration ITankConfigurable.TankConfiguration
        {
            get { return this.Tank.TankConfiguration; }
        }

        CrewConfiguration ICrewConfigurable.CrewConfiguration
        {
            get { return this.Tank.CrewConfiguration; }
        }

        CustomizationConfiguration ICustomizationConfigurable.CustomizationConfiguration
        {
            get { return this.Tank.CustomizationConfiguration; }
        }

        IRepository ITankConfigurable.Repository
        {
            get { return this.Tank.Repository; }
        }

        IRepository ICrewConfigurable.Repository
        {
            get { return this.Tank.Repository; }
        }

        IRepository ICustomizationConfigurable.Repository
        {
            get { return this.Tank.Repository; }
        }

        event EventHandler ICrewConfigurable.CrewConfigurationChanged { add { } remove { } }
        event EventHandler ITankConfigurable.TankConfigurationChanged { add { } remove { } }
        event EventHandler ICustomizationConfigurable.CustomizationConfigurationChanged { add { } remove { } }

        public IEnumerable<StatTemplate> Templates { get { return TemplateManager.Instance.Templates; } }

        public StatTemplate SelectedTemplate
        {
            get { return TemplateManager.Instance.GetTemplate(this.PersistentInfo.TemplateFilename); }
            set
            {
                if (value != this.SelectedTemplate)
                {
                    this.PersistentInfo.TemplateFilename = value.Filename;
                    this.CreateDocument();
                }

                StatsInspectorSettings.Default.LastTemplateFilename = value.Filename;
                StatsInspectorSettings.Default.Save();
            }
        }


        public IEnumerable<StatValueModeVM> StatValueModes { get { return s_statValueModes; } }

        public StatValueModeVM SelectedStatValueMode
        {
            get { return s_statValueModes[(int)this.PersistentInfo.ValueMode]; }
            set
            {
                if (this.PersistentInfo.ValueMode != value.Mode)
                {
                    this.PersistentInfo.ValueMode = value.Mode;
                    this.RaisePropertyChanged(() => this.SelectedStatValueMode);
                    StatsInspectorSettings.Default.StatValueMode = value.Mode;
                    StatsInspectorSettings.Default.Save();

                    this.UpdateStatsValueMode();
                }
            }
        }

        private StatsDocumentService StatsDocumentService { get; set; }

        public TankInstance Tank { get; }

        private FlowDocument _document;
        public override FlowDocument Document
        {
            get { return _document; }
            protected set
            {
                _document = value;
                this.RaisePropertyChanged(() => this.Document);
            }
        }

        private bool _isGenerating;
        public bool IsGenerating
        {
            get { return _isGenerating; }
            set
            {
                _isGenerating = value;
                this.RaisePropertyChanged(() => this.IsGenerating);
            }
        }

        private IProgressScope _generatingProgressScope;
        public IProgressScope GeneratingProgressScope
        {
            get { return _generatingProgressScope; }
            set
            {
                _generatingProgressScope = value;
                this.RaisePropertyChanged(() => this.GeneratingProgressScope);
            }
        }


        protected override string ExportFileName
        {
            get
            {
                return this.PersistentInfo.SaveAsPath
                        ?? this.L("stats_inspector", "export_default_filename", this.Tank["@fullKey"], this.Tank.Repository.Version)
                                 .Replace(Path.GetInvalidFileNameChars(), '-');
            }
            set
            {
                this.PersistentInfo.SaveAsPath = value;
            }
        }

        public FlowDocumentReaderViewingMode ViewingMode
        {
            get { return this.PersistentInfo.ViewingMode; }
            set
            {
                this.PersistentInfo.ViewingMode = value;
                this.RaisePropertyChanged(() => this.ViewingMode);

                StatsInspectorSettings.Default.StatDocumentViewingMode = value;
                StatsInspectorSettings.Default.Save();
            }
        }

        private IEnumerable<StatVM> _statVms;

        public StatsDocumentPersistentInfo PersistentInfo { get; }

        public StatsDocumentVM(StatsDocumentService service, CommandBindingCollection commandBindings, TankInstance tank, string persistentInfo)
            : base(commandBindings)
        {
            this.StatsDocumentService = service;
            this.Tank = tank;

            this.PersistentInfo = DocumentPersistentInfoProviderBase.Load(persistentInfo,
                                                                   () => new StatsDocumentPersistentInfo(tank),
                                                                   this.GetLogger());
            this.CreateDocument();
        }

        private void UpdateStatsValueMode()
        {

            foreach (var stat in _statVms)
                stat.ValueMode = this.SelectedStatValueMode.Mode;
        }

        private void CreateDocument()
        {
            var template = this.SelectedTemplate;
            if (template == null)
                return;

            this.IsGenerating = true;
            this.GeneratingProgressScope = ProgressScope.Create("create stats document");

            template.BeginCreateDocument(this.Tank,
                this.GeneratingProgressScope,
                r =>
                {
                    this.Document = r.Document;
                    _statVms = r.StatVms;
                    this.UpdateStatsValueMode();

                    this.IsGenerating = false;
                });

            //this.Document = template.BeginCreateDocument(this.Tank, out _statVms);
            //this.UpdateStatsValueMode();
        }

    }
}
