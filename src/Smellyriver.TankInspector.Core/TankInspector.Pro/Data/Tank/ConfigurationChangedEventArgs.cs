using System;
using System.Xml.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class ConfigurationChangedEventArgs : EventArgs
    {
        public ConfigurationAspect Aspect { get; private set; }
        public IXQueryable OldValue { get; private set; }
        public IXQueryable NewValue { get; private set; }

        public ConfigurationChangedEventArgs(ConfigurationAspect aspect, IXQueryable oldValue, IXQueryable newValue)
        {
            this.Aspect = aspect;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}
