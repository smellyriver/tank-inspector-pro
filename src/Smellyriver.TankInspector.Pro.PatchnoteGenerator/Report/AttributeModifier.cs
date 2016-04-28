namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    class AttributeModifier : OwnerModifierBase
    {
        public override int Priority
        {
            get { return 0; }
        }

        public override string Format
        {
            get { return  this.L("patchnote_generator", "generated_document_attribute_modifier"); }
        }
        

        public AttributeModifier(ItemName owner)
            : base(owner)
        {
            
        }

    }
}
