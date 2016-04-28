using System.Collections.Generic;
using System.IO;
using System.Linq;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;

namespace Smellyriver.TankInspector.Pro.Repository
{
    partial class RepositoryManager
    {
        internal class LoadRepositoriesTask : CompositeTask
        {
            private readonly RepositoryManager _repositoryManager;

            public override string Name
            {
                get { return "loadRepositories"; }
            }

            public LoadRepositoriesTask(RepositoryManager repositoryManager)
            {
                _repositoryManager = repositoryManager;
            }

            public override void PreProcess(IProgressScope progress)
            {
                base.PreProcess(progress);
                progress.ReportStatusMessage(this.L("game_client_manager", "status_loading_game_clients"));
            }

            protected override IEnumerable<TaskInfo> GetChildren()
            {
                yield return TaskInfo.FromAction("beginLoadRepositories", 
                                                 1.0, 
                                                 p => p.ReportStatusMessage(this.L("game_client_manager", "status_loading_game_clients")));

                if (!File.Exists(s_repositoriesConfigFile))
                    yield break;

                var paths = File.ReadAllLines(s_repositoriesConfigFile)
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .ToArray();


                foreach (var path in paths)
                {
                    if (LocalGameClientPath.IsPathValid(path))
                    {
                        yield return new TaskInfo(new AddRepositoryTask(_repositoryManager, path), 1.0);
                    }
                    else
                    {
                        this.LogError("invalid local game client path '{0}'", path);
                    }
                }
            }



        }
    }
}
