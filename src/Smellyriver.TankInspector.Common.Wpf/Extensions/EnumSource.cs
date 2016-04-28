using System;
using System.Windows.Markup;

namespace Smellyriver.TankInspector.Common.Wpf.Extensions
{
    public class EnumSource : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumSource(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("an enum type is required", "enumType");

            this.EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(this.EnumType);
        }
    }
}
