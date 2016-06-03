using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class TankConfigurationItemChangedEventArgs : EventArgs
    {
        public IXQueryable OldValue { get; private set; }
        public IXQueryable NewValue { get; private set; }

        public TankConfigurationItemChangedEventArgs(IXQueryable oldValue, IXQueryable newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}
