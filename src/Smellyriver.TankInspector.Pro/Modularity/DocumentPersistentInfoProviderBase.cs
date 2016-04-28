using System;
using System.IO;
using System.Runtime.Serialization;
using log4net;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    [DataContract]
    public abstract class DocumentPersistentInfoProviderBase : IDocumentPersistentInfoProvider
    {
        protected static T Deserialize<T>(string persistentInfo)
            where T : DocumentPersistentInfoProviderBase
        {
            var serializer = new DataContractSerializer(typeof(T));
            return (T)serializer.ReadObject(persistentInfo.ToStream());
        }

        public static T Load<T>(string persistentInfo, Func<T> factory, ILog log = null)
            where T : DocumentPersistentInfoProviderBase
        {
            if (string.IsNullOrWhiteSpace(persistentInfo))
                return factory();
            else
            {
                try
                {
                    return DocumentPersistentInfoProviderBase.Deserialize<T>(persistentInfo);
                }
                catch (Exception ex)
                {
                    if (log != null)
                        log.LogError("failed to deserialize persistent info: {0}\n   exception message: {1}", persistentInfo, ex.Message);

                    return factory();
                }
            }

        }

        string IDocumentPersistentInfoProvider.PersistentInfo
        {
            get { return this.Serialize(); }
        }

        protected virtual string Serialize()
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(this.GetType());
                serializer.WriteObject(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
    }
}
