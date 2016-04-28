using System;

namespace Smellyriver.TankInspector.Common.Serialization
{
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class DeserializationOnlyAttribute : Attribute
    {
        public DeserializationOnlyAttribute()
        {

        }
    }
}
