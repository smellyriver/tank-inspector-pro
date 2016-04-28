using System;

namespace Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop
{
    public class DropTargetAdorners
    {
        public static Type Highlight
        {
            get { return typeof(DropTargetHighlightAdorner); }
        }

        public static Type Insert
        {
            get { return typeof(DropTargetInsertionAdorner); }
        }
    }
}
