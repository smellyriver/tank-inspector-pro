namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    class TypeModifier : ItemNameModifier
    {
        public string Type { get; set; }

        public override int Priority
        {
            get { return -100; }
        }

        public TypeModifier(string type)
        {
            this.Type = type;
        }

        public override string Modify(string name)
        {
            return string.Format("{0} {1}", this.Type, name);
        }
    }
}
