using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Networking.Livestat;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.LivestatProvider
{
    class LivestatDatabase
    {
        private const string c_livestatCacheFile = @"livestats.xml";

        
        private static readonly DataContractSerializer s_serializer;

        public static TimeSpan ExpireTime = TimeSpan.FromDays(1.0);

        public static LivestatDatabase Instance { get; private set; }

        static LivestatDatabase()
        {
            s_serializer = new DataContractSerializer(typeof(LivestatItemCache[]), "Livestats", Stat.Xmlns);
            LivestatDatabase.Instance = new LivestatDatabase();
        }

        private static uint GetRepositoryVersion(IRepository client)
        {
            return (uint)(client.Version.Major * 10000 + client.Version.Minor * 100 + client.Version.Build);
        }

        private readonly object _repositoryLiveStatQueriesLock = new object();
        private readonly object _livestatQueryLock = new object();

        private Dictionary<IRepository, Dictionary<ulong, LivestatQuery>> _livestatQueries;

        public LivestatDatabase()
        {
            _livestatQueries = new Dictionary<IRepository, Dictionary<ulong, LivestatQuery>>();
            Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            this.SaveCache();
        }

        private LivestatItemCache[] LoadCache(IRepository repository)
        {
            try
            {
                var cacheFile = ApplicationPath.GetRepositoryDataFile(repository, c_livestatCacheFile);

                if (File.Exists(cacheFile))
                {
                    using (var file = File.OpenRead(c_livestatCacheFile))
                    {
                        return ((LivestatItemCache[])s_serializer.ReadObject(file));
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogError("failed to load livestat cache for repository '{0}': {1}", repository.ID, ex.Message);
            }

            return new LivestatItemCache[0];
        }

        private void SaveCache()
        {
            foreach (var repository in _livestatQueries.Keys)
                this.SaveCache(repository);
        }

        private void SaveCache(IRepository repository)
        {
            try
            {
                var cacheFile = ApplicationPath.GetRepositoryDataFile(repository, c_livestatCacheFile);

                using (var file = File.Create(cacheFile))
                {
                    s_serializer.WriteObject(file, _livestatQueries[repository].Values
                                                                               .Select(q => q.Cache)
                                                                               .Where(c => c != null)
                                                                               .ToArray());
                }
            }
            catch (Exception ex)
            {
                this.LogError("failed to save livestat cache for repository '{0}': {1}", repository.ID, ex.Message);
            }
        }


        public LivestatData this[IXQueryable tank, IRepository repository]
        {
            get { return this.GetLiveStat(TypeCompDescr.Calculate(tank), repository); }
        }

        private LivestatData GetLiveStat(uint compDescr, IRepository repository)
        {
            var repositoryVersion = LivestatDatabase.GetRepositoryVersion(repository);
            var key = LivestatQuery.GetLivestatKey(compDescr, repositoryVersion);

            Dictionary<ulong, LivestatQuery> repositoryLiveStatQueries;
            if (!_livestatQueries.TryGetValue(repository, out repositoryLiveStatQueries))
            {
                lock (_repositoryLiveStatQueriesLock)
                {
                    if (!_livestatQueries.TryGetValue(repository, out repositoryLiveStatQueries))
                    {
                        var caches = this.LoadCache(repository);
                        repositoryLiveStatQueries = caches.ToDictionary(c => c.Key,
                                                                        c => new LivestatQuery(compDescr, repositoryVersion, c));
                        _livestatQueries[repository] = repositoryLiveStatQueries;
                    }
                }
            }

            LivestatQuery query;
            if (!repositoryLiveStatQueries.TryGetValue(key, out query))
            {
                lock (_livestatQueryLock)
                {
                    if (!repositoryLiveStatQueries.TryGetValue(key, out query))
                    {
                        query = new LivestatQuery(compDescr, repositoryVersion);
                        repositoryLiveStatQueries[key] = query;
                    }
                }
            }

            var cache = query.Query();
            return cache == null ? null : cache.Livestats;
        }
    }
}
