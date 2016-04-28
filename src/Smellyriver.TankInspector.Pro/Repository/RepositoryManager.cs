using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Smellyriver.TankInspector.Pro.InternalModules;
using Smellyriver.TankInspector.Pro.InternalModules.RepositoryManager;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public partial class RepositoryManager
    {
        public static RepositoryManager Instance { get; private set; }

        static RepositoryManager()
        {
            RepositoryManager.Instance = new RepositoryManager();
        }

        private static readonly string s_repositoriesConfigFile = ApplicationPath.GetConfigPath("repositories.config");
        internal const string RepositoryConfigFile = "repository.config";

        private readonly ObservableCollection<IRepository> _repositories;
        private readonly ReadOnlyObservableCollection<IRepository> _readonlyRepositories;

        public ReadOnlyObservableCollection<IRepository> Repositories
        {
            get { return _readonlyRepositories; }
        }

        private readonly Dictionary<IRepository, RepositoryConfiguration> _repositoryConfigurations;

        private readonly MarkerManagerImpl _markerManager;
        internal MarkerManagerImpl MarkerManager { get { return _markerManager; } }

        private RepositoryManager()
        {
            _repositories = new ObservableCollection<IRepository>();
            _readonlyRepositories = new ReadOnlyObservableCollection<IRepository>(_repositories);
            _repositoryConfigurations = new Dictionary<IRepository, RepositoryConfiguration>();
            _markerManager = new MarkerManagerImpl();

            Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            this.SaveRepositoryConfigurations();
        }

        public Task<DocumentInfo> OpenRepositoryManagerDocument()
        {
            return DockingViewManager.Instance.DocumentManager.OpenDocument(InternalDocumentService.RepositoryManagerUri);
        }

        public Task<DocumentInfo> OpenRepositoryPropertiesDocument(IRepository repository)
        {
            return this.OpenRepositoryManagerDocument()
                .ContinueWith(t =>
                    {
                        App.BeginInvokeBackground(() =>
                            {
                                var vm = ((RepositoryManagerDocumentView)t.Result.Content).ViewModel;
                                vm.SelectRepository(repository);
                            });

                        return t.Result;
                    });
        }

        public bool AddRepository(string path)
        {
            // todo: non-local game client support
            if (!LocalGameClientPath.IsPathValid(path))
            {
                this.LogWarning("invalid local game client path '{0}'", path);
                return false;
            }

            if (this.Repositories.Any(r => PathEx.Equals(path, r.Path)))
            {
                this.LogWarning("a local game client with the path '{0}' is already existed", path);
                return false;
            }

            DialogManager.Instance.ShowProgressAsync(this.L("game_client_manager", "adding_repository_dialog_title"),
                                                     new AddRepositoryTask(this, path));

            return true;
        }

        public RepositoryConfiguration GetConfiguration(IRepository repository)
        {
            return _repositoryConfigurations[repository];
        }

        private void SaveRepositoryConfigurations()
        {
            File.WriteAllLines(s_repositoriesConfigFile, _repositories.Select(r => r.Path).ToArray());
            foreach (var repository in _repositories)
            {
                var config = _repositoryConfigurations[repository];
                var repositoryConfigFile = ApplicationPath.GetRepositoryConfigFile(repository, RepositoryConfigFile);
                RepositoryConfiguration.Save(config, repositoryConfigFile);
            }
        }

        public LocalGameClient FindOwner(string path)
        {
            return this.Repositories.OfType<LocalGameClient>()
                                    .FirstOrDefault(c => path.ToLower().StartsWith(c.RootPath.ToLower()));
        }

        public IRepository this[string id]
        {
            get { return this.Repositories.FirstOrDefault(r => r.ID == id); }
        }

        internal void RemoveRepository(IRepository repository)
        {
            _repositories.Remove(repository);
            _repositoryConfigurations.Remove(repository);

            var repositoryStorageDirectory = ApplicationPath.GetRepositoryDirectory(repository);
            try
            {
                Directory.Delete(repositoryStorageDirectory, true);
            }
            catch (Exception ex)
            {
                this.LogError("failed to remove repository directory '{0}': {1}", repositoryStorageDirectory, ex.Message);
            }
        }

    }
}
