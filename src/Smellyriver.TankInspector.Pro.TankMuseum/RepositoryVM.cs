using System.ComponentModel;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    class RepositoryVM : NotificationObject
    {
        public IRepository Model { get; }
        public ImageSource Marker
        {
            get { return this.Model.GetMarker(); }
        }
        public string Name
        {
            get { return this.Configuration.Alias; }
        }

        public GameVersion Version
        {
            get { return this.Model.Version; }
        }

        public RepositoryConfiguration Configuration { get; }

        public RepositoryVM(IRepository repository)
        {
            this.Model = repository;
            this.Configuration = RepositoryManager.Instance.GetConfiguration(repository);
            this.Configuration.PropertyChanged += Configuration_PropertyChanged;
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Alias":
                    this.RaisePropertyChanged(() => this.Name);
                    break;

                case "MarkerColor":
                    this.RaisePropertyChanged(() => this.Marker);
                    break;
            }
        }
    }
}
