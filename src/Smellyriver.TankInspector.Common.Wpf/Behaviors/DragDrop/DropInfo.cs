using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop.Utilities;

namespace Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop
{
    public class DropInfo
    {
        public DropInfo(object sender, DragEventArgs e, DragInfo dragInfo, string dataFormat)
        {
            this.Data = (e.Data.GetDataPresent(dataFormat)) ? e.Data.GetData(dataFormat) : e.Data;
            this.DragInfo = dragInfo;

            this.VisualTarget = sender as UIElement;

            if (sender is ItemsControl)
            {
                ItemsControl itemsControl = (ItemsControl)sender;
                UIElement item = itemsControl.GetItemContainerAt(e.GetPosition(itemsControl));

                this.VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();

                if (item != null)
                {
                    ItemsControl itemParent = ItemsControl.ItemsControlFromItemContainer(item);

                    this.InsertIndex = itemParent.ItemContainerGenerator.IndexFromContainer(item);
                    this.TargetCollection = itemParent.ItemsSource ?? itemParent.Items;
                    this.TargetItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    this.VisualTargetItem = item;

                    if (this.VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (e.GetPosition(item).Y > item.RenderSize.Height / 2) this.InsertIndex++;
                    }
                    else
                    {
                        if (e.GetPosition(item).X > item.RenderSize.Width / 2) this.InsertIndex++;
                    }
                }
                else
                {
                    this.TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                    this.InsertIndex = itemsControl.Items.Count;
                }
            }
        }

        public object Data { get; private set; }
        public DragInfo DragInfo { get; private set; }
        public Type DropTargetAdorner { get; set; }
        public DragDropEffects Effects { get; set; }
        public int InsertIndex { get; private set; }
        public IEnumerable TargetCollection { get; private set; }
        public object TargetItem { get; private set; }
        public UIElement VisualTarget { get; private set; }
        public UIElement VisualTargetItem { get; private set; }
        public Orientation VisualTargetOrientation { get; private set; }

        public IEnumerable<T> GetEnumerableData<T>()
        {
            if (this.Data is IEnumerable && !(this.Data is string))
                return ((IEnumerable)this.Data).OfType<T>();
            else if (this.Data is T)
                return new [] { (T)this.Data };
            else
                return new T[0];
        }
    }
}
