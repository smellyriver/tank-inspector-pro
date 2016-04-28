using System.Collections.Generic;


namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    public class StatsProviderManager
    {
        public static StatsProviderManager Instance { get; private set; }

        static StatsProviderManager()
        {
            StatsProviderManager.Instance = new StatsProviderManager();
        }


        private readonly Dictionary<string, IStat> _stats;

        public IEnumerable<IStat> Stats
        {
            get { return _stats.Values; }
        }

        private StatsProviderManager()
        {
            _stats = new Dictionary<string, IStat>();
            this.Register(new XPathStatsProvider());
        }

        public void Register(IStatsProvider provider)
        {
            foreach (var stat in provider.Stats)
            {
                if (stat == null || string.IsNullOrEmpty(stat.Key))
                {
                    Core.Support.LogError(this, "trying to register an invalid stat (null or empty key)");
                    continue;
                }

                IStat existedStat;
                if (_stats.TryGetValue(stat.Key, out existedStat))
                    Core.Support.LogError(this,
                                                              string.Format(
                                                                            "a stat with the key '{0}' (name='{1}') is already existed. it will be replaced with the new one (name='{2}')",
                                                                            stat.Key,
                                                                            stat.Name,
                                                                            existedStat.Name));

                _stats[stat.Key] = stat;
            }
        }


        public IStat GetStat(string key)
        {
            IStat stat;
            if (_stats.TryGetValue(key, out stat))
                return stat;

            return null;
        }


    }
}
