using System;
using System.Collections.Generic;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Media;
using Smellyriver.TankInspector.Pro.Modularity.Modules;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    class NodeIconService
    {
        public static NodeIconService Current { get; private set; }

        static NodeIconService()
        {
            NodeIconService.Current = new NodeIconService();
        }

        private static readonly Dictionary<string, string> s_normalFileIconMapping
            = new Dictionary<string, string>
            {
                { NodeTypes.Folder , "Folder_16.png" },
                { NodeTypes.GameClient, "WoT_16.png" },
                { NodeTypes.RootFolder, "RootFolder_16.png" },
                { NodeTypes.DataFolder, "Data_16.png" },
                { NodeTypes.XML, "XML_16.png" },
                { NodeTypes.VehiclesFolder, "Vehicles_16.png" },
                { NodeTypes.ViewTank, "ViewTank_16.png" },
                { NodeTypes.ViewNationalTechTree, "TechTree_16.png" },
                { NodeTypes.ViewComponentTechTree, "TechTree_16.png" }
            };

        private static readonly Dictionary<string, string> s_openFileIconMapping
            = new Dictionary<string, string>
            {
                { NodeTypes.Folder, "Folder_Open_16.png" },
            };
        
        private NodeIconService()
        {
            
        }

        public ImageSource GetNodeIcon(string nodeType, bool isOpen = false)
        {
            string imageFileName = null;
            if (nodeType.StartsWith(NodeTypes.FileProtocal))
            {
                var extensionName = nodeType.Substring(NodeTypes.FileProtocal.Length);

                if (extensionName == "pkg")
                    imageFileName = "Package_16.png";
                else
                {
                    var iconSource = FileDocumentService.Instance.GetIconSource(extensionName);
                    if (iconSource == null)
                        imageFileName = "File_16.png";
                    else
                        return iconSource;
                }
            }
            else if(nodeType.StartsWith(NodeTypes.TankClassProtocal))
            {
                var tankClass = nodeType.Substring(NodeTypes.TankClassProtocal.Length);
                return ApplicationImages.TryGetTankClassIcon(tankClass);
            }
            else
            {
                if (isOpen)
                    s_openFileIconMapping.TryGetValue(nodeType, out imageFileName);

                if (imageFileName == null)
                    s_normalFileIconMapping.TryGetValue(nodeType, out imageFileName);

                if (imageFileName == null)
                    imageFileName = "Unknown_16.png";
            }

            try
            {
                return BitmapImageEx.LoadAsFrozen(string.Format("Resources/Images/NodeIcons/{0}", imageFileName));
            }
            catch(Exception)
            {
                return BitmapImageEx.LoadAsFrozen("Resources/Images/NodeIcons/Unknown_16.png");
            }
        }
    }
}
