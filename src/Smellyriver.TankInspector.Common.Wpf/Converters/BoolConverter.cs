using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smellyriver.TankInspector.Common.Wpf.Converters
{
    public class BoolConverter : MarkupExtension, IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var boolValue = value is bool ? (bool)value : value != null;
            return boolValue ? this.TrueValue : this.FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
