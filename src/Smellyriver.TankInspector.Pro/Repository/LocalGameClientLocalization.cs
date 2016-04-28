using Smellyriver.TankInspector.Common.Text;
using Smellyriver.TankInspector.IO.Gettext;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public class LocalGameClientLocalization : IRepositoryLocalization
    {

        private readonly LocalizationDatabase _localizationDatabase;

        public LocalGameClientLocalization(string textFolder)
        {
            _localizationDatabase = new LocalizationDatabase(textFolder);
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
