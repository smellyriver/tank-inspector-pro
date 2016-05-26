using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class Tank : TankObject, IModuleUnlockTarget
    {

        public static Tank Create(IXQueryable tankData)
        {
            if (tankData is Tank)
                return (Tank)tankData;
            else
                return new Tank(tankData);
        }

        public bool IsPremium
        {
            get { return this["price/@currency"] == "gold"; }
        }

        public bool IsObsoleted
        {
            get
            {
                return !this.IsPremium
                    && !this.Query("successors").Any()
                    && !this.Query("predecessors").Any();
            }
        }

        public string ShortName
        {
            get { return this["shortUserString"]; }
        }

        public string Description
        {
            get { return this["description"]; }
        }

        public string FullKey
        {
            get { return this["@fullKey"]; }
        }

        public string IconKey
        {
            get { return this["@iconKey"]; }
        }

        public string Nation
        {
            get { return this["nation"]; }
        }

        public string NationKey
        {
            get { return this["nation/@key"]; }
        }

        public string ClassKey
        {
            get { return this["class/@key"]; }
        }

        public string ClassName
        {
            get { return this["class"]; }
        }

        public int Tier
        {
            get { return this.QueryInt("level"); }
        }

        public IEnumerable<string> SecretTags
        {
            get { return this.QueryManyValues("secretTags/tag"); }
        }

        public TankClass Type
        {
            get { return TankClassHelper.FromClassKey(this["class"]); }
        }

        public IEnumerable<string> PredecessorKeys
        {
            get { return this.QueryManyValues("predecessors/predecessor/@key"); }
        }

        public IEnumerable<string> SuccessorKeys
        {
            get { return this.QueryManyValues("successors/successor/@key"); }
        }


        private Turret[] _turrets;

        public IEnumerable<Turret> Turrets
        {
            get
            {
                return _turrets ?? (_turrets = this.QueryMany("turrets/turret").Select(t => new Turret(t)).ToArray());
            }
        }

        private Engine[] _engines;
        public IEnumerable<Engine> Engines
        {
            get
            {
                return _engines ?? (_engines = this.QueryMany("engines/engine").Select(t => new Engine(t)).ToArray());
            }
        }

        private Radio[] _radios;
        public IEnumerable<Radio> Radios
        {
            get { return _radios ?? (_radios = this.QueryMany("radios/radio").Select(t => new Radio(t)).ToArray()); }
        }

        private Chassis[] _chassis;
        public IEnumerable<Chassis> Chassis
        {
            get
            {
                return _chassis ?? (_chassis = this.QueryMany("chassis/chassis").Select(t => new Chassis(t)).ToArray());
            }
        }

        private Gun[] _guns;
        public IEnumerable<Gun> Guns
        {
            get { return _guns ?? (_guns = this.Turrets.SelectMany(t => t.Guns).Distinct().ToArray()); }
        }

        private Hull _hull;

        public Hull Hull
        {
            get { return _hull ?? (_hull = new Hull(this.Query("hull"))); }
        }

        public Tank(IXQueryable tankData)
            : base(tankData)
        {

        }

    }
}
