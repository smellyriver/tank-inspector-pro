using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator
{
    class RepositoryHelper
    {
        public static string GetRepositoryDisplayName(IRepository repository)
        {
            var configuration = RepositoryManager.Instance.GetConfiguration(repository);
            if (configuration.Alias == repository.Version.ToString())
                return configuration.Alias;
            else
                return string.Format("{0} ({1})", configuration.Alias, repository.Version);
        }

    }
}
