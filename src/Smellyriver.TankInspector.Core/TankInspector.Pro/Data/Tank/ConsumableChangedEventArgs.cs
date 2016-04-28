using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class ConsumableChangedEventArgs : EventArgs
    {
        public int SlotIndex { get; private set; }

        public ConsumableChangedEventArgs(int slotIndex)
        {
            this.SlotIndex = slotIndex;
        }
    }
}
