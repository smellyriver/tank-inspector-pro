using System.Collections.Generic;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    class SimpleCollectionDataFieldHandler : TankDataFieldHandler
    {
        

        public readonly static SimpleCollectionDataFieldHandler Instance = new SimpleCollectionDataFieldHandler();

        public override PatchnoteReportItem[] CreateReportItems(string name, TankDataFieldBase field, IXQueryable oldItem, IXQueryable newItem)
        {
            var oldValues = oldItem.QueryManyValues(field.XPath);
            var newValues = newItem.QueryManyValues(field.XPath);

            var diffResult = oldValues.Diff(newValues);

            var itemCount = diffResult.Added.Length + diffResult.Removed.Length;

            if (itemCount == 0)
                return null;

            var children = new List<AddedOrRemovedItemBase>();

            foreach (var added in diffResult.Added)
                children.Add(new AddedItem(added, added));

            foreach (var removed in diffResult.Removed)
                children.Add(new RemovedItem(removed, removed));

            if (children.Count == 1)
            {
                var collectionField = (SimpleCollectionDataField)field;

                children[0].ItemName.AddModifier(new TypeModifier(collectionField.ItemName));
                return children.ToArray();

            }
            else
            {
                var collection = new Collection(name, false, oldItem);

                foreach (var added in diffResult.Added)
                    collection.Children.Add(new AddedItem(added, added));

                foreach (var removed in diffResult.Removed)
                    collection.Children.Add(new RemovedItem(removed, removed));

                return new[] { collection };
            }
        }
    }
}
