using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Gameplay;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public static class IRepositoryExtensions
    {
        public static IEnumerable<UnlockInfo> GetUnlocks(this IRepository repository, Tank tank, Module module)
        {
            var unlocks = module.QueryMany("unlocks/*");

            foreach (var unlock in unlocks)
            {
                if (unlock.Name == "vehicle")
                {
                    var unlockedTank = repository.TankDatabase.Query("tank[@key='{0}']", unlock["@key"]);
                    if (unlockedTank != null)
                        yield return new UnlockInfo(new Tank(unlockedTank), unlock.QueryInt("cost"));

                    continue;
                }

                string searchRootPath;
                switch (unlock.Name)
                {
                    case "gun":
                        searchRootPath = "turrets/turret/guns/gun";
                        break;
                    case "chassis":
                        searchRootPath = "chassis/chassis";
                        break;
                    case "turret":
                        searchRootPath = "turrets/turret";
                        break;
                    case "engine":
                        searchRootPath = "engines/engine";
                        break;
                    case "radio":
                        searchRootPath = "radios/radio";
                        break;

                    default:
                        continue;
                }

                var unlockedModules = tank.QueryMany(string.Format("{0}[@key='{1}']",
                                                                   searchRootPath,
                                                                   unlock["@key"]))
                                          .Distinct(KeyEqualityComparer<IXQueryable>.Instance);

                foreach (var unlockedModule in unlockedModules)
                    yield return new UnlockInfo(Module.Create(unlockedModule), unlock.QueryDouble("cost"));
            }
        }
    }
}
