using System.ComponentModel;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    class DocumentVM : LayoutContentVM
    {
        public DocumentInfo Document { get; }

        public override ImageSource IconSource
        {
            get
            {
                if (base.IconSource != null)
                    return base.IconSource;
                else if (this.Document.RepositoryId == null)
                    return null;
                else
                {
                    var repository = RepositoryManager.Instance[this.Document.RepositoryId];
                    if (repository != null)
                        return repository.GetMarker();
                }

                return null;
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.RaisePropertyChanged(() => this.IsSelected);
            }
        }

        public DocumentVM(DocumentInfo document)
            : base(document)
        {
            this.Document = document;
            var repository = RepositoryManager.Instance[this.Document.RepositoryId];
            if(repository!= null)
            {
                var repositoryConfig = RepositoryManager.Instance.GetConfiguration(repository);
                repositoryConfig.PropertyChanged += repositoryConfig_PropertyChanged;
            }
        }

        void repositoryConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MarkerColor")
                this.RaisePropertyChanged(() => this.IconSource);
        }
    }
}
