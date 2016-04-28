using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Entities;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    public class ModelMaterial : XQueryableWrapper
    {

        public string Identifier
        {
            get { return this["identifier"]; }
        }

        public string Fx
        {
            get { return this["fx"]; }
        }

        public int CollisionFlags
        {
            get { return this.QueryInt("collisionFlags"); }
        }


        public int MaterialKind
        {
            get { return this.QueryInt("materialKind"); }
        }

        public bool ShowArmor { get; set; }

        internal ArmorGroup Armor { get; set; }

        private readonly Lazy<List<ModelMaterialProperty>> _lazyProperties;
        public List<ModelMaterialProperty> Properties { get { return _lazyProperties.Value; } }

        public ModelMaterial(IXQueryable data)
            : base(data)
        {
            _lazyProperties = new Lazy<List<ModelMaterialProperty>>(this.ReadProperties);
        }

        private List<ModelMaterialProperty> ReadProperties()
        {
            var properties = new List<ModelMaterialProperty>();
            properties.AddRange(this.QueryMany("property").Select(p => new ModelMaterialProperty(p)));
            return properties;
        }

    }
}
