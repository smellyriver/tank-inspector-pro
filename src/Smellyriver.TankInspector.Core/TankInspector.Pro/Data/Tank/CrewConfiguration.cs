using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Pro.Data.Tank.Scripting;
using Smellyriver.TankInspector.Pro.Repository;
using TankEntity = Smellyriver.TankInspector.Pro.Data.Entities.Tank;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class CrewConfiguration : ConfigurationBase
    {
        public static readonly IEqualityComparer<IXQueryable> KeyEqualityComparer
               = ProjectionEqualityComparer<IXQueryable>.Create(g => g == null ? null : g["@key"]);


        private CrewConfigurationInfo _crewConfigurationInfo;
        public CrewConfigurationInfo CrewConfigurationInfo
        {
            get { return _crewConfigurationInfo; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _crewConfigurationInfo = value;

                foreach (var crew in this.Crews)
                {
                    crew.CrewInstanceInfo = _crewConfigurationInfo[crew.PrimaryRole];
                }
            }
        }


        private readonly CrewInstance[] _crews;
        public IEnumerable<CrewInstance> Crews
        {
            get { return _crews; }
        }

        public string NationKey
        {
            get { return this.Tank["nation/@key"]; }
        }

        private readonly XElement _element;

        internal XElement Element
        {
            get { return _element; }
        }

        internal CrewConfiguration(IRepository repository, TankEntity tank, ScriptHost scriptHost, CrewConfigurationInfo configInfo)
            : base(repository, tank, scriptHost)
        {
            if (configInfo == null)
                _crewConfigurationInfo = new CrewConfigurationInfo();

            _element = new XElement("crews");

            var crewDatum = tank.QueryMany("crews/crew");

            var redundancyCounters = new Dictionary<string, int>();
            var crewList = new List<CrewInstance>();
            foreach (var data in crewDatum)
            {
                var primaryRole = data["@role"];
                var secondaryRoles = data.QueryManyValues("secondaryRoles/secondaryRole").ToArray();
                int redundancyIndex;
                if (redundancyCounters.ContainsKey(primaryRole))
                    redundancyIndex = redundancyCounters[primaryRole] = redundancyCounters[primaryRole] + 1;
                else
                    redundancyIndex = redundancyCounters[primaryRole] = 0;

                var crew = new CrewInstance(this, primaryRole, secondaryRoles, redundancyIndex);
                crewList.Add(crew);
                this.Element.Add(crew.Element);

                if (configInfo == null)
                    _crewConfigurationInfo[crew.PrimaryRole] = crew.CrewInstanceInfo;
            }

            _crews = crewList.ToArray();

            if (configInfo != null)
                _crewConfigurationInfo = configInfo;
        }
    }
}
