using System.Diagnostics;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DebuggerDisplay("Added: {ItemName}")]
    class AddedItem : AddedOrRemovedItemBase
    {
        public AddedItem(string baseName, object data)
            : base(baseName, data)
        {

        }

        public override string ActionFormat
        {
            get { return this.L("patchnote_generator", "generated_document_added_action_format"); }
        }
    }
}
