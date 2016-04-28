using System.Windows;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop
{
    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        public DropTargetHighlightAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.DropInfo.VisualTargetItem != null)
            {
                Rect rect = new Rect(
                   this.DropInfo.VisualTargetItem.TranslatePoint(new Point(), AdornedElement),
                    VisualTreeHelper.GetDescendantBounds(this.DropInfo.VisualTargetItem).Size);
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), rect, 2, 2);
            }
        }
    }
}
