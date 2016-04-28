using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    public interface IStatsProvider
    {
        IEnumerable<IStat> Stats { get; }
    }
}
