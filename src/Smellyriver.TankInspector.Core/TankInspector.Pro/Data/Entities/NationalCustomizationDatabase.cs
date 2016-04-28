using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public class NationalCustomizationDatabase : XQueryableWrapper
    {
        private static readonly Dictionary<IRepository, Dictionary<string, NationalCustomizationDatabase>> s_databases
            = new Dictionary<IRepository, Dictionary<string, NationalCustomizationDatabase>>();

        public static NationalCustomizationDatabase GetDatabase(IRepository repository, string nationKey)
        {
            return s_databases.GetOrCreate(repository,
                                           () => new Dictionary<string, NationalCustomizationDatabase>())
                .GetOrCreate(nationKey,
                             () => new NationalCustomizationDatabase(
                                 repository.CustomizationDatabase.Query("customization[@nation='{0}']", nationKey)));
        }

        public string InscriptionGroupName { get { return this["inscription/userString"]; } }
        public double InscriptionPriceFactor { get { return this.QueryDouble("inscription/priceFactor"); } }

        private Dictionary<int, Inscription> _inscriptions;
        public Dictionary<int, Inscription> Inscriptions
        {
            get
            {
                if (_inscriptions == null)
                    _inscriptions = this.LoadInscriptions();

                return _inscriptions;
            }
        }

        private bool _camouflageLoaded;

        private Dictionary<string, CamouflageGroup> _camouflageGroups;
        private Dictionary<string, CamouflageGroup> _readonlyCamouflageGroups;
        public Dictionary<string, CamouflageGroup> CamouflageGroups
        {
            get
            {
                if (!_camouflageLoaded)
                    this.LoadCamouflages();

                return _readonlyCamouflageGroups;
            }
        }

        private Dictionary<int, Camouflage> _camouflages;
        private Dictionary<int, Camouflage> _readonlyCamouflages;
        public Dictionary<int, Camouflage> Camouflages
        {
            get
            {
                if (!_camouflageLoaded)
                    this.LoadCamouflages();

                return _readonlyCamouflages;
            }
        }



        public NationalCustomizationDatabase(IXQueryable data)
            : base(data)
        {

        }


        private Dictionary<int, Inscription> LoadInscriptions()
        {
            return new Dictionary<int, Inscription>(
                this.QueryMany("inscription/inscriptions/inscription")
                    .ToDictionary(i => int.Parse(i["@id"], CultureInfo.InvariantCulture), i => new Inscription(i)));

        }

        private void LoadCamouflages()
        {
            _camouflageGroups = this.QueryMany("camouflageGroups/camouflageGroup")
                                    .ToDictionary(e => e["@key"], e => new CamouflageGroup(e));

            _readonlyCamouflageGroups = new Dictionary<string, CamouflageGroup>(_camouflageGroups);

            _camouflages = this.QueryMany("camouflages/camouflage")
                               .ToDictionary(e => int.Parse(e["@id"], CultureInfo.InvariantCulture), e => new Camouflage(e));

            _readonlyCamouflages = new Dictionary<int, Camouflage>(_camouflages);

            foreach (var group in _camouflageGroups)
            {
                group.Value.InternalCamouflages = _camouflages.Values.Where(c => c["group"].Trim() == group.Key)
                                                                     .ToArray();

                foreach (var camouflage in group.Value.InternalCamouflages)
                    camouflage.Group = group.Value;
            }

            _camouflageLoaded = true;
        }
    }
}
