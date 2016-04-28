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

        public string Class
        {
            get { return this["class"]; }
        }

        public int Tier
        {
            get { return this.QueryInt("level"); }
        }

        private Turret[] _turrets;

        public IEnumerable<Turret> Turrets
        {
            get
            {
                if (_turrets == null)
                    _turrets = this.QueryMany("turrets/turret").Select(t => new Turret(t)).ToArray();

                return _turrets;
            }
        }

        private Engine[] _engines;
        public IEnumerable<Engine> Engines
        {
            get
            {
                if (_engines == null)
                    _engines = this.QueryMany("engines/engine").Select(t => new Engine(t)).ToArray();

                return _engines;
            }
        }

        private Radio[] _radios;
        public IEnumerable<Radio> Radios
        {
            get
            {
                if (_radios == null)
                    _radios = this.QueryMany("radios/radio").Select(t => new Radio(t)).ToArray();

                return _radios;
            }
        }

        private Chassis[] _chassis;
        public IEnumerable<Chassis> Chassis
        {
            get
            {
                if (_chassis == null)
                    _chassis = this.QueryMany("chassis/chassis").Select(t => new Chassis(t)).ToArray();

                return _chassis;
            }
        }

        private Gun[] _guns;
        public IEnumerable<Gun> Guns
        {
            get
            {
                if (_guns == null)
                    _guns = this.Turrets.SelectMany(t => t.Guns).Distinct().ToArray();

                return _guns;
            }
        }

        private Hull _hull;

        public Hull Hull
        {
            get
            {
                if (_hull == null)
                    _hull = new Hull(this.Query("hull"));

                return _hull;
            }
        }

        public Tank(IXQueryable tankData)
            : base(tankData)
        {

        }

    }
}
