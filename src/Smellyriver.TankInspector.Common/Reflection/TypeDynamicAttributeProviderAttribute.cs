using System;

namespace Smellyriver.TankInspector.Common.Reflection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TypeDynamicAttributeProviderAttribute : Attribute
    {
        public Type TargetType { get; private set; }

        public TypeDynamicAttributeProviderAttribute(Type targetType)
        {
            this.TargetType = targetType;
        }
    }
}
