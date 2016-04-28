using System.Diagnostics;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DebuggerDisplay("Removed: {ItemName}")]
    class RemovedItem : AddedOrRemovedItemBase
    {
        public RemovedItem(string baseName, object data)
            : base(baseName, data)
        {

        }

        public override string ActionFormat
        {
            get { return this.L("patchnote_generator", "generated_document_removed_action_format"); }
        }
    }
}
