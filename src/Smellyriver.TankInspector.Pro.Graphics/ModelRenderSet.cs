using System;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    public class ModelRenderSet : XQueryableWrapper
    {
        //todo BlendBone
        private readonly Lazy<Geometry> _lazyGeometry;

        public Geometry Geometry
        {
            get { return _lazyGeometry.Value; }
        }

        public ModelVisual Visual { get; private set; }

        public ModelRenderSet(IXQueryable data, ModelVisual visual)
            : base(data)
        {
            _lazyGeometry = new Lazy<Geometry>(this.ReadGeometry);
            this.Visual = visual;
        }

        private Geometry ReadGeometry()
        {
            return new Geometry(this.Query("geometry"));
        }

    }
}
