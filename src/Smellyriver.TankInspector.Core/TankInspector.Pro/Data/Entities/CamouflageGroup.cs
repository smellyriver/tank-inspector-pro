using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public class CamouflageGroup : XQueryableWrapper
    {
        public new string Name { get { return this["userString"]; } }
        public string Key { get { return this["@key"]; } }

        public Camouflage[] InternalCamouflages { get; set; }
        public IEnumerable<Camouflage> Camouflages { get { return this.InternalCamouflages; } }

        public CamouflageGroup(IXQueryable data)
            : base(data)
        {

        }
    }
}
