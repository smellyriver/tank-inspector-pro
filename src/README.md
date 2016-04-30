This is the source code folder.

The source code is organized in a Visual Studio 2015 solution. You will need .NET Framework 4.0 and DirectX 9.0 installed on your computer. All other referenced third-party libraries can be located under the Libraries folder. Generally, you can clone the source code and build with the Smellyriver.TankInspector.Pro.sln file.

## Libraries
This folder contains all third-party libraries (binary only) used by this project. All their licences can be found at *Smellyriver.TankInspector.Pro/Resources/Documents/LegalNotices.xaml*.

## Smellyriver.TankInspector.Common.Wpf
A common library project for WPF utilities, controls etc. shared among projects.

## Smellyriver.TankInspector.Common
A common library project for utilities shared among projects.

## Smellyriver.TankInspector.Core
The core project providing basic WoT game data representations. 
This project is shared among most all Tank Inspector programs, including the standard version (Tank Inspector) and the mobile version (Tank Inspector Mobile) which we are working on.
This project maintains its own common classes, instead of referencing *Smellyriver.TankInspector.Common*, to keep a good independencity.

## Smellyriver.TankInspector.IO
Provide support for package file reading, Big World XML decoding and GetText decoding.
This project is shared among most all Tank Inspector programs, including the standard version (Tank Inspector) and the mobile version (Tank Inspector Mobile) which we are working on.

## Smellyriver.TankInspector.Pro.ArmorInspector
The Armor Inspector module, provides armor schema view feature for tanks.

## Smellyriver.TankInspector.Pro.CameraController
The Camera Controller module, features the Camera Controller panel.

## Smellyriver.TankInspector.Pro.ConfiguratorShared
Including shared infrastructures for all the tank configuration modules, like the Crew Configurator.

## Smellyriver.TankInspector.Pro.GameClientExplorer
The Game Client Explorer module, features the Game Client Explorer panel.

## Smellyriver.TankInspector.Pro.Globalization
Provide globalization and localization support for all modules.

## Smellyriver.TankInspector.Pro.Graphics
Contains the model/texture decoder and renderer.

## Smellyriver.TankInspector.Pro.InteractiveConsole
The Interactive Console module, features a python console for automations.

## Smellyriver.TankInspector.Pro.ModelInspector
The Model Inspector module, provides model view feature for tanks.

## Smellyriver.TankInspector.Pro.ModelShared
Including shared infrastructures for model view modules, like the Armor Inspector.

## Smellyriver.TankInspector.Pro.PatchnoteGenerator
The Patchnote Generator module, provides game client version diff and patchnote generation feature.

## Smellyriver.TankInspector.Pro.StatChangesView
Including base infrastructures for modules which could reflect tank stat changes (i.e. crew changes, equipment changes etc.).

## Smellyriver.TankInspector.Pro.StatComparer
The Stat Comparer module, featuring stat comparison among multiple tanks.

## Smellyriver.TankInspector.Pro.StatsInspector
The Stats Inspector module, provides tank stats inspection feature.

## Smellyriver.TankInspector.Pro.StatsShared
Including base infrastructures for modules concerned about tank stats.

## Smellyriver.TankInspector.Pro.TankConfigurator
The Tank Configurator module, featuring the Tank Configurator panel, which allows users to change tank modules, ammunition, equipment and consumables.

## Smellyriver.TankInspector.Pro.TankModuleShared
Including base infrastructures for modules concerned about tank modules.

## Smellyriver.TankInspector.Pro.TankModuleTree
The Tank Module Tree module, provides module upgrade tree feature for tanks.

## Smellyriver.TankInspector.Pro.TankMuseum
The Tank Museum module, featuring the the Tank Museum panel, which allows users to find tanks more easily.

## Smellyriver.TankInspector.Pro.TechTree
The Tech Tree module, shows national tank tree.

## Smellyriver.TankInspector.Pro.Themes.Default
Default visual theme of Tank Inspector PRO.

## Smellyriver.TankInspector.Pro.TurretController
The Turret Controller module, provides the Turret Controller panel.

## Smellyriver.TankInspector.Pro.XmlTools
The XML Tools module, provides the XML Tools panel, which allows users to query XML data with XPath, and format and export XML data with XSLT.

## Smellyriver.TankInspector.Pro.XmlViewerService
A simple XML Viewer service provider.

