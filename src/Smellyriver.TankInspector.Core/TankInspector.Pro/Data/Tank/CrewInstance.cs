using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using Smellyriver.TankInspector.Pro.Data.Tank.Scripting;
using Smellyriver.TankInspector.Pro.Gameplay;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public partial class CrewInstance : CoreNotificationObject
    {
        private static readonly double s_firstSkillExperience = CrewInstance.GetLevelUpExperience(100);

        private static double GetLevelUpExperience(int level)
        {
            var experience = 0.0;
            for (int i = 0; i < level; ++i)
                experience += 50 * Math.Pow(100, i * 0.01);

            return experience;
        }

        private static double ExcludeFirstFiftyPercentProficiencyExperience(double experience)
        {
            return Math.Max(experience - 9548, 0);
        }

        private static void TransferSkill(IXQueryable skill, ICollection<IXQueryable> source, ICollection<IXQueryable> destination)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

            var existedSkill = source.FirstOrDefault(s => CrewConfiguration.KeyEqualityComparer.Equals(s, skill));
            if (existedSkill == null)
                throw new ArgumentException("invalid skill", "skill");

            source.Remove(existedSkill);
            destination.Add(existedSkill);
        }

        private bool _initializingCrewInstanceInfo;

        private CrewInstanceInfo _crewInstanceInfo;
        public CrewInstanceInfo CrewInstanceInfo
        {
            get { return _crewInstanceInfo; }
            internal set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Role != this.PrimaryRole)
                    throw new ArgumentException("role mismatch", "value");

                _crewInstanceInfo = value;

                _initializingCrewInstanceInfo = true;

                this.IsDead = _crewInstanceInfo.IsDead;
                this.LastSkillTrainingLevel = _crewInstanceInfo.LastSkillTrainingLevel;

                foreach (var skill in this.LearntSkills)
                {
                    this.Configuration.ScriptHost.SetScript(this.GetScriptKey(skill), null);
                }
                _availableSkills.AddRange(this.LearntSkills);
                _learntSkills.Clear();

                foreach (var skillKey in _crewInstanceInfo.SkillKeys)
                {
                    var skill = _availableSkills.FirstOrDefault(s => s["@key"] == skillKey);
                    if (skill != null)
                        this.LearnSkill(skill);
                }

                _initializingCrewInstanceInfo = false;
            }
        }


        private readonly string _primaryRole;
        public string PrimaryRole { get { return _primaryRole; } }

        private readonly string[] _secondaryRoles;
        public IEnumerable<string> SecondaryRoles { get { return _secondaryRoles; } }

        public int RedundancyIndex { get; internal set; }

        public string Identifier
        {
            get
            {
                if (_primaryRole == "commander" || _primaryRole == "driver")
                    return _primaryRole;
                else
                    return _primaryRole + (this.RedundancyIndex + 1).ToString();
            }
        }


        private bool _isDead;
        public bool IsDead
        {
            get { return _isDead; }
            set
            {
                _isDead = value;
                this.RaisePropertyChanged("this.IsDead");

                this.CrewInstanceInfo.IsDead = _isDead;
                if (value == true)
                    this.Element.SetAttributeValue("IsDead", value);
                else
                    this.Element.SetAttributeValue("IsDead", null);

                this.InvalidateSkillEffects();
            }
        }

        private int DeadCrewCount
        {
            get { return _configuration == null ? 0 : _configuration.Crews.Count(c => c.IsDead); }
        }

        public IXQueryable[] AllRanks
        {
            get
            {
                return _configuration.Repository
                                     .CrewDatabase
                                     .QueryMany("crewDefinitions/crewDefinition[@nation='{0}']/roleRanks/ranks[@role='{1}']/rank",
                                                _configuration.NationKey,
                                                _primaryRole)
                                     .ToArray();
            }
        }

        public int RankLevel
        {
            get
            {
                return ((int)(this.LastSkillTrainingLevel / 50) - 1 + _learntSkills.Count * 2).Clamp(1, this.AllRanks.Length);
            }
        }

        public IXQueryable Rank
        {
            get { return this.AllRanks[this.RankLevel - 1]; }
        }

        public long Experience
        {
            get
            {
                var experience = s_firstSkillExperience * (Math.Pow(2, _learntSkills.Count) - 1);
                experience += Math.Pow(2, _learntSkills.Count) * CrewInstance.GetLevelUpExperience(this.LastSkillTrainingLevel);
                experience = CrewInstance.ExcludeFirstFiftyPercentProficiencyExperience(experience);
                return (long)experience;
            }
        }

        public long ExperienceToNextSkill
        {
            get
            {
                return (long)CrewInstance.ExcludeFirstFiftyPercentProficiencyExperience(s_firstSkillExperience * (Math.Pow(2, _learntSkills.Count)));
            }
        }

#if MOBILE
        private readonly List<IXQueryable> _availableSkills;
        public List<IXQueryable> AvailableSkills
        {
            get { return _availableSkills; }
        }
#else

        private readonly ObservableCollection<IXQueryable> _availableSkills;
        private readonly ReadOnlyObservableCollection<IXQueryable> _readonlyAvailableSkills;

        public ReadOnlyObservableCollection<IXQueryable> AvailableSkills
        {
            get { return _readonlyAvailableSkills; }
        }
#endif


#if MOBILE
        private readonly List<IXQueryable> _learntSkills;
        public List<IXQueryable> LearntSkills
        {
            get { return _learntSkills; }
        }
#else

        private readonly ObservableCollection<IXQueryable> _learntSkills;
        private readonly ReadOnlyObservableCollection<IXQueryable> _readonlyLearntSkills;

        public ReadOnlyObservableCollection<IXQueryable> LearntSkills
        {
            get { return _readonlyLearntSkills; }
        }
#endif

        private CrewConfiguration _configuration;
        public CrewConfiguration Configuration
        {
            get { return _configuration; }
            internal set
            {
                _configuration = value;
                this.MigrateConfiguration();
                this.NotifyExperienceAndRankChanged();
            }
        }

        private readonly XElement _element;

        internal XElement Element
        {
            get { return _element; }
        }

        internal CrewInstance(CrewConfiguration configuration, string primaryRole, string[] secondaryRoles, int redundancyIndex)
        {
            _element = new XElement("crew");
            this.Element.SetAttributeValue("primaryRole", primaryRole);
            var secondaryRolesElement = new XElement("secondaryRoles");
            foreach (var secondaryRole in secondaryRoles)
            {
                secondaryRolesElement.Add(new XElement("role", secondaryRole));
            }

            _primaryRole = primaryRole;
            _secondaryRoles = secondaryRoles;

            this.RedundancyIndex = redundancyIndex;

            this.Element.SetAttributeValue("identifier", this.Identifier);

#if MOBILE
            _availableSkills = new List<IXQueryable>();
#else
            _availableSkills = new ObservableCollection<IXQueryable>();
            _readonlyAvailableSkills = new ReadOnlyObservableCollection<IXQueryable>(_availableSkills);
#endif

#if MOBILE
            _learntSkills = new List<IXQueryable>();
#else
            _learntSkills = new ObservableCollection<IXQueryable>();
            _readonlyLearntSkills = new ReadOnlyObservableCollection<IXQueryable>(_learntSkills);

            _learntSkills.CollectionChanged += LearntSkills_CollectionChanged;
#endif

            this.Configuration = configuration;
            _crewInstanceInfo = new CrewInstanceInfo(this.PrimaryRole);


            this.LastSkillTrainingLevel = 100;
        }

#if !MOBILE
        void LearntSkills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnLearntSkillsChanged();
        }
#endif

        private void OnLearntSkillsChanged()
        {

            this.NotifyExperienceAndRankChanged();

            var skillsElement = new XElement("skills");
            foreach (var skill in this.LearntSkills)
            {
                skillsElement.Add(skill.ToElement());
            }

            var oldSkillsElement = this.Element.Element("skills");
            if (oldSkillsElement != null)
                oldSkillsElement.ReplaceWith(skillsElement);
            else
                this.Element.Add(skillsElement);

            if (!_initializingCrewInstanceInfo)
            {
                this.CrewInstanceInfo.SkillKeys = this.LearntSkills.Select(s => s["@key"]).ToArray();
            }
        }

        private void NotifyExperienceAndRankChanged()
        {
            this.RaisePropertyChanged("Experience");
            this.Element.SetElementValue("experience", this.Experience);
            this.RaisePropertyChanged("ExperienceToNextSkill");
            this.RaisePropertyChanged("RankLevel");
            this.RaisePropertyChanged("Rank");
            this.Element.SetElementValue("rank", this.Rank.Value);
            this.Element.Element("rank").SetAttributeValue("level", this.RankLevel);
        }


        private void MigrateConfiguration()
        {
            this.Configuration.ScriptHost.SetScript(this.Identifier, BasicSkillScript.Create(_primaryRole, () => this.ActualBasicTrainingLevel));
            foreach (var secondaryRole in _secondaryRoles)
                this.Configuration.ScriptHost.SetScript(secondaryRole, BasicSkillScript.Create(secondaryRole, () => this.ActualBasicTrainingLevel));

            var skills = SkillHelper.GetSkills(_configuration.Repository, _primaryRole, _secondaryRoles, true);
            if (_learntSkills.Count == 0 && _availableSkills.Count == 0)
            {
                _availableSkills.AddRange(skills);
            }
            else
            {
                var skillArray = skills.ToArray();
                this.MigrateSkills(_learntSkills, skillArray);
                this.MigrateSkills(_availableSkills, skillArray);
            }

        }

        private void MigrateSkills(IList<IXQueryable> oldSkills, IEnumerable<IXQueryable> newSkills)
        {
            var skillsToRemove = new List<int>();
            for (var i = 0; i < oldSkills.Count; ++i)
            {
                var replacement = newSkills.FirstOrDefault(s => CrewConfiguration.KeyEqualityComparer.Equals(oldSkills[i], s));
                if (replacement == null)
                    skillsToRemove.Add(i);
                else
                {
                    oldSkills[i] = replacement;
                    this.Configuration.ScriptHost.SetScript(this.GetScriptKey(replacement),
                                                            SkillScript.Create(replacement, () => this.ActualLearntSkillTrainingLevel));
                }
            }

            for (var i = skillsToRemove.Count - 1; i >= 0; --i)
                oldSkills.RemoveAt(skillsToRemove[i]);

            this.SetLastSkillScriptLevelGetter(() => this.ActualLastSkillTrainingLevel);
        }

        private string GetScriptKey(IXQueryable skill)
        {
            return string.Format("{0}:{1}", this.Identifier, skill["@key"]);
        }

        public void LearnSkill(string key)
        {
            var skill = this.AvailableSkills.FirstOrDefault(s => s["@key"] == key);
            if (skill != null)
                this.LearnSkill(skill);
        }

        public void ForgetSkill(string key)
        {
            var skill = this.LearntSkills.FirstOrDefault(s => s["@key"] == key);
            if (skill != null)
                this.ForgetSkill(skill);
        }

        public void LearnSkill(IXQueryable skill)
        {
            this.SetLastSkillScriptLevelGetter(() => this.ActualLearntSkillTrainingLevel);

            CrewInstance.TransferSkill(skill, _availableSkills, _learntSkills);
            this.Configuration.ScriptHost.SetScript(this.GetScriptKey(skill), SkillScript.Create(skill, () => this.ActualLastSkillTrainingLevel));

            this.InvalidateSkillEffects();
        }

        public void ForgetSkill(IXQueryable skill)
        {
            CrewInstance.TransferSkill(skill, _learntSkills, _availableSkills);
            this.Configuration.ScriptHost.SetScript(this.GetScriptKey(skill), null);
            this.SetLastSkillScriptLevelGetter(() => this.ActualLastSkillTrainingLevel);

            this.InvalidateSkillEffects();
        }

        private void SetLastSkillScriptLevelGetter(Func<double> levelGetter)
        {
            var lastSkill = this.LearntSkills.LastOrDefault();
            if (lastSkill != null)
            {
                var lastSkillScript = this.Configuration.ScriptHost.GetScript(this.GetScriptKey(lastSkill)) as SkillScriptBase;
                lastSkillScript.LevelGetter = levelGetter;
            }
        }

        private void InvalidateSkillEffects()
        {
            this.Configuration.ScriptHost.Invalidate();
        }

    }
}
