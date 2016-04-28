namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    class ParentModifier : OwnerModifierBase
    {
        public override string Format
        {
            get { return this.L("patchnote_generator", "generated_document_parent_modifier"); }
        }

        public override int Priority
        {
            get { return 100; }
        }

        public ParentModifier(ItemName parent)
            : base(parent)
        {

        }

    }
}
