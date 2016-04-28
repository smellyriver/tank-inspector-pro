using SharpDX;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    public class ModelMaterialProperty : XQueryableWrapper
    {
        public override string Name
        {
            get { return this.Text.Trim(); }
        }

        public string Texture
        {
            get { return this["Texture"]; }
        }

        public int IntValue
        {
            get { return this.QueryInt("Int"); }
        }

        public bool BoolValue
        {
            get { return this.QueryBool("Bool"); }
        }
        public float FloatValue
        {
            get { return this.QueryFloat("Float"); }
        }

        private Vector4 _vector4Value;

        public Vector4 Vector4Value
        {
            get { return this.QueryVector4("Vector4"); }
            set { _vector4Value = value; }
        }

        public ModelMaterialProperty(IXQueryable data)
            : base(data)
        {

        }

    }
}
