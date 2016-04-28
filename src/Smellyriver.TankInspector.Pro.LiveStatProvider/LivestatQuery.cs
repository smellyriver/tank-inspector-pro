using System;
using Smellyriver.TankInspector.Pro.Networking;

namespace Smellyriver.TankInspector.Pro.LivestatProvider
{
    class LivestatQuery
    {
        

        public static ulong GetLivestatKey(uint compDescr, uint repositoryVersion)
        {
            return ((ulong)repositoryVersion << 32) + (ulong)compDescr;
        }

        public uint CompDescr { get; }
        public uint RepositoryVersion { get; }
        public ulong Key { get; }

        public LivestatItemCache Cache { get; private set; }

        private readonly object _queryLock = new object();
        private bool _queryComplete;

        public LivestatQuery(uint compDescr, uint repositoryVersion, LivestatItemCache cache = null)
        {
            this.CompDescr = compDescr;
            this.RepositoryVersion = repositoryVersion;
            this.Key = LivestatQuery.GetLivestatKey(compDescr, repositoryVersion);

            if (cache != null && cache.CachedTime - DateTime.Now < LivestatDatabase.ExpireTime)
                this.Cache = cache;
            else
                this.Cache = null;
        }

        public LivestatItemCache Query()
        {
            lock (_queryLock)
            {
                if (_queryComplete)
                    return this.Cache;

                try
                {
                    _queryComplete = false;
                    
                    var livestat = TankInspectorRemoteService.Instance.GetLivestat(this.CompDescr, this.RepositoryVersion, true);

                    this.Cache = new LivestatItemCache
                    {
                        Key = this.Key,
                        CachedTime = DateTime.Now,
                        Livestats = livestat
                    };

                    return this.Cache;
                }
                catch (Exception ex)
                {
                    this.LogWarning("failed to query livestat: compDescr='{0}', clientVersion='{1}': {2}", 
                                   this.CompDescr, 
                                   this.RepositoryVersion, 
                                   ex.Message);
                    return null;
                }
                finally
                {
                    _queryComplete = true;
                }
            }
        }
    }
}
