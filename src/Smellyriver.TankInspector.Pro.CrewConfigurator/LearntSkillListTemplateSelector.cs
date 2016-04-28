using System.Windows;
using System.Windows.Controls;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.CrewConfigurator
{
    class LearntSkillListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate SelectedItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var listBoxItem = VisualTreeHelperEx.FindAncestor<ListBoxItem>(container);

            if (listBoxItem.IsSelected)
            {
                return this.SelectedItemTemplate;
            }
            else
            {
                return this.ItemTemplate;
            }
        }
    }
}
