using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DebuggerDisplay("Collection: {ItemName}")]
    class Collection : DataItem
    {
        public List<PatchnoteReportItem> Children { get; }

        public Collection(string name, bool wrapWithBrackets, object data)
            : base(name, wrapWithBrackets, data)
        {
            this.Children = new List<PatchnoteReportItem>();
        }

        public Collection(string name, bool wrapWithBrackets, IEnumerable<PatchnoteReportItem> children, object data)
            : base(name, wrapWithBrackets, data)
        {
            this.Children = new List<PatchnoteReportItem>(children);
        }

        public override Block[] CreateBlocks()
        {
            var list = new List();

            foreach(var child in this.Children)
            {
                var listItem = new ListItem();
                listItem.Blocks.AddRange(child.CreateBlocks());
                list.ListItems.Add(listItem);
            }

            return new Block[] 
            { 
                new Paragraph(new Run(this.ItemName.Name)),
                list
            };
            
        }

    }
}
