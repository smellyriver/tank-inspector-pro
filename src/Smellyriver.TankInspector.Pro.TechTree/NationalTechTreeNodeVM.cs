using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using TankEntity = Smellyriver.TankInspector.Pro.Data.Entities.Tank;

namespace Smellyriver.TankInspector.Pro.TechTree
{

    class NationalTechTreeNodeVM : NotificationObject
    {
        public int Column { get; private set; }
        public int Row { get; private set; }
        public TankEntity Tank { get; }
        public IEnumerable<string> UnlockTanks { get; private set; }

        public string ShortName { get { return this.Tank.ShortName; } }
        public string Name { get { return this.Tank.Name; } }
        public bool IsPremiumTank { get { return this.Tank.IsPremium; } }
        public bool IsObsoleted { get { return this.Tank.IsObsoleted; } }

        private ImageSource _icon;
        public ImageSource Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                this.RaisePropertyChanged(() => this.Icon);
            }
        }


        public MenuItemVM[] MenuItems { get; private set; }

        private readonly IRepository _repository;
        private readonly TankUnikey _unikey;

        public NationalTechTreeNodeVM(IRepository repository, 
                                      IXQueryable tank, 
                                      int row, 
                                      int column, 
                                      IEnumerable<string> unlockTanks, 
                                      ImageSource icon)
        {
            _repository = repository;
            this.Tank = new TankEntity(tank);
            _unikey = new TankUnikey(_repository, this.Tank);
            this.Column = column;
            this.Row = row;
            this.UnlockTanks = unlockTanks;

            App.BeginInvokeBackground(() => this.Icon = icon);

            this.MenuItems = TankCommandManager.Instance
                                                .Commands
                                                .OrderBy(c => c.Priority)
                                                .Select(c => new MenuItemVM(c.Name, c, _unikey)
                                                                {
                                                                    Icon = c.Icon,
                                                                })
                                                .ToArray();
        }

    }
}
