using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Collections;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.CrewConfigurator
{
    class CrewVM : NotificationObject
    {
        public CrewInstance Model { get; }

        public string PrimaryRole
        {
            get { return this.GetRoleName(this.Model.PrimaryRole); }
        }

        public IEnumerable<string> SecondaryRoles
        {
            get { return this.Model.SecondaryRoles.Select(this.GetRoleName); }
        }

        public IEnumerable<ImageSource> RoleIcons
        {
            get
            {
                var localGameClient = this.Model.Configuration.Repository as LocalGameClient;
                if (localGameClient == null)
                    yield break;

                yield return localGameClient.PackageImages.GetCrewRoleIcon(this.Model.PrimaryRole);

                foreach (var secondaryRole in this.Model.SecondaryRoles)
                    yield return localGameClient.PackageImages.GetCrewRoleIcon(secondaryRole);
            }
        }

        private SkillVMBase[] _availableSkills;
        public SkillVMBase[] AvailableSkills
        {
            get { return _availableSkills; }
            private set
            {
                _availableSkills = value;
                this.RaisePropertyChanged(() => this.AvailableSkills);
                this.RaisePropertyChanged(() => this.HasAvailableSkills);
                this.SkillToLearn = EmptySkillVM.Instance;
            }
        }

        private SkillVMBase _skillToLearn = EmptySkillVM.Instance;
        public SkillVMBase SkillToLearn
        {
            get { return _skillToLearn; }
            set
            {
                _skillToLearn = value;

                if (_skillToLearn != null && _skillToLearn != EmptySkillVM.Instance)
                    this.LearnSkill(value);

                this.RaisePropertyChanged(() => this.SkillToLearn);
            }
        }

        public ViewModelMap<IXQueryable, SkillVM> LearntSkills { get; private set; }

        public bool HasAvailableSkills
        {
            get { return this.AvailableSkills.Length > 0; }
        }

        private readonly string _isExpandedSettingPropertyName;
        private readonly PropertyInfo _isExpandedSettingProperty;

        public bool IsExpanded
        {
            get { return (bool)_isExpandedSettingProperty.GetValue(CrewConfiguratorSettings.Default, null); }
            set
            {
                _isExpandedSettingProperty.SetValue(CrewConfiguratorSettings.Default, value, null);
                CrewConfiguratorSettings.Default.Save();
            }
        }

        public bool IsAlive
        {
            get { return !this.Model.IsDead; }
            set
            {
                this.Model.IsDead = !value;
                this.RaisePropertyChanged(() => this.IsAlive);
            }
        }


        public CrewVM(CrewInstance crew)
        {
            this.Model = crew;

            this.UpdateAvailableSkills();
            this.LearntSkills = new ViewModelMap<IXQueryable, SkillVM>(this.Model.LearntSkills, this.CreateSkillVM);

            ((INotifyCollectionChanged)this.Model.AvailableSkills).CollectionChanged += OnAvailableSkillsChanged;

            var identifier = crew.Identifier;

            _isExpandedSettingPropertyName = identifier.Substring(0, 1).ToUpper()
                                           + identifier.Substring(1)
                                           + "Expanded";

            _isExpandedSettingProperty = typeof(CrewConfiguratorSettings).GetProperty(_isExpandedSettingPropertyName);

            CrewConfiguratorSettings.Default.PropertyChanged += OnCrewConfiguratorSettingChanged;
        }


        void OnCrewConfiguratorSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _isExpandedSettingPropertyName)
                this.RaisePropertyChanged(() => this.IsExpanded);
        }

        private void UpdateAvailableSkills()
        {
            var skills = new List<SkillVMBase>();
            skills.AddRange(this.Model.AvailableSkills.Select(this.CreateSkillVM));

            if (skills.Count > 0)
            {
                skills.Insert(0, EmptySkillVM.Instance);
            }

            this.AvailableSkills = skills.ToArray();
        }

        private SkillVM CreateSkillVM(IXQueryable skill)
        {
            return new SkillVM(this, this.Model.Configuration.Repository, skill);
        }

        void OnAvailableSkillsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateAvailableSkills();
        }

        private string GetRoleName(string role)
        {
            return this.Model.Configuration.Repository.CrewDatabase.QueryValue("roles/role[@key='{0}']/userString", role);
        }

        public void LearnSkill(SkillVMBase skill)
        {
            this.Model.LearnSkill(skill.Model);
        }

        public void ForgetSkill(SkillVMBase skill)
        {
            this.Model.ForgetSkill(skill.Model);
        }
    }
}
