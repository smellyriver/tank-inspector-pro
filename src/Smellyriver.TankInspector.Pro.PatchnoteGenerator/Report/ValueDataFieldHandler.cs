using System.Globalization;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Stats;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    class ValueDataFieldHandler : TankDataFieldHandler
    {
        

        public readonly static ValueDataFieldHandler Instance = new ValueDataFieldHandler();

        private string FormatValue(string value, ValueDataField field)
        {
            string formattedValue;
            if (!string.IsNullOrEmpty(field.FormatString))
            {
                double doubleValue;
                if (double.TryParse(value, out doubleValue))
                    formattedValue = string.Format(field.FormatString, doubleValue);
                else
                    formattedValue = string.Format(field.FormatString, value);
            }
            else
                formattedValue = value;
            
            formattedValue = string.IsNullOrEmpty(field.Unit) ? formattedValue : string.Format("{0} {1}", formattedValue, field.Unit);
            return formattedValue;
        }

        public override PatchnoteReportItem[] CreateReportItems(string name, TankDataFieldBase field, IXQueryable oldItem, IXQueryable newItem)
        {
            var oldValue = oldItem.QueryValue(field.XPath);
            var newValue = newItem.QueryValue(field.XPath);

            var valueField = (ValueDataField)field;

            if (string.Equals(oldValue, newValue))
                return null;

            var oldValueString = this.FormatValue(oldValue, valueField);
            var newValueString = this.FormatValue(newValue, valueField);

            if (valueField.CompareStrategy == CompareStrategy.NotComparable)
            {
                return new[] { new ChangedItem(field.Name, ChangeVerb.Changed, oldValueString, newValueString, oldValue) };
            }
            else
            {
                double oldDoubleValue, newDoubleValue;
                if (!double.TryParse(oldValue,
                                     NumberStyles.Float | NumberStyles.AllowThousands,
                                     CultureInfo.InvariantCulture,
                                     out oldDoubleValue) 
                    || 
                    !double.TryParse(newValue,
                                     NumberStyles.Float | NumberStyles.AllowThousands,
                                     CultureInfo.InvariantCulture, 
                                     out newDoubleValue))
                {
                    this.LogError("uncomparable field detected: field.Name = '{0}', oldValue = '{1}', newValue = '{2}'",
                                    field.Name, oldValue, newValue);
                    return new[] { new ChangedItem(field.Name, ChangeVerb.Changed, oldValueString, newValueString, oldValue) };
                }

                ChangeVerb verb;
                if (oldDoubleValue > newDoubleValue)
                {
                    if (valueField.CompareStrategy == CompareStrategy.Plain)
                        verb = ChangeVerb.Decreased;
                    else if (valueField.CompareStrategy == CompareStrategy.HigherBetter)
                        verb = ChangeVerb.Nerfed;
                    else
                        verb = ChangeVerb.Buffed;
                }
                else
                {
                    if (valueField.CompareStrategy == CompareStrategy.Plain)
                        verb = ChangeVerb.Increased;
                    else if (valueField.CompareStrategy == CompareStrategy.HigherBetter)
                        verb = ChangeVerb.Buffed;
                    else
                        verb = ChangeVerb.Nerfed;
                }

                return new[] { new ChangedItem(field.Name, verb, oldValueString, newValueString, oldValue) };
            }

        }
    }
}
