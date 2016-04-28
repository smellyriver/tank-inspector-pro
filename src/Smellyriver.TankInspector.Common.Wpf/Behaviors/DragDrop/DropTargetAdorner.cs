using System;
using System.Windows;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop
{
    public abstract class DropTargetAdorner : Adorner
    {
        public DropTargetAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            m_AdornerLayer.Add(this);
            IsHitTestVisible = false;
        }

        public void Detatch()
        {
            m_AdornerLayer.Remove(this);
        }

        public DropInfo DropInfo { get; set; }

        internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    "The requested adorner class does not derive from DropTargetAdorner.");
            }

            return (DropTargetAdorner)type.GetConstructor(new[] { typeof(UIElement) })
                .Invoke(new[] { adornedElement });
        }

        AdornerLayer m_AdornerLayer;
    }
}
