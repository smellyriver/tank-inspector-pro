using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.IO.Gettext;

namespace Smellyriver.TankInspector.Pro.Globalization
{
    public class Localization
    {
        public const string DefaultCatalogName = "Default";

        public static readonly CultureInfo FallbackCulture = CultureInfo.GetCultureInfo("en-US");

        public static Localization Instance { get; private set; }

        internal static void Initialize(string uiCulture)
        {
            Localization.Instance = new Localization(uiCulture);
        }


        private static string GetCatalogKey(string catalog, string l10nDirectory)
        {
            return string.Format("{0}>{1}", l10nDirectory.ToLower(), catalog);
        }

        public CultureInfo UICulture { get; private set; }


        private readonly Dictionary<string, Catalog> _catalogs;
        private readonly Dictionary<Assembly, LocalizationSettingsAttribute> _assemblyLocalizationSettings;

        private Localization(string uiCulture)
        {
            _catalogs = new Dictionary<string, Catalog>();
            _assemblyLocalizationSettings = new Dictionary<Assembly, LocalizationSettingsAttribute>();
            this.LoadUICulture(uiCulture);
        }

        private void LoadUICulture(string uiCulture)
        {
            try
            {
                if (string.IsNullOrEmpty(uiCulture))
                    this.UICulture = CultureInfo.CurrentUICulture;
                else
                    this.UICulture = CultureInfo.GetCultureInfo(uiCulture);
            }
            catch (CultureNotFoundException)
            {
                //this.LogError("UI Culture '{0}' not found, fallback to default culture", ApplicationSettings.Default.UICulture);
                //ApplicationSettings.Default.UICulture = Localization.FallbackCulture.Name;
                //ApplicationSettings.Default.Save();
                this.UICulture = FallbackCulture;
            }

            Thread.CurrentThread.CurrentUICulture = this.UICulture;

            typeof(CultureInfo).GetField("s_userDefaultUICulture", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, this.UICulture);
        }

        private LocalizationSettingsAttribute GetAssemblyLocalizationSettings(Assembly assembly)
        {
            return _assemblyLocalizationSettings.GetOrCreate(assembly,
                                                             () => assembly.GetDistinctAttribute<LocalizationSettingsAttribute>(false));
        }

        private string GetL10nDirectory(Assembly assembly)
        {
            var settings = this.GetAssemblyLocalizationSettings(assembly);

            var localizationDiretory = settings == null
                                     ? LocalizationSettingsAttribute.DefaultLocalizationDirectory
                                     : settings.LocalizationDirectory;

            return Path.Combine(Path.GetDirectoryName(assembly.Location), localizationDiretory);
        }

        public string Translate(string key,
                                string catalogName = DefaultCatalogName,
                                Assembly assembly = null,
                                bool? fallbackToEntryAssembly = null,
                                string defaultValue = null)
        {
            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            if (fallbackToEntryAssembly == null)
            {
                var settings = this.GetAssemblyLocalizationSettings(assembly);
                fallbackToEntryAssembly = settings == null ? true : settings.FallbackToMainAssembly;
            }

            var l10nDirectory = this.GetL10nDirectory(assembly);

            string translatedText;
            if (this.TryTranslate(key, catalogName, l10nDirectory, out translatedText))
                return translatedText;

            if (fallbackToEntryAssembly == true)
            {
                l10nDirectory = this.GetL10nDirectory(Assembly.GetExecutingAssembly());
                if (this.TryTranslate(key, catalogName, l10nDirectory, out translatedText))
                    return translatedText;
            }

            if (defaultValue != null)
                return defaultValue;

            return string.Format("missing: {0}:{1}", catalogName, key);
        }

        public string Translate(string key, string catalogName, string l10nDirectory)
        {
            string translatedText;
            if (this.TryTranslate(key, catalogName, l10nDirectory, out translatedText))
                return translatedText;

            return string.Format("missing: {0}:{1}", catalogName, key);
        }

        public bool TryTranslate(string key, string catalogName, string l10nDirectory, out string translatedText)
        {
            if (catalogName == null)
                catalogName = DefaultCatalogName;

            var catalogKey = Localization.GetCatalogKey(catalogName, l10nDirectory);

            var catalog = _catalogs.GetOrCreate(catalogKey, () => this.LoadCatalog(catalogName, l10nDirectory));

            if (catalog == null)
            {
                translatedText = null;
                return false;
            }

            translatedText = catalog.Gettext(key);
            return true;
        }

        private Catalog LoadCatalog(string catalogName, string l10nDirectory)
        {
            var path = this.GetLocalizedFile(catalogName + ".mo", l10nDirectory);

            if (path == null)
                return null;

            try
            {
                return Catalog.ReadFrom(path);
            }
            catch (Exception /*ex*/)
            {
                //this.LogError("failed to load localization catalog '{0}' :'{1}'", path, ex.Message);
                return null;
            }
        }

        public string GetLocalizedFile(string filename, Assembly assembly = null, bool? fallbackToEntryAssembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            if (fallbackToEntryAssembly == null)
            {
                var settings = this.GetAssemblyLocalizationSettings(assembly);
                fallbackToEntryAssembly = settings == null ? true : settings.FallbackToMainAssembly;
            }

            var l10nDirectory = this.GetL10nDirectory(assembly);
            var file = this.GetLocalizedFile(filename, l10nDirectory);

            if (file != null)
                return file;

            if (fallbackToEntryAssembly == true)
            {
                l10nDirectory = this.GetL10nDirectory(Assembly.GetExecutingAssembly());
                file = this.GetLocalizedFile(filename, l10nDirectory);
                return file;
            }

            return null;
        }

        public string GetLocalizedFile(string filename, string l10nDirectory)
        {
            var path = Path.Combine(l10nDirectory, CultureInfo.CurrentUICulture.Name, filename);
            if (!File.Exists(path))
            {
                //this.LogWarning("file '{0}' is not existed in '{1}', fallback to default culture", filename, l10nDirectory);
                path = Path.Combine(l10nDirectory, FallbackCulture.Name, filename);
                if (!File.Exists(path))
                {
                    //this.LogError("file '{0}' is not existed in default culture ('{1}')", filename, l10nDirectory);
                    return null;
                }
            }

            return path;
        }

    }
}
