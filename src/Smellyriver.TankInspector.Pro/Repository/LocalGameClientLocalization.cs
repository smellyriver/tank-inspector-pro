using System.IO;
using Smellyriver.TankInspector.Common.Text;
using Smellyriver.TankInspector.IO.Gettext;
using Smellyriver.TankInspector.IO.XmlDecoding;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public class LocalGameClientLocalization : IRepositoryLocalization
    {

        private readonly LocalizationDatabase _localizationDatabase;

        public string Language { get; }

        public LocalGameClientLocalization(string textSettingsFile, string textFolder)
        {
            _localizationDatabase = new LocalizationDatabase(textFolder);

            if (File.Exists(textSettingsFile))  // this file might not be existed on some game clients
            {
                using (var reader = new BigworldXmlReader(textSettingsFile))
                {
                    reader.ReadStartElement();
                    reader.ReadToNextSibling("clientLangID");
                    reader.ReadStartElement("clientLangID");
                    this.Language = reader.Value;
                }
            }
            else
            {
                this.Language = "en-us";
            }
        }

        public string GetLocalizedString(string key)
        {
            return _localizationDatabase.GetText(key);
        }

        public string GetLocalizedNationName(string nation)
        {
            return this.GetLocalizedString(string.Format("#menu:nations/{0}", nation));
        }

        public string GetLocalizedTankName(string nation, string key)
        {
            return this.GetLocalizedString(string.Format("#{0}_vehicles:{1}", nation, key));
        }

        public string GetLocalizedClassName(string @class)
        {
            if (@class == "observer")
                return "Observer";

            return this.GetLocalizedString(string.Format("#item_types:vehicle/tags/{0}/name", NameConvension.CamelToCStyle(@class, true)));
        }

    }
}
