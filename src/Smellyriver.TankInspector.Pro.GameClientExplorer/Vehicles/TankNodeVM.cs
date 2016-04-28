using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;
using Smellyriver.TankInspector.IO.XmlDecoding;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem;
using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.Vehicles
{
    class TankNodeVM : VehicleNodeVMBase, IAddChild
    {
        private readonly LocalGameClient _client;
        private readonly Tank _tank;
        private readonly TankUnikey _unikey;

        public TankUnikey TankUnikey { get { return _unikey; } }

        public override ImageSource IconSource
        {
            get
            {
                return NodeIconService.Current.GetNodeIcon(
                    string.Format("{0}{1}", NodeTypes.TankClassProtocal, _tank.ClassKey));
            }
        }

        public override string Description
        {
            get { return _tank.Key; }
        }

        public TankNodeVM(NationNodeVM parent, Tank tank, LocalGameClient client)
            : base(parent, tank.Name, LoadChildenStrategy.LazyStatic)
        {
            _client = client;
            _tank = tank;
            _unikey = new TankUnikey(_client, _tank);
        }

        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();

            foreach (var command in TankCommandManager.Instance.Commands)
            {
                list.Add(new ExplorerTreeContextMenuItemVM(command.Priority - 60,
                                                           command.Name,
                                                           command,
                                                           _unikey,
                                                           command.Icon));
            }

            list.Add(new ExplorerTreeContextMenuItemVM(-50,
                                                       this.L("game_client_explorer", "copy_all_files_to_mod_folder_menu_item"),
                                                       new RelayCommand(this.CopyAllFilesToModFolder),
                                                       BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/Copy_16.png")));

            return list;
        }


        private void CopyAllFilesToModFolder()
        {
            var fileList = this.Decendants.OfType<RemotePackageFileVM>().Select(n => n.Path).ToArray();

            var modDirectory = this.GameClientRoot.Model.ModDirectory;
            PackageManager.Instance.Extract(fileList, modDirectory, OverwriteStrategy.Ask);
        }

        protected override IEnumerable<TreeNodeVM> LoadChildren()
        {

            var packageIndexer = this.GameClientRoot.Model.PackageIndexer;

            var hullNode = new VirtualFolderVM(this, this.L("game_client_explorer", "hull"));
            this.AddModelNodes(hullNode, _tank.Hull);
            yield return hullNode;

            var chassisRootNode = new VirtualFolderVM(this, this.L("game_client_explorer", "chassis"));
            foreach (var chassis in _tank.Chassis)
            {
                var chassisNode = new VirtualFolderVM(chassisRootNode, chassis.Name);
                this.AddChassisModelNodes(chassisNode, chassis);
                chassisRootNode.AddChild(chassisNode);
            }
            yield return chassisRootNode;

            var turretsRootNode = new VirtualFolderVM(this, this.L("game_client_explorer", "turrets"));
            foreach (var turret in _tank.Turrets)
            {
                var turretNode = new VirtualFolderVM(turretsRootNode, turret.Name);
                this.AddModelNodes(turretNode, turret);
                turretsRootNode.AddChild(turretNode);

                var gunsRootNode = new VirtualFolderVM(turretNode, this.L("game_client_explorer", "guns"));
                turretNode.AddChild(gunsRootNode);
                foreach (var gun in turret.Guns)
                {
                    var gunNode = new VirtualFolderVM(gunsRootNode, gun.Name);
                    this.AddModelNodes(gunNode, gun);
                    gunsRootNode.AddChild(gunNode);
                }
            }
            yield return turretsRootNode;

            var exclusiveMaskNode = this.CreateExclusiveMaskNode(this, _tank, packageIndexer);
            if (exclusiveMaskNode != null)
                yield return exclusiveMaskNode;
        }


        private static IXQueryable LoadXmlFile(string unifiedPath)
        {
            using (var reader = new BigworldXmlReader(unifiedPath))
            {
                return XElement.Load(reader).ToXQueryable(); ;
            }
        }

        private ExplorerTreeNodeVM CreateExclusiveMaskNode<TParent>(TParent parent, IXQueryable element, LocalGameClientPackageIndexer indexer)
            where TParent : ExplorerTreeNodeVM, IAddChild
        {
            var exclusiveMaskLocalFile = element["camouflage/exclusionMask"];
            if (!string.IsNullOrEmpty(exclusiveMaskLocalFile))
            {
                var camouflageNode = new VirtualFolderVM(parent, this.L("game_client_explorer", "camouflage"));

                var exclusiveMaskNode = new RemotePackageFileVM(camouflageNode,
                                                                indexer.GetPackagePath(exclusiveMaskLocalFile),
                                                                exclusiveMaskLocalFile,
                                                                this.L("game_client_explorer", "exclusive_mask"));
                camouflageNode.AddChild(exclusiveMaskNode);

                return camouflageNode;
            }
            else
                return null;
        }

        private static void AddFileNode(VirtualFolderVM parent, IXQueryable element, string name, string xpath, LocalGameClientPackageIndexer indexer)
        {
            var localPath = element[xpath];
            if (!string.IsNullOrEmpty(localPath))
            {
                var fileNode = new RemotePackageFileVM(parent, indexer.GetPackagePath(localPath), localPath, name);
                parent.AddChild(fileNode);
            }

        }

        private static void AddModelNode(VirtualFolderVM parent, string localPath, LocalGameClientPackageIndexer indexer, bool addVisualNode = true)
        {
            var modelNode = new RemotePackageFileVM(parent,
                                                    indexer.GetPackagePath(localPath),
                                                    localPath,
                                                    Localization.Instance.L("game_client_explorer", "model"));
            parent.AddChild(modelNode);

            if (addVisualNode)
            {
                var model = TankNodeVM.LoadXmlFile(indexer.GetUnifiedPath(localPath));
                string visualFile = (model.Query("nodefullVisual") ?? model.Query("nodelessVisual")).Value + ".visual";
                if (!string.IsNullOrEmpty(visualFile))
                {
                    TankNodeVM.AddVisualNode(parent, visualFile, indexer);
                }
            }

        }

        private static void AddVisualNode(VirtualFolderVM parent, string localPath, LocalGameClientPackageIndexer indexer)
        {
            var visualNode = new RemotePackageFileVM(parent,
                                                     indexer.GetPackagePath(localPath),
                                                     localPath,
                                                     Localization.Instance.L("game_client_explorer", "visual"));
            parent.AddChild(visualNode);


            var visual = TankNodeVM.LoadXmlFile(indexer.GetUnifiedPath(localPath));
            foreach (var primitiveGroup in visual.QueryMany("renderSet/geometry/primitiveGroup"))
            {
                var material = primitiveGroup.Query("material");

                var identifier = material["identifier"];
                var groupNode = new VirtualFolderVM(parent, identifier);
                parent.AddChild(groupNode);

                foreach (var property in material.QueryMany("property"))
                {
                    var propertyName = (property.ToElement().FirstNode as XText).Value;
                    string displayName;
                    switch (propertyName.Trim())
                    {
                        case "specularMap":
                            displayName = Localization.Instance.L("game_client_explorer", "specular_map");
                            break;
                        case "normalMap":
                            displayName = Localization.Instance.L("game_client_explorer", "normal_map");
                            break;
                        case "diffuseMap":
                            displayName = Localization.Instance.L("game_client_explorer", "diffuse_map");
                            break;
                        case "metallicDetailMap":
                            displayName = Localization.Instance.L("game_client_explorer", "metallic_detail_map");
                            break;
                        case "metallicGlossMap":
                            displayName = Localization.Instance.L("game_client_explorer", "metallic_gloss_map");
                            break;
                        case "excludeMaskMap":
                            displayName = Localization.Instance.L("game_client_explorer", "exclude_mask_map");
                            break;
                        default:
                            if (property.Query("Texture") != null)
                            {

                            }
                            continue;
                    }

                    var textureFile = property["Texture"];
                    if (string.IsNullOrEmpty(textureFile))
                        continue;

                    var fileNode = new RemotePackageFileVM(groupNode, indexer.GetPackagePath(textureFile), textureFile, displayName);
                    groupNode.AddChild(fileNode);

                }
            }
        }

        private void AddModelNodes(VirtualFolderVM parent, IXQueryable element)
        {
            var packageIndexer = this.GameClientRoot.Model.PackageIndexer;

            this.AddModelDirectory(parent, 
                                   element, 
                                   this.L("game_client_explorer", "undamaged"), 
                                   "models/undamaged", 
                                   packageIndexer);
            this.AddModelDirectory(parent,
                                   element,
                                   this.L("game_client_explorer", "destroyed"), 
                                   "models/destroyed", 
                                   packageIndexer);
            this.AddModelDirectory(parent,
                                   element, 
                                   this.L("game_client_explorer", "exploded"), 
                                   "models/exploded", 
                                   packageIndexer);
            this.AddModelDirectory(parent,
                                   element,
                                   this.L("game_client_explorer", "collision"), 
                                   "hitTester/collisionModel", 
                                   packageIndexer, 
                                   false);
            var exclusiveMaskNode = this.CreateExclusiveMaskNode(parent, element, packageIndexer);
            if (exclusiveMaskNode != null)
                parent.AddChild(exclusiveMaskNode);
        }

        private void AddChassisModelNodes(VirtualFolderVM parent, IXQueryable element)
        {
            var packageIndexer = this.GameClientRoot.Model.PackageIndexer;

            this.AddModelNodes(parent, element);
            var splineDescElement = element.Query("splineDesc");
            if (splineDescElement != null)
            {
                var splineNode = new VirtualFolderVM(parent, 
                                                     this.L("game_client_explorer", "spline"));
                parent.AddChild(splineNode);

                TankNodeVM.AddFileNode(splineNode, 
                            splineDescElement, 
                            this.L("game_client_explorer","left_track"), 
                            "left",
                            packageIndexer);
                TankNodeVM.AddFileNode(splineNode, 
                            splineDescElement, 
                            this.L("game_client_explorer","right_track"),
                            "right", 
                            packageIndexer);
                this.AddModelDirectory(splineNode, 
                                       splineDescElement, 
                                       this.L("game_client_explorer","left_segment"), 
                                       "segmentModelLeft", 
                                       packageIndexer);
                this.AddModelDirectory(splineNode,
                                       splineDescElement, 
                                       this.L("game_client_explorer","right_segment"), 
                                       "segmentModelRight",
                                       packageIndexer);
                this.AddModelDirectory(splineNode,
                                       splineDescElement, 
                                       this.L("game_client_explorer","left_segment2"),
                                       "segment2ModelLeft",
                                       packageIndexer);
                this.AddModelDirectory(splineNode,
                                       splineDescElement,
                                       this.L("game_client_explorer", "right_segment2"),
                                       "segment2ModelRight",
                                       packageIndexer);
            }
        }

        private void AddModelDirectory(VirtualFolderVM parent,
                                       IXQueryable element,
                                       string directoryName,
                                       string xpath,
                                       LocalGameClientPackageIndexer indexer,
                                       bool addVisualNode = true)
        {
            var localPath = element[xpath];
            if (!string.IsNullOrWhiteSpace(localPath))
            {
                var node = new VirtualFolderVM(parent, directoryName);
                parent.AddChild(node);
                TankNodeVM.AddModelNode(node, localPath, indexer, addVisualNode);
            }
        }


        void IAddChild.AddChild(ExplorerTreeNodeVM child)
        {
            this.InternalChildren.Add(child);
        }
    }
}
