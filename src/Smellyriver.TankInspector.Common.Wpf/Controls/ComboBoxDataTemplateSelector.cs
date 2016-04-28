using System.Windows;
using System.Windows.Controls;

namespace Smellyriver.TankInspector.Common.Wpf.Controls
{
    public class ComboBoxDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate SelectedItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ContentPresenter presenter = (ContentPresenter)container;

            if (presenter.TemplatedParent is ComboBox)
            {
                return this.SelectedItemTemplate;
            }
            else // Templated parent is ComboBoxItem
            {
                return this.ItemTemplate;
            }
        }
    }
}
