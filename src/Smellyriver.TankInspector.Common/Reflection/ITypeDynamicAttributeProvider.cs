using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Common.Reflection
{
    public interface ITypeDynamicAttributeProvider
    {
        IEnumerable<Attribute> GetDynamicAttributes(string memberName);
    }
}
