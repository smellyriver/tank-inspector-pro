using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.IO.XmlDecoding;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public class LocalGameClient : IRepository
    {
        public string ID
        {
            get { return this.RootPath; }
        }

        public string Name
        {
            get { return this.Version.ToString(); }
        }

        public string Description
        {
            get { return this.L("game_client_manager", "game_client_description", this.Version, this.RootPath); }
        }

        public string Language { get; }

        string IRepository.Path
        {
            get { return this.RootPath; }
        }

        public GameVersion Version { get { return _paths.Version; } }
        public string[] Nations { get { return _paths.Nations; } }
        public string ModDirectory
        {
            get { return _configuration.ModDirectory; }
            set { _configuration.ModDirectory = value; }
        }
        public string RootPath { get; }

        private IXQueryable _tankDatabase;
        private IXQueryable _equipmentDatabase;
        private IXQueryable _consumableDatabase;
        private IXQueryable _crewDefinitionDatabase;
        private IXQueryable _techTreeLayoutDatabase;
        private IXQueryable _customizationDatabase;

        public IXQueryable TankDatabase { get { return _tankDatabase; } }
        public IXQueryable EquipmentDatabase { get { return _equipmentDatabase; } }
        public IXQueryable ConsumableDatabase { get { return _consumableDatabase; } }
        public IXQueryable CrewDatabase { get { return _crewDefinitionDatabase; } }
        public IXQueryable TechTreeLayoutDatabase { get { return _techTreeLayoutDatabase; } }
        public IXQueryable CustomizationDatabase { get { return _customizationDatabase; } }

        public LocalGameClientLocalization Localization { get; }
        IRepositoryLocalization IRepository.Localization
        {
            get { return this.Localization; }
        }

        private readonly LocalGameClientPath _paths;
        public LocalGameClientPath Paths { get { return _paths; } }
        public LocalGameClientPackageIndexer PackageIndexer { get; private set; }
        public LocalGameClientPackageImage PackageImages { get; private set; }

        private LocalGameClientConfiguration _configuration;

        public LocalGameClientCacheManager CacheManager { get; }

        public ObservableCollection<string> ClientPaths { get; private set; }

        internal LocalGameClient(string rootPath, IProgressScope progress)
        {
            this.RootPath = rootPath;
            _paths = new LocalGameClientPath(this);
            this.ClientPaths = _paths.ClientPaths;

            this.CacheManager = new LocalGameClientCacheManager(this);

            this.Localization = new LocalGameClientLocalization(_paths.TextSettingsFile, _paths.TextFolder);
            this.Language = this.Localization.Language;

            this.PackageIndexer = new LocalGameClientPackageIndexer(this);
            this.PackageImages = new LocalGameClientPackageImage(_paths);

            this.LoadConfig();
            this.LoadXml();

            Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            this.SaveConfig();
            this.CacheManager.UpdateCacheVersionstamp();
        }

        private void SaveConfig()
        {
            LocalGameClientConfiguration.Save(this, _configuration);
        }

        private void LoadConfig()
        {
            _configuration = LocalGameClientConfiguration.Load(this);
            if (string.IsNullOrEmpty(_configuration.ModDirectory))
                _configuration.ModDirectory = _paths.DefaultModDirectory;
        }

        private void LoadXml()
        {
            var xmlPreprocessor = new BigworldXmlPreprocessor(_paths, this.Localization);
            _tankDatabase = this.CacheManager.LoadXml("tanks.xml",
                                                      xmlPreprocessor.BuildTankDatabase).ToXQueryable();
            _consumableDatabase = this.CacheManager.LoadXml("consumables.xml",
                                                            xmlPreprocessor.ProcessConsumableDataFile).ToXQueryable();
            _equipmentDatabase = this.CacheManager.LoadXml("equipment.xml",
                                                           xmlPreprocessor.ProcessEquipmentDataFile).ToXQueryable();
            _crewDefinitionDatabase = this.CacheManager.LoadXml("crewDefinition.xml",
                                                                xmlPreprocessor.BuildCrewDatabase).ToXQueryable();
            _techTreeLayoutDatabase = this.CacheManager.LoadXml("techTreeLayout.xml",
                                                                xmlPreprocessor.BuildTechTreeLayoutDatabase).ToXQueryable();
            _customizationDatabase = this.CacheManager.LoadXml("customizations.xml",
                                                                xmlPreprocessor.BuildCustomizationDatabase).ToXQueryable();
        }

        public string GetCorrespondedModPath(string unifiedPath)
        {
            string packagePath, localPath;
            UnifiedPath.ParsePath(unifiedPath, out packagePath, out localPath);
            if (string.IsNullOrEmpty(packagePath))
                throw new ArgumentException("this is not a package path", nameof(unifiedPath));

            packagePath = PathEx.NormalizeDirectorySeparators(packagePath);

            var pathDirectory = Path.GetDirectoryName(packagePath);
            var resPackagePath = Path.Combine("res", "packages");

            if (!pathDirectory.ToLower().EndsWith(resPackagePath))
                throw new ArgumentException("package is located in an invalid localtion", nameof(unifiedPath));

            return Path.Combine(this.ModDirectory, localPath);
        }

        public IXQueryable GetTank(string nationKey, string tankKey)
        {
            return this.TankDatabase
                       .Query("tank[@fullKey='{0}']", RepositoryHelper.GetTankFullKeyEscaped(nationKey, tankKey));
        }

    }
}
