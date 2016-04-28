using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class BasicConfigurationChangedEventArgs : EventArgs
    {
        public BasicConfigurationAspect Aspect { get; private set; }

        public BasicConfigurationChangedEventArgs(BasicConfigurationAspect aspect)
        {
            this.Aspect = aspect;
        }
    }
}
