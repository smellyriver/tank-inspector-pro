namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public class Inscription : XQueryableWrapper
    {
        public new string Name { get { return this["userString"]; } }
        public string TextureName { get { return this["texName"]; } }
        public int Id { get { return this.QueryInt("@id"); } }

        public Inscription(IXQueryable data)
            : base(data)
        {

        }
    }
}
