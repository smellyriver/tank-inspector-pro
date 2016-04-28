using System;
using System.Globalization;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    public class ModelPrimitiveGroup : XQueryableWrapper
    {

        public int Id { get; private set; }

        private readonly Lazy<ModelMaterial> _lazyMaterial;

        public ModelMaterial Material
        {
            get { return _lazyMaterial.Value; }
        }

        public uint StartIndex { get; set; }

        public uint StartVertex { get; set; }

        public uint EndIndex { get; set; }

        public uint EndVertex { get; set; }

        public bool Sectioned { get; set; }

        public uint PrimitiveCount { get; set; }

        public uint VerticesCount { get; set; }

        public ModelPrimitiveGroup(IXQueryable data)
            : base(data)
        {
            this.Id = int.Parse(this.Text, CultureInfo.InvariantCulture);
            _lazyMaterial = new Lazy<ModelMaterial>(this.ReadMaterial);
        }

        private ModelMaterial ReadMaterial()
        {
            return new ModelMaterial(this.Query("material"));
        }

    }
}
