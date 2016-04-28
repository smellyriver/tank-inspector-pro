using System.Windows.Documents;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    class PotentialModelChangedItem : PatchnoteReportItem
    {
        public string TankName { get; }
        public ItemName[] ChangedModules { get; }

        public PotentialModelChangedItem(string tankName, ItemName[] changedModules)
        {
            this.TankName = tankName;
            this.ChangedModules = changedModules;
        }

        public override Block[] CreateBlocks()
        {
            var list = new List();
            foreach (var changedModule in this.ChangedModules)
                list.ListItems.Add(new ListItem(new Paragraph(new Run(changedModule.Name))));

            return new Block[]
            {
                new Paragraph(new Run(this.TankName)),
                list
            };
        }
    }
}
