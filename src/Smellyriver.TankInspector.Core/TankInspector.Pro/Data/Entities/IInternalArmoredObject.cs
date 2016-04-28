using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    internal interface IInternalArmoredObject
    {
        Dictionary<string, ArmorGroup> ArmorGroups { get; }
    }
}
