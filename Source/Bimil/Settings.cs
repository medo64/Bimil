using System;
using System.ComponentModel;

namespace Bimil {
    internal static class Settings {

        [Category("Behavior")]
        [DisplayName("Close on escape")]
        [Description("If true, escape will close application.")]
        [DefaultValue(false)]
        public static bool CloseOnEscape {
            get { return Medo.Configuration.Properties.Read("CloseOnEscape", Medo.Configuration.Settings.Read("CloseOnEscape", false)); }
            set { Medo.Configuration.Properties.Write("CloseOnEscape", value); }
        }

        [Category("Behavior")]
        [DisplayName("Show start")]
        [Description("If true, Start window will be shown.")]
        [DefaultValue(true)]
        public static bool ShowStart {
            get { return Medo.Configuration.Properties.Read("ShowStart", Medo.Configuration.Settings.Read("ShowStart", true)); }
            set { Medo.Configuration.Properties.Write("ShowStart", value); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close application timeout")]
        [Description("Time in seconds for main window to automatically close if it loses focus. Value 0 disables auto-close.")]
        [DefaultValue(900)]
        public static int AutoCloseTimeout {
            get { return LimitBetween(Medo.Configuration.Properties.Read("AutoCloseTimeout", Medo.Configuration.Settings.Read("AutoCloseTimeout", 900)), minValue: 10, maxValue: 3600, allowZero: true); }
            set { Medo.Configuration.Properties.Write("AutoCloseTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close window timeout")]
        [Description("Time in seconds for item window to automatically close if it loses focus. Value 0 disables auto-close. Note that auto-close will cancel any edit in progress.")]
        [DefaultValue(120)]
        public static int AutoCloseItemTimeout {
            get { return LimitBetween(Medo.Configuration.Properties.Read("AutoCloseItemTimeout", Medo.Configuration.Settings.Read("AutoCloseItemTimeout", 120)), minValue: 10, maxValue: 3600, allowZero: true); }
            set { Medo.Configuration.Properties.Write("AutoCloseItemTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close save")]
        [Description("If true, edited items will be automatically saved when auto-close gets activated.")]
        [DefaultValue(true)]
        public static bool AutoCloseSave {
            get { return Medo.Configuration.Properties.Read("AutoCloseSave", Medo.Configuration.Settings.Read("AutoCloseSave", true)); }
            set { Medo.Configuration.Properties.Write("AutoCloseSave", value); }
        }

        [Category("Behavior")]
        [DisplayName("Editable by default")]
        [Description("If true, all fields will be editable by default.")]
        [DefaultValue(false)]
        public static bool EditableByDefault {
            get { return Medo.Configuration.Properties.Read("EditableByDefault", Medo.Configuration.Settings.Read("EditableByDefault", false)); }
            set { Medo.Configuration.Properties.Write("EditableByDefault", value); }
        }

        [Category("Behavior")]
        [DisplayName("Show common password warnings")]
        [Description("If true, warning will be shown if a password similar to common is used.")]
        [DefaultValue(false)]
        public static bool ShowCommonPasswordWarnings {
            get { return Medo.Configuration.Properties.Read("ShowCommonPasswordWarnings", Medo.Configuration.Settings.Read("ShowCommonPasswordWarnings", true)); }
            set { Medo.Configuration.Properties.Write("ShowCommonPasswordWarnings", value); }
        }


        [Category("Compatibility")]
        [DisplayName("Show PasswordSafe warnings")]
        [Description("If true, warning will be shown upon adding fields not compatible with PasswordSafe.")]
        [DefaultValue(false)]
        public static bool ShowPasswordSafeWarnings {
            get { return Medo.Configuration.Properties.Read("ShowPasswordSafeWarnings", Medo.Configuration.Settings.Read("ShowPasswordSafeWarnings", false)); }
            set { Medo.Configuration.Properties.Write("ShowPasswordSafeWarnings", value); }
        }


        [Category("Visual")]
        [DisplayName("Scale boost")]
        [Description("Additional value to determine toolbar scaling.")]
        [DefaultValue(120)]
        public static double ScaleBoost {
            get { return Medo.Configuration.Properties.Read("ScaleBoost", Medo.Configuration.Settings.Read("ScaleBoost", 0.00)); }
            set {
                if ((value < -1) || (value > 4)) { return; }
                Medo.Configuration.Properties.Write("ScaleBoost", value);
            }
        }


        #region PasswordGenerator

        [Browsable(false)]
        public static bool PasswordGeneratorUseWord {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorUseWord", Medo.Configuration.Settings.Read("PasswordGeneratorUseWord", true)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorUseWord", value); }
        }


        [Browsable(false)]
        public static bool PasswordGeneratorIncludeUpperCase {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorIncludeUpperCase", Medo.Configuration.Settings.Read("PasswordGeneratorIncludeUpperCase", true)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorIncludeUpperCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorIncludeLowerCase {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorIncludeLowerCase", Medo.Configuration.Settings.Read("PasswordGeneratorIncludeLowerCase", true)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorIncludeLowerCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorIncludeNumbers {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorIncludeNumbers", Medo.Configuration.Settings.Read("PasswordGeneratorIncludeNumbers", true)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorIncludeNumbers", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorIncludeSpecialCharacters {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorIncludeSpecialCharacters", Medo.Configuration.Settings.Read("PasswordGeneratorIncludeSpecialCharacters", true)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorIncludeSpecialCharacters", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictSimilar {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorRestrictSimilar", Medo.Configuration.Settings.Read("PasswordGeneratorRestrictSimilar", false)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorRestrictSimilar", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictMovable {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorRestrictMovable", Medo.Configuration.Settings.Read("PasswordGeneratorRestrictMovable", false)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorRestrictMovable", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictPronounceable {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorRestrictPronounceable", Medo.Configuration.Settings.Read("PasswordGeneratorRestrictPronounceable", false)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorRestrictPronounceable", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictRepeated {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorRestrictRepeated", Medo.Configuration.Settings.Read("PasswordGeneratorRestrictRepeated", false)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorRestrictRepeated", value); }
        }

        [Browsable(false)]
        public static int PasswordGeneratorLength {
            get { return LimitBetween(Medo.Configuration.Properties.Read("PasswordGeneratorLength", Medo.Configuration.Settings.Read("PasswordGeneratorLength", 14)), minValue: 4, maxValue: 99, allowZero: false); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorLength", LimitBetween(value, minValue: 1, maxValue: 99, allowZero: false)); }
        }


        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeUpperCase {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorWordIncludeUpperCase", Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeUpperCase", false)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorWordIncludeUpperCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeNumber {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorWordIncludeNumber", Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeNumber", true)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorWordIncludeNumber", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeSpecialCharacter {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorWordIncludeSpecialCharacter", Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeSpecialCharacter", true)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorWordIncludeSpecialCharacter", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeIncomplete {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorWordIncludeIncomplete", Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeIncomplete", false)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorWordIncludeIncomplete", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictAddSpace {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorWordRestrictAddSpace", Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictAddSpace", false)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorWordRestrictAddSpace", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictBreak {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorWordRestrictBreak", Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictBreak", true)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorWordRestrictBreak", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictTitleCase {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorWordRestrictTitleCase", Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictTitleCase", true)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorWordRestrictTitleCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictSuffixOnly {
            get { return Medo.Configuration.Properties.Read("PasswordGeneratorWordRestrictSuffixOnly", Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictSuffixOnly", false)); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorWordRestrictSuffixOnly", value); }
        }

        [Browsable(false)]
        public static int PasswordGeneratorWordCount {
            get { return LimitBetween(Medo.Configuration.Properties.Read("PasswordGeneratorWordCount", Medo.Configuration.Settings.Read("PasswordGeneratorWordCount", 5)), minValue: 1, maxValue: 9, allowZero: false); }
            set { Medo.Configuration.Properties.Write("PasswordGeneratorWordCount", LimitBetween(value, minValue: 1, maxValue: 9, allowZero: false)); }
        }

        #endregion


        [Browsable(false)]
        public static bool AutoTypeUseSendWait {
            get { return Medo.Configuration.Properties.Read("AutoTypeUseSendWait", Medo.Configuration.Settings.Read("AutoTypeUseSendWait", true)); }
            set { Medo.Configuration.Properties.Write("AutoTypeUseSendWait", value); }
        }

        [Browsable(false)]
        public static int AutoTypeDelay { //in milliseconds
            get { return Medo.Configuration.Properties.Read("AutoTypeDelay", Medo.Configuration.Settings.Read("AutoTypeDelay", 10)); }
            set { Medo.Configuration.Properties.Write("AutoTypeDelay", value); }
        }

        [Browsable(false)]
        public static int AutoTypeWindowOpacity {
            get { return LimitBetween(Medo.Configuration.Properties.Read("AutoTypeWindowOpacity", Medo.Configuration.Settings.Read("AutoTypeWindowOpacity", 100)), minValue: 25, maxValue: 100, allowZero: false); }
            set { Medo.Configuration.Properties.Write("AutoTypeWindowOpacity", LimitBetween(value, minValue: 25, maxValue: 100, allowZero: false)); }
        }


        private static int LimitBetween(int value, int minValue, int maxValue, bool allowZero) {
            if (allowZero && (value == 0)) { return 0; }
            if (value < minValue) { return minValue; }
            if (value > maxValue) { return maxValue; }
            return value;
        }

    }
}
