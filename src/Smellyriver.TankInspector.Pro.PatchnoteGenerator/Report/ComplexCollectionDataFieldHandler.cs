using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Gameplay;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    class ComplexCollectionDataFieldHandler : TankDataFieldHandler
    {


        public readonly static ComplexCollectionDataFieldHandler Instance = new ComplexCollectionDataFieldHandler();

        public override PatchnoteReportItem[] CreateReportItems(string name, TankDataFieldBase field, IXQueryable oldItem, IXQueryable newItem)
        {
            var oldItems = oldItem.QueryMany(field.XPath);
            var newItems = newItem.QueryMany(field.XPath);

            var diffResult = oldItems.Diff(newItems, KeyEqualityComparer<IXQueryable>.Instance);
            var children = new List<PatchnoteReportItem>();

            foreach (var addedItem in diffResult.Added)
                children.Add(new AddedItem(addedItem["userString"], addedItem));

            foreach (var removedItem in diffResult.Removed)
                children.Add(new RemovedItem(removedItem["userString"], removedItem));

            var isSingleParent = oldItems.Count() == 1;

            foreach (var sharedItem in diffResult.Shared)
            {
                var reportItem = TankDataItemHandler.Instance.CreateReportItem(sharedItem.Source["userString"],
                                                                               sharedItem.Source,
                                                                               sharedItem.Target,
                                                                               true,
                                                                               isSingleParent);

                children.AddIfNotNull(reportItem);
            }

            if (children.Count == 0)
                return null;

            var collectionField = (ComplexCollectionDataField)field;

            foreach (var singleItemChild in children.OfType<DataItem>())
            {
                if (singleItemChild.ItemName.Modifiers.LastOrDefault() as TypeModifier == null)
                    this.AddTypeModifier(singleItemChild.ItemName, collectionField.ItemName, isSingleParent);
            }

            if (children.Count == 1)
            {
                var changedChild = children[0] as ChangedItem;
                if (changedChild != null)
                    return new[] { changedChild };

                var addedOrRemovedChild = children[0] as AddedOrRemovedItemBase;
                if (addedOrRemovedChild != null)
                    return new[] { addedOrRemovedChild };


                var collectionChild = children[0] as Collection;

                if (collectionField.OmitSingleHeader && collectionChild.Children.All(c => c is Collection))
                    return collectionChild.Children.ToArray();
            }

            return children.ToArray();
        }

        private void AddTypeModifier(ItemName itemName, string typeName, bool isSingleParent)
        {

            var ownerModifier = itemName.Modifiers.OfType<OwnerModifierBase>().LastOrDefault();

            if (isSingleParent && ownerModifier != null && ownerModifier.Owner.Modifiers.OfType<TypeModifier>().Any())
                return;

            var typeModifier = new TypeModifier(typeName);
            if (ownerModifier == null)
                itemName.AddModifier(typeModifier);
            else
                ownerModifier.Owner.AddModifier(typeModifier);

        }
    }
}
