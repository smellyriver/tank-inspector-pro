namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class ArmorGroup : CachedXQueryable
    {
        // this value is freqently used, cache to optimize it
        public double VehicleDamageFactor { get; }

        public bool IsSpacedArmor
        {
            get { return this.VehicleDamageFactor == 0; }
        }

        public bool UseArmorHomogenization
        {
            get { return this.QueryBool("useArmorHomogenization"); }
        }

        public bool UseHitAngle
        {
            get { return this.QueryBool("useHitAngle"); }
        }

        public bool UseAntifragmentationLining
        {
            get { return this.QueryBool("useAntifragmentationLining"); }
        }

        public bool MayRicochet
        {
            get { return this.QueryBool("mayRicochet"); }
        }
        public bool CollideOnceOnly
        {
            get { return this.QueryBool("CollideOnceOnly"); }
        }

        public bool ContinueTraceIfNoHit
        {
            get { return this.QueryBool("continueTraceIfNoHit"); }
        }

        // this value is freqently used, cache to optimize it
        public double Thickness { get; private set; }

        public string Key
        {
            get { return this["@key"]; }
        }

        public double ChanceToHitByProjectile
        {
            get { return this.QueryDouble("chanceToHitByProjectile"); }
        }

        public double ChanceToHitByExplosion
        {
            get { return this.QueryDouble("chanceToHitByExplosion"); }
        }

        public ArmorGroup(IXQueryable data)
            : base(data)
        {
            this.VehicleDamageFactor = this.QueryDouble("vehicleDamageFactor");
            this.Thickness = this.QueryDouble("thickness");
        }
    }
}
