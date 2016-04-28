using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class VisualTreeHelperEx
    {
        public static TAncestor FindAncestor<TAncestor>(DependencyObject descendant)
            where TAncestor : DependencyObject
        {
            while ((descendant = VisualTreeHelper.GetParent(descendant)) != null)
            {
                var ancestor = descendant as TAncestor;
                if (ancestor != null)
                    return ancestor;
            }

            return null;
        }

        public static DependencyObject FindRoot(DependencyObject descendant)
        {
            DependencyObject parent;
            while ((parent = VisualTreeHelper.GetParent(descendant)) != null)
                descendant = parent;

            return descendant;
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static IEnumerable<T> AllChildren<T>(DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null)
                yield break;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                T tChild = child as T;
                if (tChild != null)
                    yield return tChild;

                foreach (var innerChild in AllChildren<T>(child))
                    yield return innerChild;
            }
        }
    }
}
