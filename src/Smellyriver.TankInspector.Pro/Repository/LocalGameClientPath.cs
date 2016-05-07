using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public sealed class LocalGameClientPath
    {


        private const string c_vehiclesFolder = @"res\scripts\item_defs\vehicles";
        private const string c_packageFolder = @"res\packages";
        private const string c_versionFile = @"version.xml";
        private const string c_textFolder = @"res\text\LC_MESSAGES";
        private const string c_textSettingsFile = @"res\text\settings.xml";
        private const string c_commonVehicleDataFile = @"res\scripts\item_defs\vehicles\common\vehicle.xml";
        private const string c_equipmentDataFile = @"res\scripts\item_defs\vehicles\common\optional_devices.xml";
        private const string c_consumableDataFile = @"res\scripts\item_defs\vehicles\common\equipments.xml";
        private const string c_commonCrewDataFile = @"res\scripts\item_defs\tankmen\tankmen.xml";
        private const string c_crewDefinitionFile = @"res\scripts\item_defs\tankmen\{0}.xml";
        private const string c_pathsFile = @"paths.xml";

        private const string c_shellXmlFile = @"components\shells.xml";
        private const string c_radioXmlFile = @"components\radios.xml";
        private const string c_fuelTanksXmlFile = @"components\fueltanks.xml";
        private const string c_chassisXmlFile = @"components\chassis.xml";
        private const string c_enginesXmlFile = @"components\engines.xml";
        private const string c_gunsXmlFile = @"components\guns.xml";
        private const string c_turretsXmlFile = @"components\turrets.xml";

        private const string c_guiPackage = @"res\packages\gui.pkg";

        private const string c_techTreeLayoutRelativePath = @"gui\flash\techtree";
        private const string c_techTreeLayoutFilePostfix = "-tree.xml";

        public static bool IsPathValid(string rootPath)
        {
            return File.Exists(Path.Combine(rootPath, c_versionFile))
                && Directory.Exists(Path.Combine(rootPath, c_vehiclesFolder))
                && Directory.Exists(Path.Combine(rootPath, c_textFolder))
                && File.Exists(Path.Combine(rootPath, c_commonVehicleDataFile))
                && File.Exists(Path.Combine(rootPath, c_equipmentDataFile))
                && File.Exists(Path.Combine(rootPath, c_consumableDataFile))
                && File.Exists(Path.Combine(rootPath, c_commonCrewDataFile));
        }

        private static bool IsNationFolder(string path)
        {
            return Directory.Exists(Path.Combine(path, "components"))
                && File.Exists(Path.Combine(path, "list.xml"))
                && File.Exists(Path.Combine(path, "customization.xml"))
                && File.Exists(Path.Combine(path, "../../tankmen/" + Path.GetFileName(path) + ".xml"));
        }

        public string RootPath { get; }
        public GameVersion Version { get; private set; }
        public string DefaultModDirectory
        {
            get
            {
                var versionPath = string.Format("{0}.{1}.{2}{3}",
                                                this.Version.Major,
                                                this.Version.Minor,
                                                this.Version.Build,
                                                this.Version.IsCommonTest ? " Common Test" : null);
                return Path.Combine(this.RootPath,
                                    "res_mods",
                                    versionPath);
            }
        }

        public string PathsXmlFile
        {
            get { return Path.Combine(this.RootPath, c_pathsFile); }
        }

        public string VehiclesFolder
        {
            get { return Path.Combine(this.RootPath, c_vehiclesFolder); }
        }

        public string TextFolder
        {
            get { return Path.Combine(this.RootPath, c_textFolder); }
        }

        public string TextSettingsFile
        {
            get { return Path.Combine(this.RootPath, c_textSettingsFile); }
        }

        public string VersionFile
        {
            get { return Path.Combine(this.RootPath, c_versionFile); }
        }

        public string CommonVehicleDataFile
        {
            get { return Path.Combine(this.RootPath, c_commonVehicleDataFile); }
        }

        public string EquipmentDataFile
        {
            get { return Path.Combine(this.RootPath, c_equipmentDataFile); }
        }

        public string ConsumableDataFile
        {
            get { return Path.Combine(this.RootPath, c_consumableDataFile); }
        }

        public string CommonCrewDataFile
        {
            get { return Path.Combine(this.RootPath, c_commonCrewDataFile); }
        }

        public string PackageRootPath
        {
            get { return Path.Combine(this.RootPath, c_packageFolder); }
        }

        public string GuiPackageFile
        {
            get { return Path.Combine(this.RootPath, c_guiPackage); }
        }

        private readonly string[] _nationFolders;
        public string[] NationFolders { get { return _nationFolders; } }

        private readonly string[] _nations;
        public string[] Nations { get { return _nations; } }

        public IEnumerable<string> ClientPackages { get; private set; }
        public ObservableCollection<string> ClientPaths { get; private set; }

        public LocalGameClient Client { get; private set; }

        public LocalGameClientPath(LocalGameClient client)
        {
            this.Client = client;
            this.RootPath = client.RootPath;
            this.ReadVersion();

            this.LoadClientPaths();

            _nationFolders = Directory.GetDirectories(this.VehiclesFolder).Where(LocalGameClientPath.IsNationFolder).ToArray();
            _nations = _nationFolders.Select(Path.GetFileName).ToArray();
        }

        private void LoadClientPaths()
        {
            this.ClientPaths = new ObservableCollection<string>();
            var clientPackages = new List<string>();


            IEnumerable<string> paths;

            try
            {
                paths = XElement.Load(this.PathsXmlFile).ToXQueryable().QueryManyValues("Paths/Path");
            }
            catch (Exception ex)
            {
                this.LogError("failed to load client paths file '{0}': {1}", this.PathsXmlFile, ex.Message);
                return;
            }

            foreach (var path in XElement.Load(this.PathsXmlFile).ToXQueryable().QueryManyValues("Paths/Path"))
            {
                var absolutePath = Path.Combine(this.RootPath, path.StartsWith("./") || path.StartsWith(".\\") ? path.Substring(2) : path);
                if (path.EndsWith(".pkg"))
                    clientPackages.Add(absolutePath);
                else
                    this.ClientPaths.Add(absolutePath);
            }

            this.ClientPackages = clientPackages;
            this.ClientPaths.CollectionChanged += ClientPaths_CollectionChanged;
        }

        void ClientPaths_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SaveClientPaths();
        }

        public void SaveClientPaths()
        {
            var element = new XElement("root");
            var pathsElement = new XElement("Paths");
            element.Add(pathsElement);

            foreach (var item in this.ClientPaths.Union(this.ClientPackages))
            {
                var relativePath = PathEx.Relativize(item, this.RootPath);
                var itemElement = new XElement("Path", relativePath);
                pathsElement.Add(itemElement);
            }

            var writerSettings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true
            };

            try
            {
                using (var file = File.Create(this.PathsXmlFile))
                {
                    using (var xmlWriter = XmlWriter.Create(file, writerSettings))
                    {
                        element.Save(this.PathsXmlFile);
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogError("failed to save client paths to '{0}': {1}", this.PathsXmlFile, ex.Message);
            }
        }

        private void ReadVersion()
        {
            using (var stream = File.OpenRead(this.VersionFile))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    reader.ReadToDescendant("version");
                    this.Version = GameVersion.Parse(reader.ReadString().Trim());
                }
            }
        }

        public string GetNationFolder(string nationKey)
        {
            return Path.Combine(Path.Combine(this.RootPath, c_vehiclesFolder, nationKey));
        }

        public string GetCustomizationFile(string nationKey)
        {
            return Path.Combine(this.GetNationFolder(nationKey), "customization.xml");
        }

        public string GetTankListFile(string nationKey)
        {
            return Path.Combine(this.GetNationFolder(nationKey), "list.xml");
        }

        public string GetTankFile(string nationKey, string key)
        {
            return Path.Combine(this.GetNationFolder(nationKey), string.Format("{0}.xml", key));
        }

        public string GetShellListFile(string nationKey)
        {
            return Path.Combine(this.GetNationFolder(nationKey), c_shellXmlFile);
        }

        public string GetGunListFile(string nationKey)
        {
            return Path.Combine(this.GetNationFolder(nationKey), c_gunsXmlFile);
        }

        public string GetTurretListFile(string nationKey)
        {
            return Path.Combine(this.GetNationFolder(nationKey), c_turretsXmlFile);
        }

        public string GetFuelTankListFile(string nationKey)
        {
            return Path.Combine(this.GetNationFolder(nationKey), c_fuelTanksXmlFile);
        }

        public string GetEngineListFile(string nationKey)
        {
            return Path.Combine(this.GetNationFolder(nationKey), c_enginesXmlFile);
        }

        public string GetRadioListFile(string nationKey)
        {
            return Path.Combine(this.GetNationFolder(nationKey), c_radioXmlFile);
        }

        public string GetChassisListFile(string nationKey)
        {
            return Path.Combine(this.GetNationFolder(nationKey), c_chassisXmlFile);
        }

        public string GetCrewDefinitionFile(string nationKey)
        {
            return Path.Combine(this.RootPath, string.Format(c_crewDefinitionFile, nationKey));
        }

        public string GetNationalTechTreeLayoutFile(string nationKey)
        {
            return UnifiedPath.Combine(this.GuiPackageFile, string.Format(@"{0}\{1}{2}", c_techTreeLayoutRelativePath, nationKey, c_techTreeLayoutFilePostfix));
        }
    }
}
