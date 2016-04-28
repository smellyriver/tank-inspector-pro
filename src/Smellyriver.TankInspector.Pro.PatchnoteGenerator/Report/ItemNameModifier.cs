namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    abstract class ItemNameModifier
    {
        public abstract int Priority { get; }
        public abstract string Modify(string name);
    }
}
