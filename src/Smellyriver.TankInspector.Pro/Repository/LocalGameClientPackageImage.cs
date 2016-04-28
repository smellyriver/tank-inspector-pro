using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.IO.XmlDecoding;
using Smellyriver.TankInspector.Pro.UserInterface;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public sealed class LocalGameClientPackageImage
    {
        private readonly LocalGameClientPath _paths;

        private readonly Dictionary<string, ImageSource> _shellIcons;
        private readonly Dictionary<string, ImageSource> _accessoryIcons;
        private readonly Dictionary<string, ImageSource> _skillIcons;
        private readonly Dictionary<string, ImageSource> _skillSmallIcons;
        private readonly Dictionary<string, ImageSource> _crewRoleIcons;
        private readonly Dictionary<string, ImageSource> _crewRoleSmallIcons;
        private readonly Dictionary<string, ImageSource> _nationSmallIcons;
        private readonly Dictionary<string, ImageSource> _tankSmallIcons;
        private readonly Dictionary<string, ImageSource> _tankBigIcons;
        private readonly Dictionary<string, ImageSource> _tankCountorIcons;


        internal LocalGameClientPackageImage(LocalGameClientPath paths)
        {
            _paths = paths;


            using (var reader = new BigworldXmlReader(_paths.GetShellListFile(_paths.Nations[0])))
            {
                var element = XElement.Load(reader);
                _shellIcons = element.Element("icons")
                                     .Elements()
                                     .ToDictionary(e => e.Name.ToString(),
                                                   e => PackageImage.Load(_paths.GuiPackageFile, "gui/maps/icons/shell/" + e.Value.Split(' ').First()));
            }

            _accessoryIcons = new Dictionary<string, ImageSource>();
            _skillIcons = new Dictionary<string, ImageSource>();
            _skillSmallIcons = new Dictionary<string, ImageSource>();
            _nationSmallIcons = new Dictionary<string, ImageSource>();

            _tankSmallIcons = new Dictionary<string, ImageSource>();
            _tankBigIcons = new Dictionary<string, ImageSource>();
            _tankCountorIcons = new Dictionary<string, ImageSource>();

            using (var reader = new BigworldXmlReader(_paths.CommonCrewDataFile))
            {
                var element = XElement.Load(reader);
                _crewRoleIcons = element.Element("roles")
                                        .Elements()
                                        .ToDictionary(e => e.Name.ToString(),
                                                      e => PackageImage.Load(_paths.GuiPackageFile, "gui/maps/icons/tankmen/roles/big/" + e.Element("icon").Value));
                _crewRoleSmallIcons = element.Element("roles")
                                             .Elements()
                                             .ToDictionary(e => e.Name.ToString(),
                                                           e => PackageImage.Load(_paths.GuiPackageFile, "gui/maps/icons/tankmen/roles/small/" + e.Element("icon").Value));
            }

        }

        public ImageSource GetShellIcon(string iconName)
        {
            if (iconName == null)
                return null;

            return _shellIcons[iconName];
        }

        public ImageSource GetAccessoryIcon(string icon)
        {
            if (icon == null)
                return null;

            return _accessoryIcons.GetOrCreate(icon,
                                               () => PackageImage.Load(_paths.GuiPackageFile, "gui" + icon.Split(' ').First().Substring(2)));
        }

        public ImageSource GetSkillIcon(string icon)
        {
            if (icon == null)
                return null;
            return _skillIcons.GetOrCreate(icon,
                                           () => PackageImage.Load(_paths.GuiPackageFile, "gui/maps/icons/tankmen/skills/big/" + icon));
        }

        public ImageSource GetSkillSmallIcon(string icon)
        {
            if (icon == null)
                return null;

            return _skillSmallIcons.GetOrCreate(icon,
                                                () => PackageImage.Load(_paths.GuiPackageFile, "gui/maps/icons/tankmen/skills/small/" + icon));
        }

        public ImageSource GetCrewRoleIcon(string iconName)
        {
            if (iconName == null)
                return null;

            return _crewRoleIcons[iconName];
        }

        public ImageSource GetCrewRoleSmallIcon(string iconName)
        {
            if (iconName == null)
                return null;

            return _crewRoleSmallIcons[iconName];
        }

        public ImageSource GetNationSmallIcon(string nationKey)
        {
            return _nationSmallIcons.GetOrCreate(nationKey,
                                                 () => PackageImage.Load(_paths.GuiPackageFile,
                                                                         string.Format("gui/maps/icons/filters/nations/{0}.png", nationKey)));
        }

        public ImageSource GetClassSmallIcon(string @class)
        {
            return _nationSmallIcons.GetOrCreate(@class,
                                                 () => PackageImage.Load(_paths.GuiPackageFile,
                                                                         string.Format("gui/maps/icons/filters/tanks/{0}.png", @class)));
        }

        private ImageSource LoadTankIcon(string relativePath, string hyphenKey)
        {
            var localPath = string.Format("{0}/{1}.png", relativePath, hyphenKey);
            if (PackageStream.IsFileExisted(_paths.GuiPackageFile, localPath))
                return PackageImage.Load(_paths.GuiPackageFile, localPath);

            localPath = string.Format("{0}/noImage.png", relativePath);
            return PackageImage.Load(_paths.GuiPackageFile, localPath);
        }

        public ImageSource GetTankSmallIcon(string hyphenKey)
        {
            return _tankSmallIcons.GetOrCreate(hyphenKey,
                                               () => this.LoadTankIcon("gui/maps/icons/vehicle/small", hyphenKey));
        }

        public ImageSource GetTankBigIcon(string hyphenKey)
        {
            return _tankBigIcons.GetOrCreate(hyphenKey,
                                             () => this.LoadTankIcon("gui/maps/icons/vehicle", hyphenKey));
        }

        public ImageSource GetTankContourIcon(string hyphenKey)
        {
            return _tankCountorIcons.GetOrCreate(hyphenKey,
                                                 () => this.LoadTankIcon("gui/maps/icons/vehicle/contour", hyphenKey));
        }
    }
}
