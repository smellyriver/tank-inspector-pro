namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    abstract class DataItem : PatchnoteReportItem
    {
        public ItemName ItemName { get; private set; }

        public object Data { get; private set; }

        public DataItem(string baseName, bool wrapWithBrackets, object data)
        {
            this.ItemName = new ItemName(baseName, wrapWithBrackets);
            this.Data = data;
        }
    }
}
