using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smellyriver.TankInspector.Pro.Data.Entities;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class ComponentChangedEventArgs : EventArgs
    {
        public Component OldValue { get; private set; }
        public Component NewValue { get; private set; }

        public ComponentChangedEventArgs(Component oldValue, Component newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}
