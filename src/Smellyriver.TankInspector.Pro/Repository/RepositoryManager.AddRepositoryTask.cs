using System.Collections.Generic;
using System.IO;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;

namespace Smellyriver.TankInspector.Pro.Repository
{
    partial class RepositoryManager
    {
        class AddRepositoryTask : CompositeTask
        {

            public override string Name
            {
                get { return "addRepository:" + _path; }
            }

            private readonly string _path;
            private LocalGameClient _client;
            private readonly RepositoryManager _repositoryManager;

            public AddRepositoryTask(RepositoryManager repositoryManager, string path)
            {
                _repositoryManager = repositoryManager;
                _path = path;
            }

            public override void PreProcess(IProgressScope progress)
            {
                base.PreProcess(progress);
                progress.ReportStatusMessage(this.L("game_client_manager", "status_loading_game_client_with_path", _path));
            }

            protected override IEnumerable<TaskInfo> GetChildren()
            {
                yield return TaskInfo.FromAction("loadGameClient", 1.0, progress =>
                    {
                        progress.ReportStatusMessage(this.L("game_client_manager", "status_loading_game_client"));
                        _client = new LocalGameClient(_path, progress);
                    });

                yield return TaskInfo.FromAction("loadGameClientConfiguration", 9.0, progress =>
                    {
                        progress.ReportStatusMessage(this.L("game_client_manager", "status_loading_game_client_configuration"));

                        var repositoryConfigFile = ApplicationPath.GetRepositoryConfigFile(_client, RepositoryConfigFile);

                        RepositoryConfiguration config;
                        if (File.Exists(repositoryConfigFile))
                        {
                            config = RepositoryConfiguration.Load(repositoryConfigFile);
                            config.MarkerColor = _repositoryManager.MarkerManager.ApplyForColor(config.MarkerColor);
                        }
                        else
                        {
                            config = new RepositoryConfiguration(_client);
                            config.MarkerColor = _repositoryManager.MarkerManager.ApplyForColor();
                        }

                        _repositoryManager._repositoryConfigurations[_client] = config;

                        _repositoryManager._repositories.Add(_client);

                    });
            }

        }
    }
}
