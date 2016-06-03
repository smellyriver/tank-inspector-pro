using System;
using Smellyriver.TankInspector.Pro.Data.Entities;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class ConsumableChangedEventArgs : TankConfigurationItemChangedEventArgs
    {
        public int SlotIndex { get; private set; }

        public ConsumableChangedEventArgs(int slotIndex, Consumable oldValue, Consumable newValue) : base(oldValue, newValue)
        {
            this.SlotIndex = slotIndex;
        }
    }
}
