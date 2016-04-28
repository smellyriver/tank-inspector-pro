using System;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public class Camouflage : XQueryableWrapper
    {
        private static CamouflageKind ParseCamouflageKind(string kind)
        {
            switch(kind)
            {
                case "summer" :
                    return CamouflageKind.Summer;
                case "winter":
                    return CamouflageKind.Winter;
                case "desert":
                    return CamouflageKind.Desert;
                default:
                    throw new NotSupportedException();
            }
        }
        public string Key { get { return this["@key"]; } }
        public string Texture { get { return this["texture"]; } }
        public double PriceFactor { get { return this.QueryDouble("priceFactor"); } }
        public double CamouflageFactor { get { return this.QueryDouble("invisibilityFactor"); } }
        public string Description { get { return this["description"]; } }
        public int Id { get { return this.QueryInt("@id"); } }
        public CamouflageKind Kind { get { return Camouflage.ParseCamouflageKind(this["kind"].Trim()); } }
        
        public CamouflageGroup Group { get; internal set; }

        public Camouflage(IXQueryable data)
            : base(data)
        {

        }

        private string[] LoadDeniedTankKeys()
        {
            var deny = this["deny"];
            if (deny == null)
                return new string[0];

            return deny.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool GetIsDenied(string tankKey)
        {
            return this.QueryManyValues("deny").Any(d => d.Trim() == tankKey);
        }
    }
}
