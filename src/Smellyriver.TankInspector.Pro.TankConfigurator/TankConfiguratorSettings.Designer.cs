﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Smellyriver.TankInspector.Pro.TankConfigurator {
    
    
    [CompilerGenerated()]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class TankConfiguratorSettings : ApplicationSettingsBase {
        
        private static TankConfiguratorSettings defaultInstance = ((TankConfiguratorSettings)(ApplicationSettingsBase.Synchronized(new TankConfiguratorSettings())));
        
        public static TankConfiguratorSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("True")]
        public bool ModulesExpanded {
            get {
                return ((bool)(this["ModulesExpanded"]));
            }
            set {
                this["ModulesExpanded"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("True")]
        public bool AmmunitionExpanded {
            get {
                return ((bool)(this["AmmunitionExpanded"]));
            }
            set {
                this["AmmunitionExpanded"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("False")]
        public bool EquipmentsExpanded {
            get {
                return ((bool)(this["EquipmentsExpanded"]));
            }
            set {
                this["EquipmentsExpanded"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("False")]
        public bool ConsumablesExpanded {
            get {
                return ((bool)(this["ConsumablesExpanded"]));
            }
            set {
                this["ConsumablesExpanded"] = value;
            }
        }
    }
}
