using System;
using System.Linq;
using System.Reflection;
using Smellyriver.TankInspector.Pro.UserInterface.ViewModels;
using Xceed.Wpf.AvalonDock.Layout;

namespace Smellyriver.TankInspector.Pro.UserInterface
{
    [Obsolete("this is no more needed apparently")]
    class LayoutUpdateStrategy : ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            if (destinationContainer != null)
                return false;

            // WARNING: tricky code, do not modify unless you know what you are doing

            // if the pane is floating let the manager go ahead
            if (destinationContainer != null
                && destinationContainer.FindParent<LayoutFloatingWindow>() != null)
                return false;

            var panel = anchorableToShow.Content as PanelVM;
            if (panel == null)
                return false;

            // find the pane with the same ContentId, which is loaded from the layout profile and has an empty content
            // we are going to replace it with the real content (anchorableToShow)
            var placeholder = layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(x => x.ContentId == panel.ContentId);

            // placeholder not found, we will leave this pane to the manager (will be placed in the default location)
            if (placeholder == null)
                return false;

            // this will happen once a hidden pane become visible again, the manager will insert it for us.
            if (placeholder == anchorableToShow)
                return false;

            // sync visiblity
            panel.IsVisible = placeholder.IsVisible;

            // PreviousContainer and PreviousContainerIndex must be synchronized to anchorableToShow, otherwise if it is hidden, it 
            // cannot be recovered (to its PreviousContainer)
            var previousContainerProperty = placeholder.GetType()
                                                       .GetProperty("PreviousContainer", BindingFlags.NonPublic | BindingFlags.Instance);
            var previousContainer = previousContainerProperty.GetValue(placeholder, null);
            previousContainerProperty.SetValue(anchorableToShow, previousContainer, null);

            anchorableToShow.PreviousContainerIndex = placeholder.PreviousContainerIndex;

            // now we could replace the placeholder with anchorableToShow
            var parent = placeholder.Parent;
            parent.ReplaceChild(placeholder, anchorableToShow);

            return true;
        }
        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown) { }
        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }
        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown) { }
    }
}
