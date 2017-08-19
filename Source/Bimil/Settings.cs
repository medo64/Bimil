using System;
using System.ComponentModel;
using Medo.Configuration;

namespace Bimil {
    internal static class Settings {

        [Category("Behavior")]
        [DisplayName("Close on escape")]
        [Description("If true, escape will close application.")]
        [DefaultValue(false)]
        public static bool CloseOnEscape {
            get { return Config.Read("CloseOnEscape", Medo.Configuration.Settings.Read("CloseOnEscape", false)); }
            set { Config.Write("CloseOnEscape", value); }
        }

        [Category("Behavior")]
        [DisplayName("Show start")]
        [Description("If true, Start window will be shown.")]
        [DefaultValue(true)]
        public static bool ShowStart {
            get { return Config.Read("ShowStart", Medo.Configuration.Settings.Read("ShowStart", true)); }
            set { Config.Write("ShowStart", value); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close application timeout")]
        [Description("Time in seconds for main window to automatically close if it loses focus. Value 0 disables auto-close.")]
        [DefaultValue(900)]
        public static int AutoCloseTimeout {
            get { return LimitBetween(Config.Read("AutoCloseTimeout", Medo.Configuration.Settings.Read("AutoCloseTimeout", 900)), minValue: 10, maxValue: 3600, allowZero: true); }
            set { Config.Write("AutoCloseTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close window timeout")]
        [Description("Time in seconds for item window to automatically close if it loses focus. Value 0 disables auto-close. Note that auto-close will cancel any edit in progress.")]
        [DefaultValue(120)]
        public static int AutoCloseItemTimeout {
            get { return LimitBetween(Config.Read("AutoCloseItemTimeout", Medo.Configuration.Settings.Read("AutoCloseItemTimeout", 120)), minValue: 10, maxValue: 3600, allowZero: true); }
            set { Config.Write("AutoCloseItemTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close save")]
        [Description("If true, edited items will be automatically saved when auto-close gets activated.")]
        [DefaultValue(true)]
        public static bool AutoCloseSave {
            get { return Config.Read("AutoCloseSave", Medo.Configuration.Settings.Read("AutoCloseSave", true)); }
            set { Config.Write("AutoCloseSave", value); }
        }

        [Category("Behavior")]
        [DisplayName("Editable by default")]
        [Description("If true, all fields will be editable by default.")]
        [DefaultValue(true)]
        public static bool EditableByDefault {
            get { return Config.Read("EditableByDefault", Medo.Configuration.Settings.Read("EditableByDefault", true)); }
            set { Config.Write("EditableByDefault", value); }
        }

        [Category("Behavior")]
        [DisplayName("Show common password warnings")]
        [Description("If true, warning will be shown if a password similar to common is used.")]
        [DefaultValue(true)]
        public static bool ShowCommonPasswordWarnings {
            get { return Config.Read("ShowCommonPasswordWarnings", Medo.Configuration.Settings.Read("ShowCommonPasswordWarnings", true)); }
            set { Config.Write("ShowCommonPasswordWarnings", value); }
        }


        [Category("Compatibility")]
        [DisplayName("Show PasswordSafe warnings")]
        [Description("If true, warning will be shown upon adding fields not compatible with PasswordSafe.")]
        [DefaultValue(false)]
        public static bool ShowPasswordSafeWarnings {
            get { return Config.Read("ShowPasswordSafeWarnings", Medo.Configuration.Settings.Read("ShowPasswordSafeWarnings", false)); }
            set { Config.Write("ShowPasswordSafeWarnings", value); }
        }


        [Category("Visual")]
        [DisplayName("Scale boost")]
        [Description("Additional value to determine toolbar scaling.")]
        [DefaultValue(0)]
        public static double ScaleBoost {
            get { return Config.Read("ScaleBoost", Medo.Configuration.Settings.Read("ScaleBoost", 0.00)); }
            set {
                if ((value < -1) || (value > 4)) { return; }
                Config.Write("ScaleBoost", value);
            }
        }


        #region PasswordGenerator

        [Browsable(false)]
        public static bool PasswordGeneratorUseWord {
            get { return Config.Read("PasswordGeneratorUseWord", Medo.Configuration.Settings.Read("PasswordGeneratorUseWord", true)); }
            set { Config.Write("PasswordGeneratorUseWord", value); }
        }


        [Browsable(false)]
        public static bool PasswordGeneratorIncludeUpperCase {
            get { return Config.Read("PasswordGeneratorIncludeUpperCase", Medo.Configuration.Settings.Read("PasswordGeneratorIncludeUpperCase", true)); }
            set { Config.Write("PasswordGeneratorIncludeUpperCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorIncludeLowerCase {
            get { return Config.Read("PasswordGeneratorIncludeLowerCase", Medo.Configuration.Settings.Read("PasswordGeneratorIncludeLowerCase", true)); }
            set { Config.Write("PasswordGeneratorIncludeLowerCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorIncludeNumbers {
            get { return Config.Read("PasswordGeneratorIncludeNumbers", Medo.Configuration.Settings.Read("PasswordGeneratorIncludeNumbers", true)); }
            set { Config.Write("PasswordGeneratorIncludeNumbers", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorIncludeSpecialCharacters {
            get { return Config.Read("PasswordGeneratorIncludeSpecialCharacters", Medo.Configuration.Settings.Read("PasswordGeneratorIncludeSpecialCharacters", true)); }
            set { Config.Write("PasswordGeneratorIncludeSpecialCharacters", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictSimilar {
            get { return Config.Read("PasswordGeneratorRestrictSimilar", Medo.Configuration.Settings.Read("PasswordGeneratorRestrictSimilar", false)); }
            set { Config.Write("PasswordGeneratorRestrictSimilar", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictMovable {
            get { return Config.Read("PasswordGeneratorRestrictMovable", Medo.Configuration.Settings.Read("PasswordGeneratorRestrictMovable", false)); }
            set { Config.Write("PasswordGeneratorRestrictMovable", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictPronounceable {
            get { return Config.Read("PasswordGeneratorRestrictPronounceable", Medo.Configuration.Settings.Read("PasswordGeneratorRestrictPronounceable", false)); }
            set { Config.Write("PasswordGeneratorRestrictPronounceable", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictRepeated {
            get { return Config.Read("PasswordGeneratorRestrictRepeated", Medo.Configuration.Settings.Read("PasswordGeneratorRestrictRepeated", false)); }
            set { Config.Write("PasswordGeneratorRestrictRepeated", value); }
        }

        [Browsable(false)]
        public static int PasswordGeneratorLength {
            get { return LimitBetween(Config.Read("PasswordGeneratorLength", Medo.Configuration.Settings.Read("PasswordGeneratorLength", 14)), minValue: 4, maxValue: 99, allowZero: false); }
            set { Config.Write("PasswordGeneratorLength", LimitBetween(value, minValue: 1, maxValue: 99, allowZero: false)); }
        }


        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeUpperCase {
            get { return Config.Read("PasswordGeneratorWordIncludeUpperCase", Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeUpperCase", false)); }
            set { Config.Write("PasswordGeneratorWordIncludeUpperCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeNumber {
            get { return Config.Read("PasswordGeneratorWordIncludeNumber", Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeNumber", true)); }
            set { Config.Write("PasswordGeneratorWordIncludeNumber", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeSpecialCharacter {
            get { return Config.Read("PasswordGeneratorWordIncludeSpecialCharacter", Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeSpecialCharacter", true)); }
            set { Config.Write("PasswordGeneratorWordIncludeSpecialCharacter", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeIncomplete {
            get { return Config.Read("PasswordGeneratorWordIncludeIncomplete", Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeIncomplete", false)); }
            set { Config.Write("PasswordGeneratorWordIncludeIncomplete", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictAddSpace {
            get { return Config.Read("PasswordGeneratorWordRestrictAddSpace", Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictAddSpace", false)); }
            set { Config.Write("PasswordGeneratorWordRestrictAddSpace", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictBreak {
            get { return Config.Read("PasswordGeneratorWordRestrictBreak", Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictBreak", true)); }
            set { Config.Write("PasswordGeneratorWordRestrictBreak", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictTitleCase {
            get { return Config.Read("PasswordGeneratorWordRestrictTitleCase", Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictTitleCase", true)); }
            set { Config.Write("PasswordGeneratorWordRestrictTitleCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictSuffixOnly {
            get { return Config.Read("PasswordGeneratorWordRestrictSuffixOnly", Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictSuffixOnly", false)); }
            set { Config.Write("PasswordGeneratorWordRestrictSuffixOnly", value); }
        }

        [Browsable(false)]
        public static int PasswordGeneratorWordCount {
            get { return LimitBetween(Config.Read("PasswordGeneratorWordCount", Medo.Configuration.Settings.Read("PasswordGeneratorWordCount", 5)), minValue: 1, maxValue: 9, allowZero: false); }
            set { Config.Write("PasswordGeneratorWordCount", LimitBetween(value, minValue: 1, maxValue: 9, allowZero: false)); }
        }

        #endregion


        [Browsable(false)]
        public static bool AutoTypeUseSendWait {
            get { return Config.Read("AutoTypeUseSendWait", Medo.Configuration.Settings.Read("AutoTypeUseSendWait", true)); }
            set { Config.Write("AutoTypeUseSendWait", value); }
        }

        [Browsable(false)]
        public static int AutoTypeDelay { //in milliseconds
            get { return Config.Read("AutoTypeDelay", Medo.Configuration.Settings.Read("AutoTypeDelay", 10)); }
            set { Config.Write("AutoTypeDelay", value); }
        }

        [Browsable(false)]
        public static int AutoTypeWindowOpacity {
            get { return LimitBetween(Config.Read("AutoTypeWindowOpacity", Medo.Configuration.Settings.Read("AutoTypeWindowOpacity", 100)), minValue: 25, maxValue: 100, allowZero: false); }
            set { Config.Write("AutoTypeWindowOpacity", LimitBetween(value, minValue: 25, maxValue: 100, allowZero: false)); }
        }


        private static int LimitBetween(int value, int minValue, int maxValue, bool allowZero) {
            if (allowZero && (value == 0)) { return 0; }
            if (value < minValue) { return minValue; }
            if (value > maxValue) { return maxValue; }
            return value;
        }

    }
}
