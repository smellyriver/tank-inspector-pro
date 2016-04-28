using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.InternalModules.RepositoryManager
{
    class PathVM : NotificationObject
    {
        private bool _isModDirectory;
        public bool IsModDirectory
        {
            get { return _isModDirectory; }
            set
            {
                _isModDirectory = value;
                this.RaisePropertyChanged(() => this.IsModDirectory);
            }
        }

        public string DisplayPath { get; private set; }

        public string Path { get; }

        public PathVM(string path, LocalGameClient _client)
        {
            this.Path = path;
            this.IsModDirectory = PathEx.Equals(_client.ModDirectory, path);
            this.DisplayPath = PathEx.NormalizeDirectorySeparators(PathEx.Relativize(this.Path, _client.RootPath));
        }
    }
}
