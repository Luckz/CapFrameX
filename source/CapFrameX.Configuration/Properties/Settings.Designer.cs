﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CapFrameX.Configuration.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.3.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("40")]
        public int MovingAverageWindowSize {
            get {
                return ((int)(this["MovingAverageWindowSize"]));
            }
            set {
                this["MovingAverageWindowSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2.5")]
        public double StutteringFactor {
            get {
                return ((double)(this["StutteringFactor"]));
            }
            set {
                this["StutteringFactor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MyDocuments\\CapFrameX\\Captures")]
        public string ObservedDirectory {
            get {
                return ((string)(this["ObservedDirectory"]));
            }
            set {
                this["ObservedDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int FpsValuesRoundingDigits {
            get {
                return ((int)(this["FpsValuesRoundingDigits"]));
            }
            set {
                this["FpsValuesRoundingDigits"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowLowParameter {
            get {
                return ((bool)(this["ShowLowParameter"]));
            }
            set {
                this["ShowLowParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MyDocuments\\CapFrameX\\Screenshots")]
        public string ScreenshotDirectory {
            get {
                return ((string)(this["ScreenshotDirectory"]));
            }
            set {
                this["ScreenshotDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseSingleRecordMaxStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordMaxStatisticParameter"]));
            }
            set {
                this["UseSingleRecordMaxStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseSingleRecordP99QuantileStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordP99QuantileStatisticParameter"]));
            }
            set {
                this["UseSingleRecordP99QuantileStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseSingleRecordP95QuantileStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordP95QuantileStatisticParameter"]));
            }
            set {
                this["UseSingleRecordP95QuantileStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseSingleRecordAverageStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordAverageStatisticParameter"]));
            }
            set {
                this["UseSingleRecordAverageStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseSingleRecordP5QuantileStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordP5QuantileStatisticParameter"]));
            }
            set {
                this["UseSingleRecordP5QuantileStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseSingleRecordP1QuantileStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordP1QuantileStatisticParameter"]));
            }
            set {
                this["UseSingleRecordP1QuantileStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseSingleRecordP0Dot1QuantileStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordP0Dot1QuantileStatisticParameter"]));
            }
            set {
                this["UseSingleRecordP0Dot1QuantileStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseSingleRecordP1LowAverageStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordP1LowAverageStatisticParameter"]));
            }
            set {
                this["UseSingleRecordP1LowAverageStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseSingleRecordP0Dot1LowAverageStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordP0Dot1LowAverageStatisticParameter"]));
            }
            set {
                this["UseSingleRecordP0Dot1LowAverageStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseSingleRecordMinStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordMinStatisticParameter"]));
            }
            set {
                this["UseSingleRecordMinStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseSingleRecordAdaptiveSTDStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordAdaptiveSTDStatisticParameter"]));
            }
            set {
                this["UseSingleRecordAdaptiveSTDStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("F12")]
        public string CaptureHotKey {
            get {
                return ((string)(this["CaptureHotKey"]));
            }
            set {
                this["CaptureHotKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("voice response")]
        public string HotkeySoundMode {
            get {
                return ((string)(this["HotkeySoundMode"]));
            }
            set {
                this["HotkeySoundMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int CaptureTime {
            get {
                return ((int)(this["CaptureTime"]));
            }
            set {
                this["CaptureTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseSingleRecordP0Dot2QuantileStatisticParameter {
            get {
                return ((bool)(this["UseSingleRecordP0Dot2QuantileStatisticParameter"]));
            }
            set {
                this["UseSingleRecordP0Dot2QuantileStatisticParameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.25")]
        public double VoiceSoundLevel {
            get {
                return ((double)(this["VoiceSoundLevel"]));
            }
            set {
                this["VoiceSoundLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.5")]
        public double SimpleSoundLevel {
            get {
                return ((double)(this["SimpleSoundLevel"]));
            }
            set {
                this["SimpleSoundLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("P1")]
        public string SecondaryMetric {
            get {
                return ((string)(this["SecondaryMetric"]));
            }
            set {
                this["SecondaryMetric"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DateTime")]
        public string ComparisonContext {
            get {
                return ((string)(this["ComparisonContext"]));
            }
            set {
                this["ComparisonContext"] = value;
            }
        }
    }
}
