using System.Collections;
using System.Linq;
using System.Windows.Input;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Collections;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Wpf.Input;
using RepositoryManagerService = Smellyriver.TankInspector.Pro.Repository.RepositoryManager;

namespace Smellyriver.TankInspector.Pro.InternalModules.RepositoryManager
{
    class RepositoryManagerDocumentVM : NotificationObject
    {
        
        public RepositoryManagerDocumentPersistentInfo PersistentInfo { get; private set; }

        public ViewModelMap<IRepository, RepositoryVM> Repositories { get; }

        private RepositoryVM _selectedRepository;
        public RepositoryVM SelectedRepository
        {
            get { return _selectedRepository; }
            set
            {
                _selectedRepository = value;
                this.RaisePropertyChanged(() => this.SelectedRepository);
                this.RaisePropertyChanged(() => this.IsEditAreaEnabled);
            }
        }

        public ICommand AddRepositoryCommand { get; private set; }
        public ICommand RemoveRepositoryCommand { get; private set; }

        public bool IsEditAreaEnabled { get { return this.SelectedRepository != null; } }

        public RepositoryManagerDocumentVM(RepositoryManagerDocumentService service, CommandBindingCollection commandBindings, string persistentInfo)
        {
            this.PersistentInfo = DocumentPersistentInfoProviderBase.Load(persistentInfo,
                                                                               () => new RepositoryManagerDocumentPersistentInfo(), this.GetLogger());

            var repositoryManager = RepositoryManagerService.Instance;

            this.Repositories = new ViewModelMap<IRepository, RepositoryVM>(
                repositoryManager.Repositories,
                r => new RepositoryVM(r, repositoryManager.GetConfiguration(r)));

            if (this.Repositories.Count > 0)
                this.SelectedRepository = this.Repositories[0];

            this.AddRepositoryCommand = new RelayCommand(this.AddRepository);
            this.RemoveRepositoryCommand = new RelayCommand<IList>(this.RemoveRepository, this.CanRemoveRepository);
        }

        private bool CanRemoveRepository(IList selectedItems)
        {
            return selectedItems.Count > 0;
        }

        private void RemoveRepository(IList selectedItems)
        {
            var repositoriesToRemove = selectedItems.Cast<RepositoryVM>().ToArray();
            foreach (var repository in repositoriesToRemove)
            {
                RepositoryManagerService.Instance.RemoveRepository(repository.Repository);
            }
        }

        private void AddRepository()
        {
            var path = RepositoryHelper.RegistryGameClientPath;
            if (DialogManager.Instance.ShowFolderBrowserDialog(this.L("game_client_manager", "open_game_client_dialog_message"),
                                                              ref path,
                                                              false) == true)
            {
                if (RepositoryManagerService.Instance.Repositories.Any(r => PathEx.Equals(path, r.Path)))
                {
                    DialogManager.Instance.ShowMessageAsync(this.L("game_client_manager", "game_client_existed_message_title"),
                                                            this.L("game_client_manager", "game_client_existed_message"));

                    return;
                }

                RepositoryManagerService.Instance.AddRepository(path);
            }
        }

        public void SelectRepository(IRepository repository)
        {
            this.SelectedRepository = this.Repositories.FirstOrDefault(r => r.Repository == repository);
        }
    }
}
