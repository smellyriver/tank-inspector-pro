using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface.Design;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    class TankVM : NotificationObject
    {
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public ImageSource NationIcon { get; private set; }
        public string Nation { get; private set; }

        private ImageSource _smallIcon;
        public ImageSource SmallIcon
        {
            get { return _smallIcon; }
            set
            {
                _smallIcon = value;
                this.RaisePropertyChanged(() => this.SmallIcon);
            }
        }

        public ImageSource ClassIcon { get; private set; }
        public string Tier { get; private set; }
        public int ClassSortIndex { get; private set; }

        public RepositoryVM Repository { get; private set; }

        public Tank Model { get; private set; }

        public TankUnikey TankUnikey { get; private set; }

        public TankVM(RepositoryVM repository, Tank tank)
        {
            this.Repository = repository;
            this.Model = tank;

            this.TankUnikey = new TankUnikey(repository.Model, tank);

            this.Name = tank.Name;
            this.ShortName = tank.ShortName;
            this.Tier = RomanNumberService.GetRomanNumber(tank.Tier);

            this.ClassSortIndex = TankClassComparer.GetClassSortIndex(tank.ClassKey);

            this.Nation = tank.NationKey;

            var gameClient = repository.Model as LocalGameClient;
            if (gameClient != null)
            {
                App.BeginInvokeBackground(() => this.SmallIcon = gameClient.PackageImages.GetTankSmallIcon(tank.IconKey));
                this.NationIcon = gameClient.PackageImages.GetNationSmallIcon(tank.NationKey);
                this.ClassIcon = gameClient.PackageImages.GetClassSmallIcon(tank.ClassKey);
            }
        }
    }
}
