using System;
using System.ComponentModel;
using System.Globalization;
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
            set {
                Config.Write("ShowStart", value);
                if ((value == true) && LoadLast) { LoadLast = false; }
            }
        }

        [Category("Behavior")]
        [DisplayName("Load last")]
        [Description("If true, the last used file will be loaded.")]
        [DefaultValue(false)]
        public static bool LoadLast {
            get { return Config.Read("LoadLast", Medo.Configuration.Settings.Read("LoadLast", false)) && !ShowStart; }
            set {
                Config.Write("LoadLast", value);
                if ((value == true) && ShowStart) { ShowStart = false; }
            }
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
        [DisplayName("Auto-clear clipboard")]
        [Description("Time in seconds for clipboard to clear.")]
        [DefaultValue(30)]
        public static int AutoClearClipboardTimeout {
            get { return LimitBetween(Config.Read("AutoClearClipboardTimeout", Medo.Configuration.Settings.Read("AutoClearClipboardTimeout", 30)), minValue: 10, maxValue: 3600, allowZero: true); }
            set { Config.Write("AutoClearClipboardTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-clear after paste")]
        [Description("If true, data will be removed from clipboard after paste. Valid only if also clipboard timeout is set.")]
        [DefaultValue(true)]
        public static bool AutoClearClipboardAfterPaste {
            get { return Config.Read("AutoClearClipboardAfterPaste", true); }
            set { Config.Write("AutoClearClipboardAfterPaste", value); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-clear only sensitive data")]
        [Description("If true, only sensitive data (such as password or two factor keys) is to be auto-clear from clipboard.")]
        [DefaultValue(false)]
        public static bool AutoClearClipboardForSensitiveDataOnly {
            get { return Config.Read("AutoClearClipboardForSensitiveDataOnly", false); }
            set { Config.Write("AutoClearClipboardForSensitiveDataOnly", value); }
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

        [Category("Behavior")]
        [DisplayName("Save password history by default")]
        [Description("If true, password history storage will automatically be added to all new entries.")]
        [DefaultValue(false)]
        public static bool SavePasswordHistoryByDefault {
            get { return Config.Read("SavePasswordHistory", Medo.Configuration.Settings.Read("SavePasswordHistory", false)); }
            set { Config.Write("SavePasswordHistory", value); }
        }

        [Category("Behavior")]
        [DisplayName("Default password history count")]
        [Description("Default number of password history entries.")]
        [DefaultValue(5)]
        public static int SavePasswordHistoryDefaultCount {
            get { return Config.Read("SavePasswordHistoryDefaultCount", Medo.Configuration.Settings.Read("SavePasswordHistoryDefaultCount", 5)); }
            set { Config.Write("SavePasswordHistoryDefaultCount", value); }
        }


        [Category("Compatibility")]
        [DisplayName("Show PasswordSafe warnings")]
        [Description("If true, warning will be shown upon adding fields not compatible with PasswordSafe.")]
        [DefaultValue(false)]
        public static bool ShowPasswordSafeWarnings {
            get { return Config.Read("ShowPasswordSafeWarnings", Medo.Configuration.Settings.Read("ShowPasswordSafeWarnings", false)); }
            set { Config.Write("ShowPasswordSafeWarnings", value); }
        }


        [Category("Behavior")]
        [DisplayName("Always use NTP for two-factor")]
        [Description("If true, each time two-factor code is taken an NTP request will be sent.")]
        [DefaultValue(true)]
        public static bool AlwaysUseNtpForTwoFactor {
            get { return Config.Read("AlwaysUseNtpForTwoFactor", true); }
            set { Config.Write("AlwaysUseNtpForTwoFactor", value); }
        }

        [Category("Behavior")]
        [DisplayName("Show NTP drift warning")]
        [Description("If true, warning will be shown if system time drifts from NTP server time.")]
        [DefaultValue(false)]
        public static bool ShowNtpDriftWarning {
            get { return Config.Read("ShowNtpDriftWarning", false); }
            set { Config.Write("ShowNtpDriftWarning", value); }
        }

        [Category("Behavior")]
        [DisplayName("NTP drift warning seconds")]
        [Description("Amount of drift allowed between system and NTP server.")]
        [DefaultValue(10)]
        public static int NtpDriftWarningSeconds {
            get { return Config.Read("NtpDriftWarningSeconds", LimitBetween(10, 1, 30, allowZero: false)); }
            set { Config.Write("NtpDriftWarningSeconds", LimitBetween(value, 1, 30, allowZero: false)); }
        }

        [Category("Behavior")]
        [DisplayName("NTP server")]
        [Description("NTP server to use.")]
        public static string NtpServer {
            get { return Config.Read("NtpServer", DefaultNtpServer); }
            set { Config.Write("NtpServer", value); }
        }

        [Category("Behavior")]
        [DisplayName("Try current password")]
        [Description("Tries to open file using current document's password, if possible.")]
        public static bool TryCurrentPassword {
            get { return Config.Read("TryCurrentPassword", false); }
            set { Config.Write("TryCurrentPassword", value); }
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

        public enum PasswordGeneratorTab {
            Classic = 0,
            Word = 1,
            Triplet = 2
        }

        [Browsable(false)]
        public static PasswordGeneratorTab PasswordGeneratorTabSelection {
            get { return (PasswordGeneratorTab)LimitBetween(Config.Read("PasswordGeneratorTabSelection", Medo.Configuration.Settings.Read("PasswordGeneratorTabSelection", (int)(PasswordGeneratorUseWord ? PasswordGeneratorTab.Word : PasswordGeneratorTab.Classic))), 0, 2, allowZero: true); }
            set { Config.Write("PasswordGeneratorTabSelection", (int)value); }
        }

        private static bool PasswordGeneratorUseWord {
            get { return Config.Read("PasswordGeneratorUseWord", Medo.Configuration.Settings.Read("PasswordGeneratorUseWord", true)); }
            set { Config.Write("PasswordGeneratorUseWord", value); }
        }

        #region Classic

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

        #endregion Classic

        #region Word

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

        #endregion Word

        #region Triplet

        [Browsable(false)]
        public static bool PasswordGeneratorTripletIncludeNumber {
            get { return Config.Read("PasswordGeneratorTripletIncludeNumber", Medo.Configuration.Settings.Read("PasswordGeneratorTripletIncludeNumber", true)); }
            set { Config.Write("PasswordGeneratorTripletIncludeNumber", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorTripletIncludeSpecialCharacter {
            get { return Config.Read("PasswordGeneratorTripletIncludeSpecialCharacter", Medo.Configuration.Settings.Read("PasswordGeneratorTripletIncludeSpecialCharacter", true)); }
            set { Config.Write("PasswordGeneratorTripletIncludeSpecialCharacter", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorTripletRandomUpperCase {
            get { return Config.Read("PasswordGeneratorTripletRandomUpperCase", Medo.Configuration.Settings.Read("PasswordGeneratorTripletRandomUpperCase", false)); }
            set { Config.Write("PasswordGeneratorTripletRandomUpperCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorTripletRandomLetterDrop {
            get { return Config.Read("PasswordGeneratorTripletRandomLetterDrop", Medo.Configuration.Settings.Read("PasswordGeneratorTripletRandomLetterDrop", false)); }
            set { Config.Write("PasswordGeneratorTripletRandomLetterDrop", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorTripletRestrictTitleCase {
            get { return Config.Read("PasswordGeneratorTripletRestrictTitleCase", Medo.Configuration.Settings.Read("PasswordGeneratorTripletRestrictTitleCase", true)); }
            set { Config.Write("PasswordGeneratorTripletRestrictTitleCase", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorTripletRestrictBreak {
            get { return Config.Read("PasswordGeneratorTripletRestrictBreak", Medo.Configuration.Settings.Read("PasswordGeneratorTripletRestrictBreak", true)); }
            set { Config.Write("PasswordGeneratorTripletRestrictBreak", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorTripletRestrictSuffixOnly {
            get { return Config.Read("PasswordGeneratorTripletRestrictSuffixOnly", Medo.Configuration.Settings.Read("PasswordGeneratorTripletRestrictSuffixOnly", true)); }
            set { Config.Write("PasswordGeneratorTripletRestrictSuffixOnly", value); }
        }

        [Browsable(false)]
        public static bool PasswordGeneratorTripletRestrictAddSpace {
            get { return Config.Read("PasswordGeneratorTripletRestrictAddSpace", Medo.Configuration.Settings.Read("PasswordGeneratorTripletRestrictAddSpace", false)); }
            set { Config.Write("PasswordGeneratorTripletRestrictAddSpace", value); }
        }

        [Browsable(false)]
        public static int PasswordGeneratorTripletCount {
            get { return LimitBetween(Config.Read("PasswordGeneratorTripletCount", Medo.Configuration.Settings.Read("PasswordGeneratorTripletCount", 6)), minValue: 1, maxValue: 9, allowZero: false); }
            set { Config.Write("PasswordGeneratorTripletCount", LimitBetween(value, minValue: 1, maxValue: 9, allowZero: false)); }
        }

        #endregion Triplet

        #endregion PasswordGenerator


        #region Autotype

        [Browsable(false)]
        public static bool AutoTypeUseSendWait {
            get { return Config.Read("AutoTypeUseSendWait", Medo.Configuration.Settings.Read("AutoTypeUseSendWait", true)); }
            set { Config.Write("AutoTypeUseSendWait", value); }
        }

        [Browsable(false)]
        public static int AutoTypeDelay { //in milliseconds
            get { return Config.Read("AutoTypeDelay", LimitBetween(Medo.Configuration.Settings.Read("AutoTypeDelay", 15), 5, 100)); }
            set { Config.Write("AutoTypeDelay", LimitBetween(value, 5, 100)); }
        }

        [Browsable(false)]
        public static int AutoTypeWindowOpacity {
            get { return LimitBetween(Config.Read("AutoTypeWindowOpacity", Medo.Configuration.Settings.Read("AutoTypeWindowOpacity", 100)), minValue: 25, maxValue: 100, allowZero: false); }
            set { Config.Write("AutoTypeWindowOpacity", LimitBetween(value, minValue: 25, maxValue: 100, allowZero: false)); }
        }

        #endregion Autotype


        #region Helper

        private static readonly Random Random = new Random();

        private static readonly string DefaultNtpServer = Random.Next(0, 4).ToString(CultureInfo.InvariantCulture) + ".medo64.pool.ntp.org";

        private static int LimitBetween(int value, int minValue, int maxValue, bool allowZero = false) {
            if (allowZero && (value == 0)) { return 0; }
            if (value < minValue) { return minValue; }
            if (value > maxValue) { return maxValue; }
            return value;
        }

        #endregion Helper
    }
}
