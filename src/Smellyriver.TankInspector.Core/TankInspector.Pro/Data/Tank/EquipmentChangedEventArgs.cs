using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class EquipmentChangedEventArgs : EventArgs
    {
        public int SlotIndex { get; private set; }

        public EquipmentChangedEventArgs(int slotIndex)
        {
            this.SlotIndex = slotIndex;
        }
    }
}
