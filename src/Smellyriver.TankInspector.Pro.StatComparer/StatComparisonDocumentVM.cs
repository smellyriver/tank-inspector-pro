using System.Linq;
using System.Windows.Input;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM : NotificationObject
    {
        public const string RowHeaderKey = "#header";

        

        private DocumentInfo _documentInfo;
        public DocumentInfo DocumentInfo
        {
            get { return _documentInfo; }
            set
            {
                _documentInfo = value;
                _documentInfo.Title = this.Title;
            }
        }

        public IDropTarget DropHandler { get; private set; }

        private bool _isInitializing;

        public StatComparisonDocumentVM(StatComparisonDocumentService service, CommandBindingCollection commandBindings, string persistentInfo)
        {
            _isInitializing = true;

            _commandBindings = commandBindings;

            this.PersistentInfo = DocumentPersistentInfoProviderBase.Load(persistentInfo, 
                                                                            () => new StatComparisonDocumentPersistentInfo(), 
                                                                            this.GetLogger());
            this.StatsManager = new StatsManagerVM(this);
            this.TanksManager = new TanksManagerVM(this);

            this.InitializeGrid();

            if (this.Title == null)
            {
                service.PromptForTitle()
                       .ContinueWith(t => this.Title = string.IsNullOrWhiteSpace(t.Result) 
                                                     ? this.L("stat_comparison", "new_comparison_default_title")
                                                     : t.Result);
            }

            this.UpdateStatsValueMode();

            this.DropHandler = new DropHandlerImpl(this);

            this.InitializeCommands();

            _isInitializing = false;
        }

        private void UpdateStatsValueMode()
        {
            foreach (var stat in _statVms.Values.SelectMany(s => s.Values))
                stat.ValueMode = this.StatValueMode.Mode;
        }

    }
}
