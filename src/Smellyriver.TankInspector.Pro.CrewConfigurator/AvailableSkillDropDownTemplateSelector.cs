using System.Windows;
using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.CrewConfigurator
{
    class AvailableSkillDropDownTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmptyItemTemplate { get; set; }
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate SelectedEmptyItemTemplate { get; set; }
        public DataTemplate SelectedItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ContentPresenter presenter = (ContentPresenter)container;

            presenter.IsEnabled = item != EmptySkillVM.Instance;

            if (presenter.TemplatedParent is ComboBox)
            {
                return item == EmptySkillVM.Instance ? this.SelectedEmptyItemTemplate : this.SelectedItemTemplate;
            }
            else // Templated parent is ComboBoxItem
            {
                return item == EmptySkillVM.Instance ? this.EmptyItemTemplate : this.ItemTemplate;
            }
        }
    }
}