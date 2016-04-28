using System;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    interface IStatValueGroup
    {
        event EventHandler StatValueLoaded;
        void NotifyStatValueLoaded();
    }
}
