using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    class ScriptHost
    {
        private readonly Dictionary<string, Script> _scripts;

        public ScriptingContext ScriptingContext { get; private set; }

        private bool _isInvalidated;

        private ElementChangedEventHandler _elementChanged;
        public event ElementChangedEventHandler ElementChanged
        {
            add { _elementChanged += value; }
            remove { _elementChanged -= value; }
        }

        internal XElement Element
        {
            get
            {
                this.Update();
                return this.ScriptingContext.Element;
            }
        }

        internal CrewConfiguration CrewConfiguration { get; set; }

        public ScriptHost()
        {
            _scripts = new Dictionary<string, Script>();
            _isInvalidated = true;
        }

        public void SetScript(string key, Script script)
        {
            if (script != null)
                _scripts[key] = script;
            else if (_scripts.ContainsKey(key))
                _scripts.Remove(key);

            this.Invalidate();
        }

        public Script GetScript(string key)
        {
            Script script;
            if (_scripts.TryGetValue(key, out script))
                return script;

            return null;
        }

        public void Invalidate()
        {
            _isInvalidated = true;

            if (_elementChanged != null)
                _elementChanged(this, new ElementChangedEventArgs(this.Element));
        }

        private void Update()
        {
            if (_isInvalidated)
                this.Execute();
        }

        private void Execute()
        {
            this.ScriptingContext = new ScriptingContext(this.CrewConfiguration);
            foreach (var script in _scripts.Values.OrderBy(s => s.Priority))
                script.Execute(ScriptingContext);

            _isInvalidated = false;
        }

    }
}
