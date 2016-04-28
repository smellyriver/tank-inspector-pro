using System.Windows.Media;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    static class ComboBoxItemVM
    {
        public static ComboBoxItemVM<TModel> Create<TModel>(TModel model, string name, ImageSource icon, string description = null)
        {
            return new ComboBoxItemVM<TModel>(model, name, icon, description);
        }
    }

    class ComboBoxItemVM<TModel> : NotificationObject
    {
        private ImageSource _icon;
        public ImageSource Icon
        {
            get { return _icon; }
            protected set
            {
                _icon = value;
                this.RaisePropertyChanged(() => this.Icon);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            protected set
            {
                _name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            protected set
            {
                _description = value;
                this.RaisePropertyChanged(() => this.Description);
            }
        }

        public TModel Model { get; private set; }

        public ComboBoxItemVM(TModel model, string name, ImageSource icon, string description = null)
        {
            this.Model = model;
            this.Name = name;
            this.Icon = icon;
            this.Description = description;
        }
    }
}
