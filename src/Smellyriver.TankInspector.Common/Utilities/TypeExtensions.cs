using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface(this Type type, Type interfaceType, bool includeSelf = true)
        {
            bool result;
            if (interfaceType.IsGenericTypeDefinition)
            {
                result = type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
                if (!result && includeSelf)
                    result = type.IsGenericType && type.GetGenericTypeDefinition() == interfaceType;
            }
            else
            {
                result = type.GetInterfaces().Contains(interfaceType);
                if (!result && includeSelf)
                    result = type == interfaceType;
            }

            return result;
   
        }

        public static bool ImplementsGenericInterface(this Type type, Type interfaceTypeDefinition, out Type[] genericArguments, bool includeSelf = true)
        {
            if (interfaceTypeDefinition.IsGenericTypeDefinition)
            {
                var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceTypeDefinition);

                if (interfaceType == null && includeSelf)
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == interfaceTypeDefinition)
                        interfaceType = type;
                }

                if (interfaceType == null)
                {
                    genericArguments = null;
                    return false;
                }
                else
                {
                    genericArguments = interfaceType.GetGenericArguments();
                    return true;
                }
            }
            else
                throw new ArgumentException("interfaceTypeDefinition must be a generic interface type definition", "interfaceTypeDefinition");
        }

        public static Type[] GetInterfaceGenericArguments(this Type type, Type interfaceTypeDefinition, bool includeSelf = true)
        {
            if (interfaceTypeDefinition.IsGenericTypeDefinition)
            {
                var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceTypeDefinition);

                if (interfaceType == null && includeSelf)
                    interfaceType = type.IsGenericType && type.GetGenericTypeDefinition() == interfaceTypeDefinition ? type : null;

                if (interfaceType == null)
                    return null;

                return interfaceType.GetGenericArguments();
            }
            else
                throw new ArgumentException("interfaceTypeDefinition must be a generic interface type definition", "interfaceTypeDefinition");
        }

        public static bool ImplementsGenericInterface(this Type type, Type interfaceTypeDefinition, bool includeSelf = true)
        {
            if (interfaceTypeDefinition.IsGenericTypeDefinition)
            {
                var result = type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceTypeDefinition);
                if (!result && includeSelf)
                    result = type.IsGenericType && type.GetGenericTypeDefinition() == interfaceTypeDefinition;

                return result;
            }
            else
                throw new ArgumentException("interfaceTypeDefinition must be a generic interface type definition", "interfaceTypeDefinition");
        }

        public static bool Inherits(this Type type, Type baseType, bool includeSelf = true)
        {
            if (type == null)
                return false;

            Type t;
            if (includeSelf)
                t = type;
            else
                t = type.BaseType;

            while (t != null)
            {
                if (t == baseType)
                    return true;

                t = t.BaseType;
            }
            return false;
        }

        public static bool InheritsOrImplements(this Type type, Type baseType, bool includeSelf = true)
        {
            if (baseType.IsInterface)
                return type.ImplementsInterface(baseType, includeSelf);
            else
                return type.Inherits(baseType, includeSelf);
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;

            // the following method is way too complex (and slow) and not compilable under AOT.

            //Expression<Func<object>> e = Expression.Lambda<Func<object>>(
            //    Expression.Convert(
            //        Expression.Default(type), typeof(object)
            //    )
            //);

            //return e.Compile()();
        }

        private static IEnumerable<PropertyInfo> GetAllPropertiesBase(this Type type, BindingFlags bindingAttr)
        {
            var currentType = type;
            do
            {
                foreach (var property in currentType.GetProperties(bindingAttr))
                    yield return property;
            }
            while ((currentType = currentType.BaseType) != null);
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type, BindingFlags bindingAttr)
        {
            return type.GetProperties(bindingAttr | BindingFlags.FlattenHierarchy);
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type, BindingFlags bindingAttr)
        {
            var currentType = type;
            do
            {
                foreach (var field in currentType.GetFields(bindingAttr))
                    yield return field;
            }
            while ((currentType = currentType.BaseType) != null);
        }

        public static bool TryGetPropertyValue(this Type type, object target, string propertyName, out object value, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance, object[] index = null)
        {
            var property = type.GetProperty(propertyName, bindingAttr);
            if (property != null)
            {
                value = property.GetValue(target, index);
                return true;
            }

            value = null;
            return false;
        }

        public static object GetPropertyValue(this Type type, object target, string propertyName, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance, object[] index = null)
        {
            object result;
            if (TryGetPropertyValue(type, target, propertyName, out result, bindingAttr, index))
                return result;
            else
                throw new ArgumentException("specified property does not exist", "propertyName");
        }

        public static bool TryGetFieldValue(this Type type, object target, string fieldName, out object value, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            var field = type.GetField(fieldName, bindingAttr);
            if (field != null)
            {
                value = field.GetValue(target);
                return true;
            }

            value = null;
            return false;
        }

        public static object GetFieldValue(this Type type, object target, string fieldName, BindingFlags bindingAttr = BindingFlags.Public)
        {
            object result;
            if (TryGetFieldValue(type, target, fieldName, out result, bindingAttr))
                return result;
            else
                throw new ArgumentException("specified field does not exist", "fieldName");
        }

        public static bool TrySetFieldValue(this Type type, object target, string fieldName, object value, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            var field = type.GetField(fieldName, bindingAttr);
            if (field != null)
            {
                field.SetValue(target, value);
                return true;
            }
            return false;
        }

        public static void SetFieldValue(this Type type, object target, string fieldName, object value, BindingFlags bindingAttr = BindingFlags.Public)
        {
            if (!TrySetFieldValue(type, target, fieldName, value, bindingAttr))
                throw new ArgumentException("specified field does not exist", "fieldName");
        }

    }
}
