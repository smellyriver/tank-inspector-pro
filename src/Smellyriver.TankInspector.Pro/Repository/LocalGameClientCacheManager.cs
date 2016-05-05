using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using Smellyriver.TankInspector.Pro.IO;

namespace Smellyriver.TankInspector.Pro.Repository
{
    
    public class LocalGameClientCacheManager
    {
        [DataContract]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        struct CacheVersionStamp
        {
            public static readonly DataContractSerializer Serializer
                = new DataContractSerializer(typeof(CacheVersionStamp));

            [DataMember]
            public GameVersion GameVersion;

            [DataMember]
            public int CacheFormatVersion;

            public CacheVersionStamp(int cacheFormatVersion, GameVersion gameVersion)
                : this()
            {
                this.CacheFormatVersion = cacheFormatVersion;
                this.GameVersion = gameVersion;
            }
        }


        public const int CurrentCacheFormatVersion = 1100;

        private readonly LocalGameClient _client;

        public bool IsCacheExpired { get; }

        internal LocalGameClientCacheManager(LocalGameClient client)
        {
            _client = client;
            this.IsCacheExpired = this.GetIsCacheExpired();
        }

        public bool GetIsCacheExpired()
        {
            var cacheVersionstampFile = ApplicationPath.GetRepositoryDataFile(_client, "cache.versionstamp");
            if (!File.Exists(cacheVersionstampFile))
                return true;

            try
            {
                using (var file = File.OpenRead(cacheVersionstampFile))
                {
                    var stamp = (CacheVersionStamp)CacheVersionStamp.Serializer.ReadObject(file);
                    if (stamp.GameVersion != _client.Version || stamp.CacheFormatVersion < CurrentCacheFormatVersion)
                    {
                        this.LogInfo("cache version stamp expired");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogError("failed to read cache versionstamp: {0}", ex.Message);
                return true;
            }

            return false;
        }

        internal void UpdateCacheVersionstamp()
        {

            var cacheVersionstampFile = ApplicationPath.GetRepositoryDataFile(_client, "cache.versionstamp");

            try
            {
                using (var file = File.Create(cacheVersionstampFile))
                {
                    CacheVersionStamp.Serializer.WriteObject(file, new CacheVersionStamp(CurrentCacheFormatVersion, _client.Version));
                }
            }
            catch (Exception ex)
            {
                this.LogError("failed to write xml cache versionstamp: {0}", ex.Message);
            }

        }

        public T Load<T>(string cacheFilename, Func<T> buildAction, Func<Stream, T> loadAction, Action<Stream, T> saveAction)
        {
            var cacheFile = ApplicationPath.GetRepositoryDataFile(_client, cacheFilename);

            if (!this.IsCacheExpired)
            {
                if (File.Exists(cacheFile))
                {
                    try
                    {
                        using (var file = File.OpenRead(cacheFile))
                        {
                            return loadAction(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.LogError("failed to load cache file '{0}': {1}", cacheFilename, ex.Message);
                    }
                }
            }
            else
            {
                this.LogInfo("cache of '{0}' is expired, gonna rebuild", cacheFilename);
            }

            var result = buildAction();

            try
            {
                using (var file = File.Create(cacheFile))
                {
                    saveAction(file, result);
                }
            }
            catch (Exception ex)
            {
                this.LogError("failed to save cache for '{0}': {1}", cacheFilename, ex.Message);
            }

            return result;
        }

        public T LoadBinarySerialized<T>(string cacheFilename, Func<T> buildAction)
        {
            var serializer = new BinaryFormatter();

            return this.Load(cacheFilename, 
                             buildAction, 
                             s => (T)serializer.Deserialize(s), 
                             (s, o) => serializer.Serialize(s, o));
        }

        public T LoadDataContractSerialized<T>(string cacheFilename, Func<T> buildAction)
        {
            var serializer = new DataContractSerializer(typeof(T));

            return this.Load(cacheFilename,
                             buildAction,
                             s => (T)serializer.ReadObject(s),
                             (s, o) => serializer.WriteObject(s, o));
        }

        private T LoadDataContractSerialized<T>(string cacheFilename)
        {
            var cacheFile = ApplicationPath.GetRepositoryDataFile(_client, cacheFilename);
            if (File.Exists(cacheFile))
            {
                try
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    using (var file = File.OpenRead(cacheFile))
                    {
                        return (T)serializer.ReadObject(file);
                    }
                }
                catch (Exception ex)
                {
                    this.LogError("failed to load cache file '{0}': {1}", cacheFilename, ex.Message);
                    return default(T);
                }
            }

            return default(T);
        }

        public XElement LoadXml(string cacheFilename, Func<XElement> buildAction)
        {
            var cacheFile = ApplicationPath.GetRepositoryDataFile(_client, cacheFilename);
            if (!this.IsCacheExpired)
            {
                if (File.Exists(cacheFile))
                {
                    try
                    {
                        return XElement.Load(cacheFile);
                    }
                    catch (Exception ex)
                    {
                        this.LogError("failed to load xml cache file '{0}': {1}", cacheFilename, ex.Message);
                    }
                }

            }
            else
            {
                this.LogInfo("xml cache of '{0}' is expired, gonna rebuild", cacheFilename);
            }

            var element = buildAction();

            try
            {
                element.Save(cacheFile);
            }
            catch (Exception ex)

            {
                this.LogError("failed to save xml cache for '{0}': {1}", cacheFilename, ex.Message);
            }

            return element;
        }


    }
}
