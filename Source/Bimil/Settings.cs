using System;
using System.ComponentModel;

namespace Bimil {
    internal static class Settings {

        [Category("Behavior")]
        [DisplayName("Close on escape")]
        [Description("If true, escape will close application.")]
        [DefaultValue(false)]
        public static bool CloseOnEscape {
            get { return Medo.Configuration.Settings.Read("CloseOnEscape", false); }
            set { Medo.Configuration.Settings.Write("CloseOnEscape", value); }
        }

        [Category("Behavior")]
        [DisplayName("Show start")]
        [Description("If true, Start window will be shown.")]
        [DefaultValue(true)]
        public static bool ShowStart {
            get { return Medo.Configuration.Settings.Read("ShowStart", true); }
            set { Medo.Configuration.Settings.Write("ShowStart", value); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close application timeout")]
        [Description("Time in seconds for main window to automatically close if it loses focus. Value 0 disables auto-close.")]
        [DefaultValue(900)]
        public static int AutoCloseTimeout {
            get { return LimitBetween(Medo.Configuration.Settings.Read("AutoCloseTimeout", 900), minValue: 10, maxValue: 3600, allowZero: true); }
            set { Medo.Configuration.Settings.Write("AutoCloseTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close window timeout")]
        [Description("Time in seconds for item window to automatically close if it loses focus. Value 0 disables auto-close. Note that auto-close will cancel any edit in progress.")]
        [DefaultValue(120)]
        public static int AutoCloseItemTimeout {
            get { return LimitBetween(Medo.Configuration.Settings.Read("AutoCloseItemTimeout", 120), minValue: 10, maxValue: 3600, allowZero: true); }
            set { Medo.Configuration.Settings.Write("AutoCloseItemTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close save")]
        [Description("If true, edited items will be automatically saved when auto-close gets activated.")]
        [DefaultValue(true)]
        public static bool AutoCloseSave {
            get { return Medo.Configuration.Settings.Read("AutoCloseSave", true); }
            set { Medo.Configuration.Settings.Write("AutoCloseSave", value); }
        }

        [Category("Behavior")]
        [DisplayName("Editable by default")]
        [Description("If true, all fields will be editable by default.")]
        [DefaultValue(false)]
        public static bool EditableByDefault {
            get { return Medo.Configuration.Settings.Read("EditableByDefault", false); }
            set { Medo.Configuration.Settings.Write("EditableByDefault", value); }
        }


        [Category("Compatibility")]
        [DisplayName("Show PasswordSafe warnings")]
        [Description("If true, warning will be shown upon adding fields not compatible with PasswordSafe.")]
        [DefaultValue(false)]
        public static bool ShowPasswordSafeWarnings {
            get { return Medo.Configuration.Settings.Read("ShowPasswordSafeWarnings", false); }
            set { Medo.Configuration.Settings.Write("ShowPasswordSafeWarnings", value); }
        }


        [Category("Visual")]
        [DisplayName("Scale boost")]
        [Description("Additional value to determine toolbar scaling.")]
        [DefaultValue(120)]
        public static double ScaleBoost {
            get { return Medo.Configuration.Settings.Read("ScaleBoost", 0.00); }
            set {
                if ((value < -1) || (value > 4)) { return; }
                Medo.Configuration.Settings.Write("ScaleBoost", value);
            }
        }


        #region PasswordGenerator

        [Browsable(false)]
        public static bool PasswordGeneratorUseWord {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorUseWord", true); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorUseWord", value); }
        }


        [Browsable(false)]
        public static bool PasswordGeneratorIncludeUpperCase {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorIncludeUpperCase", true); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorIncludeUpperCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorIncludeLowerCase {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorIncludeLowerCase", true); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorIncludeLowerCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorIncludeNumbers {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorIncludeNumbers", true); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorIncludeNumbers", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorIncludeSpecialCharacters {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorIncludeSpecialCharacters", true); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorIncludeSpecialCharacters", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictSimilar {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorRestrictSimilar", false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorRestrictSimilar", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictMovable {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorRestrictMovable", false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorRestrictMovable", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictPronounceable {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorRestrictPronounceable", false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorRestrictPronounceable", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorRestrictRepeated {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorRestrictRepeated", false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorRestrictRepeated", value); }
        }

        [Browsable(false)]
        public static int PasswordGeneratorLength {
            get { return LimitBetween(Medo.Configuration.Settings.Read("PasswordGeneratorLength", 12), minValue: 4, maxValue: 99, allowZero: false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorLength", LimitBetween(value, minValue: 1, maxValue: 99, allowZero: false)); }
        }


        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeUpperCase {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeUpperCase", false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorWordIncludeUpperCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeNumber {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeNumber", true); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorWordIncludeNumber", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeSpecialCharacter {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeSpecialCharacter", true); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorWordIncludeSpecialCharacter", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordIncludeIncomplete {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorWordIncludeIncomplete", false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorWordIncludeIncomplete", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictAddSpace {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictAddSpace", false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorWordRestrictAddSpace", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictBreak {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictBreak", true); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorWordRestrictBreak", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictTitleCase {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictTitleCase", true); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorWordRestrictTitleCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorWordRestrictSuffixOnly {
            get { return Medo.Configuration.Settings.Read("PasswordGeneratorWordRestrictSuffixOnly", false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorWordRestrictSuffixOnly", value); }
        }

        [Browsable(false)]
        public static int PasswordGeneratorWordCount {
            get { return LimitBetween(Medo.Configuration.Settings.Read("PasswordGeneratorWordCount", 4), minValue: 1, maxValue: 9, allowZero: false); }
            set { Medo.Configuration.Settings.Write("PasswordGeneratorWordCount", LimitBetween(value, minValue: 1, maxValue: 9, allowZero: false)); }
        }

        #endregion


        [Browsable(false)]
        public static string AutoTypeSuffixKeys {
            get { return Medo.Configuration.Settings.Read("AutoTypeSuffixKeys", "{Tab}"); }
            set { Medo.Configuration.Settings.Write("AutoTypeSuffixKeys", value); }
        }

        [Browsable(false)]
        public static bool AutoTypeUseClipboard {
            get { return Medo.Configuration.Settings.Read("AutoTypeUseClipboard", false); }
            set { Medo.Configuration.Settings.Write("AutoTypeUseClipboard", value); }
        }

        [Browsable(false)]
        public static bool AutoTypeUseSendWait {
            get { return Medo.Configuration.Settings.Read("AutoTypeUseSendWait", true); }
            set { Medo.Configuration.Settings.Write("AutoTypeUseSendWait", value); }
        }

        [Browsable(false)]
        public static int AutoTypeDelay { //in milliseconds
            get { return Medo.Configuration.Settings.Read("AutoTypeDelay", 10); }
            set { Medo.Configuration.Settings.Write("AutoTypeDelay", value); }
        }


        private static int LimitBetween(int value, int minValue, int maxValue, bool allowZero) {
            if (allowZero && (value == 0)) { return 0; }
            if (value < minValue) { return minValue; }
            if (value > maxValue) { return maxValue; }
            return value;
        }

    }
}
