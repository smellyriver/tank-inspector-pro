using System.Windows.Documents;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{

    abstract class AddedOrRemovedItemBase : DataItem
    {
        public abstract string ActionFormat { get; }

        public AddedOrRemovedItemBase(string baseName, object data)
            : base(baseName, true, data)
        {

        }

        public override Block[] CreateBlocks()
        {
            return new[] { new Paragraph(new Run(string.Format(this.ActionFormat, this.ItemName.Name))) };
        }
    }
}
