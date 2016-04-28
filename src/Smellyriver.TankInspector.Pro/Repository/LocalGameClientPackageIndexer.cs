using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Smellyriver.TankInspector.IO;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public class LocalGameClientPackageIndexer : IPackageIndexer
    {
        private const string c_packageIndicesCacheFile = "packageIndices.xml";
        private static DataContractSerializer s_cacheSerializer;

        

        static LocalGameClientPackageIndexer()
        {
            s_cacheSerializer = new DataContractSerializer(typeof(PackageIndexEntry[]));
        }

        [DataContract]
        [Serializable]
        struct PackageIndexEntry
        {
            [DataMember]
            public string FilePath;

            [DataMember]
            public string PackagePath;

            public PackageIndexEntry(string filePath, string packagePath)
                : this()
            {
                this.FilePath = filePath;
                this.PackagePath = packagePath;
            }
        }

        private Dictionary<string, string> _packageIndices;

        private readonly LocalGameClient _client;

        internal LocalGameClientPackageIndexer(LocalGameClient client)
        {
            _client = client;
            this.LoadPackageIndices();
        }

        private void LoadPackageIndices()
        {
            var packageIndexEntries = _client.CacheManager.LoadBinarySerialized(c_packageIndicesCacheFile, this.BuildPackageIndices);
            _packageIndices = new Dictionary<string, string>();
            foreach (var entry in packageIndexEntries)
            {
                _packageIndices[entry.FilePath] = entry.PackagePath;
            }
        }

        private PackageIndexEntry[] BuildPackageIndices()
        {
            var entries = new List<PackageIndexEntry>();

            foreach (var packageFile in _client.Paths.ClientPackages)
            {
                this.LogInfo("buiding package indices for '{0}'", packageFile);

                var fileEntries = PackageStream.GetFileEntries(packageFile);
                if (fileEntries == null)
                {
                    this.LogError("non-existed or invalid package file: {0}", packageFile);
                    continue;
                }

                foreach (var filePath in fileEntries)
                    entries.Add(new PackageIndexEntry(filePath.ToLower(), packageFile));
            }

            return entries.ToArray();
        }

        public string GetPackagePath(string filename)
        {
            string packagePath;
            if (_packageIndices.TryGetValue(filename.ToLower(), out packagePath))
                return packagePath;
            else
                return null;
        }

        public string GetUnifiedPath(string filename)
        {
            return UnifiedPath.Combine(this.GetPackagePath(filename), filename);
        }
    }
}
