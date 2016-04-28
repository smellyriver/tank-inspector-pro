using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    public class Geometry : XQueryableWrapper
    {
        public string VerticesName
        {
            get { return this["vertices"].Trim(); }
        }

        public string IndicesName
        {
            get { return this["primitive"].Trim(); }
        }

        public string StreamName
        {
            get { return this["stream"].Trim(); }
        }

        private readonly Lazy<Dictionary<int, ModelPrimitiveGroup>> _lazyModelPrimitiveGroups;
        public Dictionary<int, ModelPrimitiveGroup> ModelPrimitiveGroups
        {
            get { return _lazyModelPrimitiveGroups.Value; }
        }

        public Geometry(IXQueryable geometry)
            : base(geometry)
        {
            _lazyModelPrimitiveGroups = new Lazy<Dictionary<int, ModelPrimitiveGroup>>(this.ReadModelPrimitiveGroups);
        }

        private Dictionary<int, ModelPrimitiveGroup> ReadModelPrimitiveGroups()
        {
            return this.QueryMany("primitiveGroup").Select(g => new ModelPrimitiveGroup(g))
                                                   .ToDictionary(g => g.Id);
        }

    }
}
