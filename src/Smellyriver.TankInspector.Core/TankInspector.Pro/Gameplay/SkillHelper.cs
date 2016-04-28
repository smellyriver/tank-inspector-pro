using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Gameplay
{
    public static class SkillHelper
    {
        public static IEnumerable<IXQueryable> GetSkills(IRepository repository, string role, bool includeCommonSkills = false)
        {
            if (includeCommonSkills)
                foreach (var skill in repository.CrewDatabase.QueryMany("skills/skill[not(@role)]"))
                    yield return skill;

            foreach (var skill in repository.CrewDatabase.QueryMany("skills/skill[@role='{0}']", role))
                yield return skill;
        }

        public static IEnumerable<IXQueryable> GetSkills(IRepository repository, string primaryRole, IEnumerable<string> secondaryRoles, bool includeCommonSkills = true)
        {
            return SkillHelper.GetSkills(repository, primaryRole, includeCommonSkills)
                .Union(secondaryRoles.SelectMany(r => SkillHelper.GetSkills(repository, r, false)));
        }
    }
}
