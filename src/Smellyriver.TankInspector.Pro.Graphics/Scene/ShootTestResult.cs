using System;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    public class ShootTestResult
    {

        public static readonly ShootTestResult NotApplicable = new ShootTestResult(PenetrationState.NotApplicable, 0, 0, 0, false, false);
        public int EquivalentThickness { get; }
        public PenetrationState PenetrationState { get; }
        public int ImpactAngle { get; }
        public int NormalizationAngle { get; }
        public bool Is2xRuleActive { get; }
        public bool Is3xRuleActive { get; }

        public ShootTestResult(PenetrationState state, double equivalentThickness, double impactAngle, double normalizationAngle, bool is2xRuleActive, bool is3xRuleActive)
        {
            this.PenetrationState = state;
            this.EquivalentThickness = (int)Math.Round(equivalentThickness);
            this.ImpactAngle = (int)Math.Round(impactAngle);
            this.NormalizationAngle = (int)Math.Round(normalizationAngle);
            this.Is2xRuleActive = is2xRuleActive;
            this.Is3xRuleActive = is3xRuleActive;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ShootTestResult;
            if (other == null)
                return false;

            return this.EquivalentThickness == other.EquivalentThickness
                && this.PenetrationState == other.PenetrationState
                && this.ImpactAngle == other.ImpactAngle
                && this.NormalizationAngle == other.NormalizationAngle
                && this.Is2xRuleActive == other.Is2xRuleActive
                && this.Is3xRuleActive == other.Is3xRuleActive;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
