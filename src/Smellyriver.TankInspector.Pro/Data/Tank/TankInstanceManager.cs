using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Utilities;
using TankEntity = Smellyriver.TankInspector.Pro.Data.Entities.Tank;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public static class TankInstanceManager
    {
        private const string c_tankConfigStorageFile = "tanks.config";

        private static readonly Dictionary<string, TankInstance> s_instances;

        private static readonly Dictionary<IRepository, Dictionary<string, TankInstanceConfigurationInfo>> s_configInfoStorages;

        private static DataContractSerializer s_configInfoSerializer;

        static TankInstanceManager()
        {
            s_instances = new Dictionary<string, TankInstance>();
            s_configInfoStorages = new Dictionary<IRepository, Dictionary<string, TankInstanceConfigurationInfo>>();
            s_configInfoSerializer = new DataContractSerializer(typeof(Dictionary<string, TankInstanceConfigurationInfo>));

            Application.Current.Exit += TankInstanceManager.Application_Exit;
        }

        static void Application_Exit(object sender, ExitEventArgs e)
        {
            foreach (var repositoryConfigInfoStorage in s_configInfoStorages)
            {
                var configFile = ApplicationPath.GetRepositoryConfigFile(repositoryConfigInfoStorage.Key, c_tankConfigStorageFile);

                try
                {
                    using (var file = File.Create(configFile))
                    {
                        s_configInfoSerializer.WriteObject(file, repositoryConfigInfoStorage.Value);
                    }
                }
                catch (Exception ex)
                {
                    typeof(TankInstance).LogError("failed to save tank config info for repository '{0}': {1}", repositoryConfigInfoStorage.Key.ID, ex.Message);
                }
            }
        }

        private static Dictionary<string, TankInstanceConfigurationInfo> LoadRepositoryTankConfigInfoStorage(IRepository repository)
        {
            var configFile = ApplicationPath.GetRepositoryConfigFile(repository, c_tankConfigStorageFile);
            try
            {
                using (var file = File.OpenRead(configFile))
                {
                    return (Dictionary<string, TankInstanceConfigurationInfo>)s_configInfoSerializer.ReadObject(file);
                }
            }
            catch (Exception ex)
            {
                typeof(TankInstance).LogError("failed to load tank config info for repository '{0}': {1}", repository.ID, ex.Message);
                return new Dictionary<string, TankInstanceConfigurationInfo>();
            }
        }

        private static Dictionary<string, TankInstanceConfigurationInfo> GetRepositoryTankConfigInfoStorage(IRepository repository)
        {
            return s_configInfoStorages.GetOrCreate(repository, TankInstanceManager.LoadRepositoryTankConfigInfoStorage);
        }

        private static TankInstance GetInstance(IRepository repository, IXQueryable tank, TankUnikey unikey)
        {
            var key = unikey.ToString();

            return s_instances.GetOrCreate(key, () =>
            {
                var storage = TankInstanceManager.GetRepositoryTankConfigInfoStorage(repository);
                TankInstanceConfigurationInfo configInfo;
                storage.TryGetValue(key, out configInfo);
                var instance = new TankInstance(repository, TankEntity.Create(tank), configInfo);
                storage[key] = instance.ConfigurationInfo;
                return instance;
            });
        }

        public static TankInstance GetInstance(IRepository repository, IXQueryable tank)
        {
            return TankInstanceManager.GetInstance(repository, tank, new TankUnikey(repository, tank));
        }

        public static TankInstance GetInstance(TankUnikey key)
        {
            IRepository repository;
            IXQueryable tank;
            if (key.TryGetTank(out tank, out repository))
                return TankInstanceManager.GetInstance(repository, tank, key);
            else
                return null;
        }
    }
}
