using System;
using System.Reflection;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class ObjectExtensions
    {
        public static string GetName(this object value, bool useTypeNameAsDefault = true)
        {
            if (value == null)
                return "null";

            var type = value.GetType();
            while (type != null)
            {
                var nameField = type.GetField("Name");
                if (nameField != null && nameField.FieldType == typeof(string))
                    return nameField.GetValue(value) as string;
                else
                {
                    var nameProperty = type.GetProperty("Name");
                    if (nameProperty != null)
                    {
                        var getMethod = nameProperty.GetGetMethod();
                        if (getMethod != null && getMethod.ReturnType == typeof(string))
                        {
                            return getMethod.Invoke(value, null) as string;
                        }
                    }
                }

                type = type.BaseType;
            }

            if (useTypeNameAsDefault)
                return value.GetType().Name;
            else
                return null;
        }

        public static bool TryGetPropertyValue(this object @this, string propertyName, out object value, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance, object[] index = null)
        {
            if (@this == null)
            {
                value = null;
                return false;
            }

            return @this.GetType().TryGetPropertyValue(@this, propertyName, out value, bindingAttr, index);
        }

        public static object GetPropertyValue(this object @this, string propertyName, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance, object[] index = null)
        {
            return @this.GetType().GetPropertyValue(@this, propertyName, bindingAttr, index);
        }

        public static bool TryGetFieldValue(this object @this, string fieldName, out object value, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            if (@this == null)
            {
                value = null;
                return false;
            }

            return @this.GetType().TryGetFieldValue(@this, fieldName, out value, bindingAttr);
        }

        public static object GetFieldValue(this object @this, string fieldName, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            return @this.GetType().GetFieldValue(@this, fieldName, bindingAttr);
        }

        public static bool TrySetFieldValue(this object @this, string fieldName, object value, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            if (@this == null)
            {
                value = null;
                return false;
            }

            return @this.GetType().TrySetFieldValue(@this, fieldName, value, bindingAttr);
        }

        public static void SetFieldValue(this object @this, string fieldName, object value, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            @this.GetType().SetFieldValue(@this, fieldName, value, bindingAttr);
        }


        public static object ValueOrDefault(this object @this, Type targetType)
        {
            return @this == null ? targetType.GetDefaultValue() : @this;
        }

        public static object ValueOrDefault(this object @this, object defaultValue)
        {
            return @this == null ? defaultValue : @this;
        }

        public static object ValueOrDefault<T>(this T @this)
        {
            return @this == null ? default(T) : @this;
        }
    }
}
