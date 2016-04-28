using System.ComponentModel;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    class RepositoryComboBoxItemVM : ComboBoxItemVM<IRepository>
    {
        public RepositoryComboBoxItemVM(IRepository repository)
            : base(repository, null, null)
        {
            var configuration = RepositoryManager.Instance.GetConfiguration(repository);

            configuration.PropertyChanged += configuration_PropertyChanged;

            this.Name = configuration.Alias;
            this.Icon = repository.GetMarker();
            this.Description = repository.Version.ToString();
        }

        void configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Alias":
                    this.Name = ((RepositoryConfiguration)sender).Alias;
                    break;

                case "MarkerColor":
                    this.Icon = this.Model.GetMarker();
                    break;
            }
        }
    }
}
