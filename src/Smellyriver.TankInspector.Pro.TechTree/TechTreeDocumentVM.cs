using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.TechTree
{
    class TechTreeDocumentVM : NotificationObject
    {
        

        private readonly TechTreeDocumentService _service;
        private readonly CommandBindingCollection _commandBindings;

        public TechTreeDocumentPersistentInfo PersistentInfo { get; }

        private Dictionary<string, LocalGameClientNationalTechTreeVM> _nationalTechTrees;
        public IEnumerable<LocalGameClientNationalTechTreeVM> NationalTechTrees
        {
            get { return _nationalTechTrees.Values; }
        }

        private LocalGameClientNationalTechTreeVM _selectedNationalTechTree;
        public LocalGameClientNationalTechTreeVM SelectedNationalTechTree
        {
            get { return _selectedNationalTechTree; }
            set
            {
                _selectedNationalTechTree = value;
                if (_selectedNationalTechTree != null && !_selectedNationalTechTree.IsLoaded)
                    _selectedNationalTechTree.BeginLoad();

                this.PersistentInfo.NationKey = _selectedNationalTechTree.NationKey;

                this.RaisePropertyChanged(() => this.SelectedNationalTechTree);
            }
        }


        public TechTreeDocumentVM(TechTreeDocumentService service,
                                  CommandBindingCollection commandBindings,
                                  IRepository repository,
                                  string persistentInfo)
        {
            _service = service;
            _commandBindings = commandBindings;
            this.PersistentInfo = DocumentPersistentInfoProviderBase.Load(persistentInfo,
                                                                      () => new TechTreeDocumentPersistentInfo(repository.Nations.First()),
                                                                      this.GetLogger());

            if (!repository.Nations.Contains(this.PersistentInfo.NationKey))
            {
                this.LogError("unknown nation key: {0}", this.PersistentInfo.NationKey);
                this.PersistentInfo.NationKey = repository.Nations.First();
            }

            // todo: handle non-LocalGameClient repositories
            _nationalTechTrees = repository.Nations.ToDictionary(n => n, n => new LocalGameClientNationalTechTreeVM((LocalGameClient)repository, n));

            this.SelectNation(this.PersistentInfo.NationKey);
        }

        internal void SelectNation(string nationKey)
        {
            this.SelectedNationalTechTree = _nationalTechTrees[nationKey];
        }
    }
}
