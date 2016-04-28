using System;
using System.IO;
using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.IO;

namespace Smellyriver.TankInspector.Pro.Repository
{
    [DataContract]
    class LocalGameClientConfiguration
    {
        

        public static readonly DataContractSerializer Serializer;
        private const string c_configFile = "client.config";

        static LocalGameClientConfiguration()
        {
            Serializer = new DataContractSerializer(typeof(LocalGameClientConfiguration));
        }

        private static string GetConfigFile(LocalGameClient client)
        {
            return ApplicationPath.GetRepositoryConfigFile(client, c_configFile);
        }

        public static LocalGameClientConfiguration Load(LocalGameClient client)
        {
            var configFile = LocalGameClientConfiguration.GetConfigFile(client);
            if (File.Exists(configFile))
            {
                try
                {
                    using (var file = File.OpenRead(configFile))
                    {
                        var config = (LocalGameClientConfiguration)Serializer.ReadObject(file);
                        return config;
                    }
                }
                catch (Exception ex)
                {
                    typeof(LocalGameClientConfiguration).LogError("failed to load local game client configuration file '{0}': {1}", configFile, ex.Message);
                }
            }

            return new LocalGameClientConfiguration();
        }

        public static void Save(LocalGameClient client, LocalGameClientConfiguration config)
        {
            var configFile = LocalGameClientConfiguration.GetConfigFile(client);

            try
            {
                using (var file = File.Create(configFile))
                {
                    Serializer.WriteObject(file, config);
                }
            }
            catch (Exception ex)
            {
                typeof(LocalGameClientConfiguration).LogError("failed to save local game client configuration file '{0}': {1}", configFile, ex.Message);
            }
        }

        [DataMember]
        public string ModDirectory { get; set; }
    }
}
