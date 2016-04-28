using System.Windows.Input;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.CrewConfigurator
{
    class SkillVM : SkillVMBase
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string ShortDescription { get; private set; }

        public ImageSource Icon { get; private set; }
        public ImageSource SmallIcon { get; private set; }

        public ICommand ForgetSkill { get; private set; }

        private readonly CrewVM _owner;

        public SkillVM(CrewVM owner, IRepository repository, IXQueryable skill)
            : base(skill)
        {
            _owner = owner;

            this.Name = skill["userString"];
            this.Description = skill["description"];
            this.ShortDescription = skill["shortDesc"];

            var localGameClient = repository as LocalGameClient;
            if (localGameClient != null)
            {
                this.Icon = localGameClient.PackageImages.GetSkillIcon(skill["icon"]);
                this.SmallIcon = localGameClient.PackageImages.GetSkillSmallIcon(skill["icon"]);
            }

            this.ForgetSkill = new RelayCommand(() => _owner.ForgetSkill(this));
        }

    }
}
