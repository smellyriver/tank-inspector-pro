namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    abstract class OwnerModifierBase : ItemNameModifier
    {
        public abstract string Format { get; }
        public ItemName Owner { get; }

        public OwnerModifierBase(ItemName owner)
        {
            this.Owner = owner;
        }

        public override string Modify(string name)
        {
            return string.Format(this.Format, name, this.Owner.Name);
        }
    }
}
