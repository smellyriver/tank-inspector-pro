using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Collections;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.InternalModules.RepositoryManager
{
    class RepositoryVM : NotificationObject
    {
        public IRepository Repository { get; }
        public RepositoryConfiguration Configuration { get; }

        public string Version { get { return this.Repository.Version.ToString(); } }
        public string Alias
        {
            get { return this.Configuration.Alias; }
            set
            {
                this.Configuration.Alias = value;
                this.RaisePropertyChanged(() => this.Alias);
            }
        }

        public string Path { get { return this.Repository.Path; } }

        public ImageSource Marker { get { return this.Repository.GetMarker(); } }

        public Color MarkerColor
        {
            get { return this.Configuration.MarkerColor; }
            set
            {
                this.Configuration.MarkerColor = value;
                this.RaisePropertyChanged(() => this.MarkerColor);
                this.RaisePropertyChanged(() => this.Marker);
            }
        }

        public IEnumerable<Color> AvailableMarkerColors
        {
            get
            {
                return new[] { this.MarkerColor }
                    .Union(Pro.Repository.RepositoryManager.Instance.MarkerManager.AvailableColors);
            }
        }

        public ViewModelMap<string, PathVM> ClientPaths { get; }

        public ICommand SetAsModDirectoryCommand { get; private set; }
        public ICommand AddClientPathCommand { get; private set; }
        public ICommand RemoveClientPathsCommand { get; private set; }

        private readonly LocalGameClient _client;

        public RepositoryVM(IRepository repository, RepositoryConfiguration config)
        {
            this.Repository = repository;
            this.Configuration = config;

            _client = repository as LocalGameClient;
            if (_client != null)
            {
                this.ClientPaths = new ViewModelMap<string, PathVM>(_client.ClientPaths,
                                                                    p => new PathVM(p, _client));

                this.SetAsModDirectoryCommand = new RelayCommand<PathVM>(this.SetAsModDirectory, this.CanSetAsModDirectory);
                this.AddClientPathCommand = new RelayCommand(this.AddClientPath);
                this.RemoveClientPathsCommand = new RelayCommand<IList>(this.RemoveClientPaths, this.CanRemoveClientPaths);
            }
        }

        private bool CanRemoveClientPaths(IList selectedItems)
        {
            return selectedItems != null && selectedItems.Count > 0;
        }

        private void RemoveClientPaths(IList selectedItems)
        {
            var paths = selectedItems.Cast<PathVM>().Select(v => v.Path).ToArray();
            foreach (var path in paths)
            {
                _client.ClientPaths.Remove(path);
            }
        }

        private void AddClientPath()
        {
            var path = _client.RootPath;
            if (DialogManager.Instance.ShowFolderBrowserDialog(this.L("game_client_manager", "add_game_client_path_dialog_message"),
                                                               ref path,
                                                               true) == true)
            {
                if (_client.ClientPaths.Any(p => PathEx.Equals(path, p)))
                {
                    DialogManager.Instance.ShowMessageAsync(this.L("game_client_manager", "game_client_path_existed_message_title"),
                                                            this.L("game_client_manager", "game_client_path_existed_message"));

                    return;
                }

                _client.ClientPaths.Add(path);
            }
        }

        private void SetAsModDirectory(PathVM selectedItem)
        {
            foreach (var item in this.ClientPaths)
            {
                item.IsModDirectory = item == selectedItem;
            }

            _client.ModDirectory = selectedItem.Path;
        }

        private bool CanSetAsModDirectory(PathVM selectedItem)
        {
            if (selectedItem == null)
                return false;

            if (selectedItem.IsModDirectory)
                return false;

            return true;
        }

    }
}
