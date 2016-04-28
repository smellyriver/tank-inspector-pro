using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    class TankDataItemHandler
    {
        public readonly static TankDataItemHandler Instance = new TankDataItemHandler();

        public static TankDataFieldHandler GetHandler(TankDataFieldType fieldType)
        {
            switch (fieldType)
            {
                case TankDataFieldType.Value:
                    return ValueDataFieldHandler.Instance;
                case TankDataFieldType.SimpleCollection:
                    return SimpleCollectionDataFieldHandler.Instance;
                case TankDataFieldType.ComplexCollection:
                    return ComplexCollectionDataFieldHandler.Instance;
                default:
                    throw new NotSupportedException();
            }
        }

        public PatchnoteReportItem CreateReportItem(string name,
                                                    IXQueryable oldItem,
                                                    IXQueryable newItem,
                                                    bool doCompact,
                                                    bool isSingleParent = false,
                                                    bool wrapTitleWithBrackets = true)
        {

            var children = new List<PatchnoteReportItem>();
            var fields = TankDataFieldManager.Instance.GetFields(oldItem.Name);
            foreach (var field in fields)
            {
                var handler = TankDataItemHandler.GetHandler(field.FieldType);
                var reportItem = handler.CreateReportItems(field.Name, field, oldItem, newItem);

                if (reportItem != null)
                    children.AddRange(reportItem);
            }

            if (children.Count == 0)
                return null;

            if (doCompact && children.Count == 1)
            {
                var changedChild = children[0] as ChangedItem;
                if (changedChild != null)
                {
                    this.AddAttributeOrParentModifier(changedChild.ItemName, name, isSingleParent);
                    return changedChild;
                }

                var addedOrRemovedChild = children[0] as AddedOrRemovedItemBase;
                if (addedOrRemovedChild != null)
                {
                    if (!isSingleParent)
                        this.AddParentModifier(addedOrRemovedChild.ItemName, name, isSingleParent);
                    return addedOrRemovedChild;
                }

                var collectionChild = children[0] as Collection;
                if (collectionChild != null)
                {
                    if (!isSingleParent)
                        this.AddParentModifier(collectionChild.ItemName, name, isSingleParent);
                    return collectionChild;
                }


                throw new NotImplementedException();
            }
            else
                return new Collection(name, wrapTitleWithBrackets, children, oldItem);
        }

        private void AddAttributeOrParentModifier(ItemName itemName, string modifierName, bool isSingleParent)
        {
            if (itemName.Modifiers.Any(m => m is AttributeModifier))
                this.AddParentModifier(itemName, modifierName, isSingleParent);
            else
                itemName.AddModifier(new AttributeModifier(new ItemName(modifierName)));
        }

        private void AddParentModifier(ItemName itemName, string parentName, bool isSingleParent)
        {
            if (isSingleParent)
                return;

            itemName.AddModifier(new ParentModifier(new ItemName(parentName)));
        }
    }
}
