using System.Windows;
using System.Windows.Controls;
using Smellyriver.TankInspector.Pro.UserInterface.ViewModels;

namespace Smellyriver.TankInspector.Pro.UserInterface
{
    class PanesStyleSelector : StyleSelector
    {
        public Style PanelStyle { get; set; }

        public Style DocumentStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is PanelVM)
                return PanelStyle;

            if (item is DocumentVM)
                return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}
