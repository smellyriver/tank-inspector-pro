using System.Collections.Generic;
using System.Diagnostics;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    [DebuggerDisplay("{Name}")]
    class ItemName
    {

        private readonly string _baseName;

        private readonly List<ItemNameModifier> _modifiers;

        public IEnumerable<ItemNameModifier> Modifiers { get { return _modifiers; } }

        public string Name
        {
            get
            {
                var name = _baseName;

                foreach (var modifier in _modifiers)
                    name = modifier.Modify(name);

                return name;
            }
        }

        public ItemName(string baseName, bool wrapWithBrackets = true)
        {
            _baseName = baseName;
            if (wrapWithBrackets)
                _baseName = string.Format("[{0}]", _baseName);

            _modifiers = new List<ItemNameModifier>();
        }

        public void AddModifier(ItemNameModifier modifier)
        {
            var insertIndex = 0;

            for (; insertIndex < _modifiers.Count; ++insertIndex)
                if (_modifiers[insertIndex].Priority > modifier.Priority)
                    break;

            _modifiers.Insert(insertIndex, modifier);
        }

        public override string ToString()
        {
            return this.Name;
        }

    }
}
