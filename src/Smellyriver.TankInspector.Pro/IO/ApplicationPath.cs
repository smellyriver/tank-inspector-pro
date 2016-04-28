using System.IO;
using System.Reflection;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.IO
{
    public static class ApplicationPath
    {
        public const string RepositoryDirectoryName = "Repositories";
        public const string ModuleDirectoryName = "Modules";
        public const string ConfigDirectoryName = "Config";
        public const string DataDirectoryName = "Data";

        public static readonly string LocalDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string ConfigDirectory = Path.Combine(LocalDirectory, ConfigDirectoryName);
        public static readonly string DataDirectory = Path.Combine(LocalDirectory, DataDirectoryName);
        public static readonly string ModuleDirectory = Path.Combine(LocalDirectory, ModuleDirectoryName);
        public static readonly string RepositoryDirectory = Path.Combine(LocalDirectory, RepositoryDirectoryName);

        static ApplicationPath()
        {
            if (!Directory.Exists(ConfigDirectory))
                Directory.CreateDirectory(ConfigDirectory);

            if (!Directory.Exists(RepositoryDirectory))
                Directory.CreateDirectory(RepositoryDirectory);
        }

        private static string EnsureExisted(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        private static string GetRepositoryDirectoryID(IRepository repository)
        {
            return repository.ID.Replace(Path.GetInvalidFileNameChars(), '_');
        }

        public static string GetRepositoryDirectory(IRepository repository)
        {
            return ApplicationPath.EnsureExisted(Path.Combine(RepositoryDirectory,
                                                              ApplicationPath.GetRepositoryDirectoryID(repository)));
        }

        public static string GetRepositoryConfigDirectory(IRepository repository)
        {
            return ApplicationPath.EnsureExisted(Path.Combine(ApplicationPath.GetRepositoryDirectory(repository),
                                                              ConfigDirectoryName));
        }

        public static string GetRepositoryDataDirectory(IRepository repository)
        {
            return ApplicationPath.EnsureExisted(Path.Combine(ApplicationPath.GetRepositoryDirectory(repository),
                                                              DataDirectoryName));
        }

        public static string GetRepositoryConfigFile(IRepository repository, string filename)
        {
            return Path.Combine(ApplicationPath.GetRepositoryConfigDirectory(repository), filename);
        }

        public static string GetRepositoryDataFile(IRepository repository, string filename)
        {
            return Path.Combine(ApplicationPath.GetRepositoryDataDirectory(repository), filename);
        }

        public static string GetConfigPath(string filename)
        {
            return Path.Combine(ConfigDirectory, filename);
        }

        public static string GetDataFile(string filename)
        {
            return Path.Combine(DataDirectory, filename);
        }

        public static string GetModuleDirectory(Assembly moduleAssembly)
        {
            return ApplicationPath.EnsureExisted(Path.Combine(ModuleDirectory, moduleAssembly.GetName().Name));
        }
    }
}
