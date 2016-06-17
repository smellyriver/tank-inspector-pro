using System;
using Smellyriver.TankInspector.Pro.Data.Entities;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class EquipmentChangedEventArgs : ComponentChangedEventArgs
    {
        public int SlotIndex { get; private set; }

        public EquipmentChangedEventArgs(int slotIndex, Equipment oldValue, Equipment newValue) : base(oldValue, newValue)
        {
            this.SlotIndex = slotIndex;
        }
    }
}
