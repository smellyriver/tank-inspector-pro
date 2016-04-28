using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Smellyriver.TankInspector.Common.Reflection;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class MemberInfoExtensions
    {

        public static bool CheckMutualAttributes<TTrueAttribute, TFalseAttribute>(
            this MemberInfo member, bool defaultValue, out TTrueAttribute trueAttribute, out TFalseAttribute falseAttribute)
            where TTrueAttribute : Attribute
            where TFalseAttribute : Attribute
        {
            falseAttribute = MemberInfoExtensions.GetDistinctAttribute<TFalseAttribute>(member, false);
            if (falseAttribute == null)
            {

                trueAttribute = MemberInfoExtensions.GetDistinctAttribute<TTrueAttribute>(member, true);
                if (trueAttribute != null)
                {
                    return true;
                }
                else
                {
                    trueAttribute = null;
                    return defaultValue;
                }
            }
            else
            {
                trueAttribute = null;
                return false;
            }
        }

        public static bool CheckMutualAttributes<TTrueAttribute, TFalseAttribute>(
            this MemberInfo member, bool defaultValue)
            where TTrueAttribute : Attribute
            where TFalseAttribute : Attribute
        {
            TTrueAttribute trueAttr;
            TFalseAttribute falseAttr;
            return MemberInfoExtensions.CheckMutualAttributes(member, defaultValue, out trueAttr, out falseAttr);
        }

        public static bool CheckMutualAttributes<TTrueAttribute, TFalseAttribute>(
            this MemberInfo member, out TTrueAttribute trueAttribute, out TFalseAttribute falseAttribute)
            where TTrueAttribute : Attribute
            where TFalseAttribute : Attribute
        {
            return MemberInfoExtensions.CheckMutualAttributes(member, false, out trueAttribute, out falseAttribute);
        }

        public static bool CheckMutualAttributes<TTrueAttribute, TFalseAttribute>(this MemberInfo member)
            where TTrueAttribute : Attribute
            where TFalseAttribute : Attribute
        {
            return MemberInfoExtensions.CheckMutualAttributes<TTrueAttribute, TFalseAttribute>(member, false);
        }

        public static TAttribute GetDistinctAttribute<TAttribute>(this MemberInfo member, bool inherit)
             where TAttribute : Attribute
        {

            var attribute = DynamicAttributeProvider.Instance.GetDynamicAttributes(member).OfType<TAttribute>().FirstOrDefault();
            if (attribute != null)
                return attribute;

            return ((ICustomAttributeProvider)member).GetDistinctAttribute<TAttribute>(inherit);
        }

        public static TAttribute GetDistinctAttribute<TAttribute>(this ICustomAttributeProvider target, bool inherit)
             where TAttribute : Attribute
        {
            var attributes = target.GetCustomAttributes(typeof(TAttribute), inherit);

            return attributes == null ? null : (TAttribute)attributes.FirstOrDefault();
        }

        public static string GetDisplayName(this MemberInfo member)
        {
            var attr = MemberInfoExtensions.GetDistinctAttribute<DisplayNameAttribute>(member, true);
            if (attr != null)
                return attr.DisplayName;
            else
                return member.Name;
        }

        public static IEnumerable<Attribute> GetAttributes(this MemberInfo member, bool inherit)
        {
            List<Attribute> attributes = new List<Attribute>();

            attributes.AddRange(DynamicAttributeProvider.Instance.GetDynamicAttributes(member));
            attributes.AddRange(member.GetCustomAttributes(inherit).Cast<Attribute>());
            return attributes;
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo member, bool inherit)
            where TAttribute : Attribute
        {
            return MemberInfoExtensions.GetAttributes(member, inherit).OfType<TAttribute>();
        }

    }
}
