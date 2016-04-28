using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.IO;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    class TankDataFieldManager
    {
        public const string Xmlns = "http://schemas.smellyriver.com/stipro/patchnote-generator";

        private const string c_definitionFile = "TankDataFields.xml";
        private readonly static string s_definitionFile
            = Path.Combine(ApplicationPath.GetModuleDirectory(Assembly.GetCallingAssembly()), c_definitionFile);
        public readonly static TankDataFieldManager Instance;

        static TankDataFieldManager()
        {
            Instance = new TankDataFieldManager();
        }

        private Dictionary<string, TankDataFieldBase[]> _tankDataFields;

        public TankDataFieldManager()
        {
            var serializer = new DataContractSerializer(typeof(TankDataFieldBase[]), "TankDataFields", Xmlns);
            using(var file = File.OpenRead(s_definitionFile))
            {
                var fields = (TankDataFieldBase[])serializer.ReadObject(file);
                _tankDataFields = fields.GroupBy(f => f.ElementName).ToDictionary(g => g.Key, g => g.ToArray());
            }
        }

        public IEnumerable<TankDataFieldBase> GetFields(string elementName)
        {
            TankDataFieldBase[] items;
            if (!_tankDataFields.TryGetValue(elementName, out items))
                return null;

            return items;
        }
    }
}
