using System;
using System.Diagnostics;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DebuggerDisplay("Changed: {ItemName}")]
    class ChangedItem : DataItem
    {

        public ChangeVerb Verb { get; }
        public string OldValue { get; }
        public string NewValue { get; }

        public ChangedItem(string baseName, ChangeVerb verb, string oldValue, string newValue, object data)
            : base(baseName, false, data)
        {
            this.Verb = verb;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }


        public override Block[] CreateBlocks()
        {
            return new[] { 
                new Paragraph(
                    new Run(this.L("patchnote_generator", "generated_document_changed_item_format",
                                   string.Format(this.GetChangeVerbActionFormat(),  this.ItemName.Name), 
                                   this.OldValue, 
                                   this.NewValue))) 
            };
        }

        private string GetChangeVerbActionFormat()
        {
            switch (this.Verb)
            {
                case ChangeVerb.Buffed:
                    return this.L("patchnote_generator", "generated_document_buffed_action_format");
                case ChangeVerb.Changed:
                    return this.L("patchnote_generator", "generated_document_changed_action_format");
                case ChangeVerb.Decreased:
                    return this.L("patchnote_generator", "generated_document_decreased_action_format");
                case ChangeVerb.Increased:
                    return this.L("patchnote_generator", "generated_document_increased_action_format");
                case ChangeVerb.Nerfed:
                    return this.L("patchnote_generator", "generated_document_nerfed_action_format");
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
