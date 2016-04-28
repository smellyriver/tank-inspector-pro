using System;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class Shell : Component
    {
        public double Penetration100 { get { return this.QueryDouble("piercingPower/p100"); } }
        public double Penetration400 { get { return this.QueryDouble("piercingPower/p400"); } }
        public double Damage { get { return this.QueryDouble("damage/armor"); } }
        public double ModuleDamage { get { return this.QueryDouble("damage/device"); } }
        public string IconKey { get { return this["icon"]; } }

        public bool IsKinetic
        {
            get { return this.Type.IsKineticShellType(); }
        }

        public bool HasNormalizationEffect
        {
            get { return this.Type.HasNormalizationEffect(); }
        }


        public double BaseNormalization
        {
            get { return this.Type.GetBaseNormalization(); }
        }

        public double RicochetAngle
        {
            get
            {
                switch (this["kind"])
                {
                    case "HIGH_EXPLOSIVE":
                    case "HIGH_EXPLOSIVE_PREMIUM":
                        return 90.0;
                    case "HOLLOW_CHARGE":
                        return 85.0;
                    case "ARMOR_PIERCING":
                    case "ARMOR_PIERCING_HE":
                    case "ARMOR_PIERCING_CR":
                        return 70.0;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public ShellType Type
        {
            get
            {
                switch (this["kind"])
                {
                    case "HIGH_EXPLOSIVE":
                        return ShellType.HE;
                    case "HIGH_EXPLOSIVE_PREMIUM":
                        return ShellType.PremiumHE;
                    case "HOLLOW_CHARGE":
                        return ShellType.HEAT;
                    case "ARMOR_PIERCING":
                        return ShellType.AP;
                    case "ARMOR_PIERCING_HE":
                        return ShellType.APHE;
                    case "ARMOR_PIERCING_CR":
                        return ShellType.APCR;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public Shell(IXQueryable shellData)
            : base(shellData)
        {

        }

    }
}
