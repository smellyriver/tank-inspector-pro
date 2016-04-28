using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Common.Wpf.Converters
{
    public class ColorToBrushConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || (value is string && string.IsNullOrEmpty((string)value)))
                return null;
            if (!(value is Color))
                throw new NotSupportedException();
            var color = (Color)value;
            return new SolidColorBrush(color);
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
