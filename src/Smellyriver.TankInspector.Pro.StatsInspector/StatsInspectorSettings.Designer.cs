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
using System.Windows.Controls;
using Smellyriver.TankInspector.Pro.Data.Stats;

namespace Smellyriver.TankInspector.Pro.StatsInspector {
    
    
    [CompilerGenerated()]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class StatsInspectorSettings : ApplicationSettingsBase {
        
        private static StatsInspectorSettings defaultInstance = ((StatsInspectorSettings)(ApplicationSettingsBase.Synchronized(new StatsInspectorSettings())));
        
        public static StatsInspectorSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("Templates/Default.xaml")]
        public string LastTemplateFilename {
            get {
                return ((string)(this["LastTemplateFilename"]));
            }
            set {
                this["LastTemplateFilename"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("Page")]
        public FlowDocumentReaderViewingMode StatDocumentViewingMode {
            get {
                return ((FlowDocumentReaderViewingMode)(this["StatDocumentViewingMode"]));
            }
            set {
                this["StatDocumentViewingMode"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("Instance")]
        public StatValueMode StatValueMode {
            get {
                return ((StatValueMode)(this["StatValueMode"]));
            }
            set {
                this["StatValueMode"] = value;
            }
        }
    }
}
