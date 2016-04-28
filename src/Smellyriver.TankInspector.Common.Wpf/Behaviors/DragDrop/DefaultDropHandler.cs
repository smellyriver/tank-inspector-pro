using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop.Utilities;

namespace Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop
{
    public class DefaultDropHandler : IDropTarget
    {
        public virtual void DragOver(DropInfo dropInfo)
        {
            if (DefaultDropHandler.CanAcceptData(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Copy;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        public virtual void Drop(DropInfo dropInfo)
        {
            int insertIndex = dropInfo.InsertIndex;
            IList destinationList = DefaultDropHandler.GetList(dropInfo.TargetCollection);
            IEnumerable data = DefaultDropHandler.ExtractData(dropInfo.Data);

            if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
            {
                IList sourceList = DefaultDropHandler.GetList(dropInfo.DragInfo.SourceCollection);

                foreach (object o in data)
                {
                    int index = sourceList.IndexOf(o);

                    if (index != -1)
                    {
                        sourceList.RemoveAt(index);
                        
                        if (sourceList == destinationList && index < insertIndex)
                        {
                            --insertIndex;
                        }
                    }
                }
            }

            foreach (object o in data)
            {
                destinationList.Insert(insertIndex++, o);
            }
        }

        protected static bool CanAcceptData(DropInfo dropInfo)
        {
            if (dropInfo.DragInfo == null)
                return false;

            if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
            {
                return DefaultDropHandler.GetList(dropInfo.TargetCollection) != null;
            }
            else if (dropInfo.DragInfo.SourceCollection is ItemCollection)
            {
                return false;
            }
            else
            {
                if (DefaultDropHandler.TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
                {
                    return !DefaultDropHandler.IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
                }
                else
                {
                    return false;
                }
            }
        }

        protected static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
            }
            else
            {
                return Enumerable.Repeat(data, 1);
            }
        }

        protected static IList GetList(IEnumerable enumerable)
        {
            if (enumerable is ICollectionView)
            {
                return ((ICollectionView)enumerable).SourceCollection as IList;
            }
            else
            {
                return enumerable as IList;
            }
        }

        protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
        {
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

            while (parent != null)
            {
                if (parent == sourceItem)
                {
                    return true;
                }

                parent = ItemsControl.ItemsControlFromItemContainer(parent);
            }

            return false;
        }

        protected static bool TestCompatibleTypes(IEnumerable target, object data)
        {
            TypeFilter filter = (t, o) =>
            {
                return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            };

            var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
            var enumerableTypes = from i in enumerableInterfaces select i.GetGenericArguments().Single();

            if (enumerableTypes.Count() > 0)
            {
                Type dataType = TypeUtilities.GetCommonBaseClass(DefaultDropHandler.ExtractData(data));
                return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
            }
            else
            {
                return target is IList;
            }
        }
    }
}
