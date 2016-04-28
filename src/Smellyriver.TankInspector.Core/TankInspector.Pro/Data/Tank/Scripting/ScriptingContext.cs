using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    class ScriptingContext : CoreNotificationObject, ISupportInitialize, ICloneable
    {

        private bool _isInitializing;
        private HashSet<string> _changedDomainsDuringInitialization;

        private Dictionary<string, Dictionary<string, double>> _storage;

        private readonly CrewConfiguration _crewConfiguration;

        internal XElement Element { get; private set; }

        public ScriptingContext(CrewConfiguration crewConfiguration)
        {
            _storage = new Dictionary<string, Dictionary<string, double>>();
            using (var stream = Core.Support.GetDefaultStatesStream())
            using (var xmlReader = XmlReader.Create(stream))
                this.Element = XElement.Load(xmlReader);
            _crewConfiguration = crewConfiguration;
        }

        internal int GetCrewCount(string primaryRole)
        {
            if (primaryRole == null)
                return _crewConfiguration.Crews.Count();
            else
                return _crewConfiguration.Crews.Count(c => c.PrimaryRole == primaryRole);
        }

        private XElement GetDomainElement(string domain)
        {
            var domainElement = this.Element.Element(domain);
            if (domainElement == null)
            {
                domainElement = new XElement(domain);
                this.Element.Add(domainElement);
            }

            return domainElement;
        }

        public void SetValue(string domain, string key, double value)
        {
            var domainStorage = _storage.GetOrCreate(domain, () => new Dictionary<string, double>());
            domainStorage[key] = value;

            if (!Regex.IsMatch(key, @"__level\d"))
                this.GetDomainElement(domain).SetElementValue(key, value);

            this.RaiseDomainChanged(domain);
        }

        private void RaiseDomainChanged(string domain)
        {
            if (_isInitializing)
                _changedDomainsDuringInitialization.Add(domain);
            else
                this.RaisePropertyChanged(domain);
        }

        public bool HasDomain(string domain)
        {
            return _storage.ContainsKey(domain);
        }

        public double GetValue(string domain, string key, double defaultValue)
        {
            var value = this.GetValue(domain, key);
            if (value == null)
                return defaultValue;
            else
                return value.Value;
        }

        public double? GetValue(string domain, string key)
        {
            Dictionary<string, double> domainStorage;
            if (_storage.TryGetValue(domain, out domainStorage))
            {
                double value;
                if (domainStorage.TryGetValue(key, out value))
                    return value;
            }

            return null;
        }

        public void Clear()
        {
            var domains = _storage.Keys.ToArray();
            _storage.Clear();
            this.Element.RemoveNodes();
            foreach (var domain in domains)
                this.RaiseDomainChanged(domain);
        }



        public void BeginInit()
        {
            _isInitializing = true;
            _changedDomainsDuringInitialization = new HashSet<string>();
        }

        public void EndInit()
        {
            _isInitializing = false;

            foreach (var domain in _changedDomainsDuringInitialization)
                this.RaiseDomainChanged(domain);

            _changedDomainsDuringInitialization = null;
        }

        public ScriptingContext Clone()
        {
            var clone = (ScriptingContext)this.MemberwiseClone();
            clone._storage = new Dictionary<string, Dictionary<string, double>>();
            clone.Element = new XElement(this.Element);

            foreach (var pair in this._storage)
            {
                clone._storage.Add(pair.Key, new Dictionary<string, double>(pair.Value));
            }

            clone.ClearEventHandlers();

            return clone;
        }


        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
