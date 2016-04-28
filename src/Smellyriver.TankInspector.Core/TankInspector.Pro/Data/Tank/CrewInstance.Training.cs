using Smellyriver.TankInspector.Pro.Data.Tank.Scripting;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    partial class CrewInstance
    {

        private int _lastSkillTrainingLevel;
        public int LastSkillTrainingLevel
        {
            get { return _lastSkillTrainingLevel; }
            set
            {
                _lastSkillTrainingLevel = value;

                this.CrewInstanceInfo.LastSkillTrainingLevel = _lastSkillTrainingLevel;
                this.Element.SetElementValue("trainingLevel", value);

                this.NotifyTrainingLevelChanged();
                this.NotifyExperienceAndRankChanged();
                this.InvalidateSkillEffects();
            }
        }

        private double _buffTrainingLevel;
        public double BuffTrainingLevel
        {
            get
            {
                var context = this.Configuration.ScriptHost.ScriptingContext;
                double commanderBuff;
                if (this.PrimaryRole == "commander")
                    commanderBuff = 0;
                else
                    commanderBuff = context.GetValue("commander", CommanderBasicSkillScript.CrewTrainingLevelBuffSkillKey, 0.0);
                var brotherhoodValue = context.GetValue("brotherhood", "crewLevelIncrease", 0.0);
                var ventValue = context.GetValue("staticAdditiveDevice", "miscAttrs/crewLevelIncrease", 0.0);
                var stimulatorValue = context.GetValue("stimulator", "crewLevelIncrease", 0.0);
                var result = commanderBuff + brotherhoodValue + ventValue + stimulatorValue;
                var changed = _buffTrainingLevel != result;
                _buffTrainingLevel = result;

                if (changed)
                    this.RaisePropertyChanged("BuffTrainingLevel");

                return result;
            }
        }

        private double JackOfAllTradesProvidedTrainingLevel
        {
            get
            {
                var deadCount = this.DeadCrewCount;
                if (deadCount == 0)
                    return 0;

                var trainingLevel = this.Configuration.ScriptHost.ScriptingContext.GetValue("universalist", "efficiency", 0.0);
                return trainingLevel / deadCount;
            }
        }

        public double ActualBasicTrainingLevel
        {
            get
            {
                if (this.IsDead)
                    return this.JackOfAllTradesProvidedTrainingLevel;
                else
                    return (this.LearntSkills.Count > 0 ? 100 : this.LastSkillTrainingLevel) + this.BuffTrainingLevel;
            }
        }

        public double ActualLearntSkillTrainingLevel
        {
            get
            {
                if (this.IsDead)
                    return 0;
                else
                    return 100 + this.BuffTrainingLevel;
            }
        }

        public double ActualLastSkillTrainingLevel
        {
            get
            {
                if (this.IsDead)
                    return 0;
                else
                    return this.LastSkillTrainingLevel + this.BuffTrainingLevel;
            }
        }

        private void NotifyTrainingLevelChanged()
        {
            this.RaisePropertyChanged("LastSkillTrainingLevel");
            this.RaisePropertyChanged("ActualBasicTrainingLevel");
            this.RaisePropertyChanged("ActualLearntSkillTrainingLevel");
            this.RaisePropertyChanged("ActualLastSkillTrainingLevel");
        }
    }
}
