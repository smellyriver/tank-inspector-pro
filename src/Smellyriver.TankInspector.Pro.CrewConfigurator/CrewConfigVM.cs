using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.CrewConfigurator
{
    class CrewConfigVM : NotificationObject
    {

        private CrewConfiguration _configuration;
        public CrewConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                _configuration = value;

                this.RaisePropertyChanged(() => this.Configuration);

                if (_configuration != null)
                    this.InitializeCrews();
            }
        }

        public IRepository Repository { get; set; }

        private CrewVM[] _crews;
        public CrewVM[] Crews
        {
            get { return _crews; }
            set
            {
                _crews = value;
                this.RaisePropertyChanged(() => this.Crews);
            }
        }

        private ImageSource _brotherInArmsSkillIcon;
        public ImageSource BrotherInArmsSkillIcon
        {
            get { return _brotherInArmsSkillIcon; }
            private set
            {
                _brotherInArmsSkillIcon = value;
                this.RaisePropertyChanged(() => this.BrotherInArmsSkillIcon);
            }
        }

        private ImageSource _camouflageSkillIcon;
        public ImageSource CamouflageSkillIcon
        {
            get { return _camouflageSkillIcon; }
            private set
            {
                _camouflageSkillIcon = value;
                this.RaisePropertyChanged(() => this.CamouflageSkillIcon);
            }
        }

        private ImageSource _fireFightingSkillIcon;
        public ImageSource FireFightingSkillIcon
        {
            get { return _fireFightingSkillIcon; }
            private set
            {
                _fireFightingSkillIcon = value;
                this.RaisePropertyChanged(() => this.FireFightingSkillIcon);
            }
        }

        private ImageSource _repairSkillIcon;
        public ImageSource RepairSkillIcon
        {
            get { return _repairSkillIcon; }
            private set
            {
                _repairSkillIcon = value;
                this.RaisePropertyChanged(() => this.RepairSkillIcon);
            }
        }

        public ICommand TeachAllBrotherInArmsSkillCommand { get; private set; }
        public ICommand TeachAllCamouflageSkillCommand { get; private set; }
        public ICommand TeachAllFireFightingSkillCommand { get; private set; }
        public ICommand TeachAllRepairSkillCommand { get; private set; }

        public CrewConfigVM()
        {
            this.TeachAllBrotherInArmsSkillCommand = new RelayCommand(this.TeachAllBrotherInArmsSkill);
            this.TeachAllCamouflageSkillCommand = new RelayCommand(this.TeachAllCamouflageSkill);
            this.TeachAllFireFightingSkillCommand = new RelayCommand(this.TeachAllFireFightingSkill);
            this.TeachAllRepairSkillCommand = new RelayCommand(this.TeachAllRepairSkill);
        }

        private void InitializeCrews()
        {
            this.Crews = _configuration.Crews.Select(c => new CrewVM(c)).ToArray();

            var localGameClient = this.Repository as LocalGameClient;
            if (localGameClient != null)
            {
                this.BrotherInArmsSkillIcon = localGameClient.PackageImages.GetSkillSmallIcon(localGameClient.CrewDatabase["skills/skill[@key='brotherhood']/icon"]);
                this.CamouflageSkillIcon = localGameClient.PackageImages.GetSkillSmallIcon(localGameClient.CrewDatabase["skills/skill[@key='camouflage']/icon"]);
                this.FireFightingSkillIcon = localGameClient.PackageImages.GetSkillSmallIcon(localGameClient.CrewDatabase["skills/skill[@key='fireFighting']/icon"]);
                this.RepairSkillIcon = localGameClient.PackageImages.GetSkillSmallIcon(localGameClient.CrewDatabase["skills/skill[@key='repair']/icon"]);
            }

        }

        private void TeachAllCamouflageSkill()
        {
            this.TeachAllSkill("camouflage");
        }

        private void TeachAllFireFightingSkill()
        {
            this.TeachAllSkill("fireFighting");
        }

        private void TeachAllRepairSkill()
        {
            this.TeachAllSkill("repair");
        }

        private void TeachAllBrotherInArmsSkill()
        {
            this.TeachAllSkill("brotherhood");
        }

        private void TeachAllSkill(string skillKey)
        {
            foreach (var crew in this.Crews)
            {
                var availableSkill = crew.AvailableSkills.OfType<SkillVM>().FirstOrDefault(s => s.Model["@key"] == skillKey);
                if (availableSkill != null)
                    crew.LearnSkill(availableSkill);
            }
        }
    }
}
