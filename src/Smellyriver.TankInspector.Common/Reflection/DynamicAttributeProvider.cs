using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Common.Reflection
{
    public class DynamicAttributeProvider
    {
        public static DynamicAttributeProvider Instance { get; private set; }

        static DynamicAttributeProvider()
        {
            DynamicAttributeProvider.Instance = new DynamicAttributeProvider();
        }

        private Dictionary<Type, ITypeDynamicAttributeProvider> _providers;
        private Dictionary<Type, ITypeDynamicAttributeProvider> _genericProviders;

        public void RegisterDynamicAttributeProvider(Type type, ITypeDynamicAttributeProvider provider)
        {
            if (type.IsGenericTypeDefinition)
                _genericProviders.Add(type, provider);
            else
                _providers.Add(type, provider);
        }

        private void InitializeTypesWithTypeDynamicAttributeProviderAttribute()
        {
            List<Type> types = new List<Type>();

            foreach (Type type in TypeHelper.GetAllLoadedTypes())
            {
                var attributes = type.GetCustomAttributes(typeof(TypeDynamicAttributeProviderAttribute), false);

                if (attributes.Length > 0)
                {
                    ITypeDynamicAttributeProvider provider = Activator.CreateInstance(type) as ITypeDynamicAttributeProvider;
                    foreach (var attribute in attributes)
                    {
                        var providerAttribute = attribute as TypeDynamicAttributeProviderAttribute;
                        this.RegisterDynamicAttributeProvider(providerAttribute.TargetType, provider);
                    }
                }
            }
        }

        private DynamicAttributeProvider()
        {
            _providers = new Dictionary<Type, ITypeDynamicAttributeProvider>();
            _genericProviders = new Dictionary<Type, ITypeDynamicAttributeProvider>();
            this.InitializeTypesWithTypeDynamicAttributeProviderAttribute();
        }

        private bool TryGetProvider(Type type, out ITypeDynamicAttributeProvider provider)
        {
            provider = null;
            if (!_providers.TryGetValue(type, out provider))
                if (type.IsGenericType)
                    _genericProviders.TryGetValue(type.GetGenericTypeDefinition(), out provider);

            return provider != null;
        }

        public IEnumerable<Attribute> GetDynamicAttributes(MemberInfo member)
        {
            Type type = null;
            string memberName;
            ITypeDynamicAttributeProvider provider;
            if (member is Type && this.TryGetProvider((Type)member, out provider))
            {
                type = member as Type;
                memberName = null;
            }
            else if (member.DeclaringType != null && this.TryGetProvider(member.DeclaringType, out provider))
            {
                type = member.DeclaringType;
                memberName = member.Name;
            }
            else
                return new Attribute[] { };

            var attributes = provider.GetDynamicAttributes(memberName);
            if (attributes == null)
                return new Attribute[] { };
            else
                return attributes;
            
        }


        public IEnumerable<Attribute> GetDynamicAttributes(PropertyDescriptor property)
        {
            ITypeDynamicAttributeProvider provider;
            if (property.ComponentType != null && this.TryGetProvider(property.ComponentType, out provider))
            {
                var attributes = provider.GetDynamicAttributes(property.Name);
                if (attributes == null)
                    return new Attribute[] { };
                else
                    return attributes;
            }
            else
            {
                return new Attribute[] { };
            }
        }
    }
}
