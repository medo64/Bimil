namespace Bimil.Desktop;

using System;
using Medo.Configuration;

internal static class Settings {

    /// <summary>
    /// Gets/sets if main window should be closed when Escape key is pressed.
    /// Default value is false.
    /// </summary>
    public static bool CloseOnEscape {
        get { return Config.Read("CloseOnEscape", false); }
        set { Config.Write("CloseOnEscape", value); }
    }

    /// <summary>
    /// Gets/sets theme.
    /// Value can be either "Default", "Light", or "Dark".
    /// Default value is "Default" (auto-detect).
    /// </summary>
    public static ThemeVariant Theme {
        get {
            var value = Config.Read("Theme", "Default");
            if (value.Equals("Light", StringComparison.OrdinalIgnoreCase)) { return ThemeVariant.Light; }
            if (value.Equals("Dark", StringComparison.OrdinalIgnoreCase)) { return ThemeVariant.Dark; }
            return ThemeVariant.Default;
        }
        set { Config.Write("Theme", value.ToString()); }
    }


    // [Category("Behavior")]
    // [DisplayName("Show start")]
    // [Description("If true, Start window will be shown.")]
    // [DefaultValue(true)]
    // public static bool ShowStart {
    //     get { return Config.Read("ShowStart", true); }
    //     set {
    //         Config.Write("ShowStart", value);
    //         if ((value == true) && LoadLast) { LoadLast = false; }
    //     }
    // }

    // [Category("Behavior")]
    // [DisplayName("Load last")]
    // [Description("If true, the last used file will be loaded.")]
    // [DefaultValue(false)]
    // public static bool LoadLast {
    //     get { return Config.Read("LoadLast", false) && !ShowStart; }
    //     set {
    //         Config.Write("LoadLast", value);
    //         if ((value == true) && ShowStart) { ShowStart = false; }
    //     }
    // }

    // [Category("Behavior")]
    // [DisplayName("Auto-close application timeout")]
    // [Description("Time in seconds for main window to automatically close if it loses focus. Value 0 disables auto-close.")]
    // [DefaultValue(900)]
    // public static int AutoCloseTimeout {
    //     get { return LimitBetween(Config.Read("AutoCloseTimeout", 900), minValue: 10, maxValue: 3600, allowZero: true); }
    //     set { Config.Write("AutoCloseTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Auto-close window timeout")]
    // [Description("Time in seconds for item window to automatically close if it loses focus. Value 0 disables auto-close. Note that auto-close will cancel any edit in progress.")]
    // [DefaultValue(120)]
    // public static int AutoCloseItemTimeout {
    //     get { return LimitBetween(Config.Read("AutoCloseItemTimeout", 120), minValue: 10, maxValue: 3600, allowZero: true); }
    //     set { Config.Write("AutoCloseItemTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Auto-close save")]
    // [Description("If true, edited items will be automatically saved when auto-close gets activated.")]
    // [DefaultValue(true)]
    // public static bool AutoCloseSave {
    //     get { return Config.Read("AutoCloseSave", true); }
    //     set { Config.Write("AutoCloseSave", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Auto-clear clipboard")]
    // [Description("Time in seconds for clipboard to clear.")]
    // [DefaultValue(30)]
    // public static int AutoClearClipboardTimeout {
    //     get { return LimitBetween(Config.Read("AutoClearClipboardTimeout", 30), minValue: 10, maxValue: 3600, allowZero: true); }
    //     set { Config.Write("AutoClearClipboardTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Auto-clear after paste")]
    // [Description("If true, data will be removed from clipboard after paste. Valid only if also clipboard timeout is set.")]
    // [DefaultValue(true)]
    // public static bool AutoClearClipboardAfterPaste {
    //     get { return Config.Read("AutoClearClipboardAfterPaste", true); }
    //     set { Config.Write("AutoClearClipboardAfterPaste", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Auto-clear only sensitive data")]
    // [Description("If true, only sensitive data (such as password or two factor keys) is to be auto-clear from clipboard.")]
    // [DefaultValue(false)]
    // public static bool AutoClearClipboardForSensitiveDataOnly {
    //     get { return Config.Read("AutoClearClipboardForSensitiveDataOnly", false); }
    //     set { Config.Write("AutoClearClipboardForSensitiveDataOnly", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Editable by default")]
    // [Description("If true, all fields will be editable by default.")]
    // [DefaultValue(true)]
    // public static bool EditableByDefault {
    //     get { return Config.Read("EditableByDefault", true); }
    //     set { Config.Write("EditableByDefault", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Show common password warnings")]
    // [Description("If true, warning will be shown if a password similar to common is used.")]
    // [DefaultValue(true)]
    // public static bool ShowCommonPasswordWarnings {
    //     get { return Config.Read("ShowCommonPasswordWarnings", true); }
    //     set { Config.Write("ShowCommonPasswordWarnings", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Save password history by default")]
    // [Description("If true, password history storage will automatically be added to all new entries.")]
    // [DefaultValue(false)]
    // public static bool SavePasswordHistoryByDefault {
    //     get { return Config.Read("SavePasswordHistory", false); }
    //     set { Config.Write("SavePasswordHistory", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Default password history count")]
    // [Description("Default number of password history entries.")]
    // [DefaultValue(5)]
    // public static int SavePasswordHistoryDefaultCount {
    //     get { return Config.Read("SavePasswordHistoryDefaultCount", 5); }
    //     set { Config.Write("SavePasswordHistoryDefaultCount", value); }
    // }


    // [Category("Compatibility")]
    // [DisplayName("Show PasswordSafe warnings")]
    // [Description("If true, warning will be shown upon adding fields not compatible with PasswordSafe.")]
    // [DefaultValue(false)]
    // public static bool ShowPasswordSafeWarnings {
    //     get { return Config.Read("ShowPasswordSafeWarnings", false); }
    //     set { Config.Write("ShowPasswordSafeWarnings", value); }
    // }


    // [Category("Behavior")]
    // [DisplayName("Check password at Have I been pwned?")]
    // [Description("If true, hashed password will be sent over TLS 1.2 to Have I been pwned? (haveibeenpwned.com) servers to verify if it is among breached passwords. This will only be done during explicit weak password search and not during the normal password entry or use.")]
    // [DefaultValue(false)]
    // public static bool HibpCheckWeakPassword {
    //     get { return Config.Read("HibpCheckWeakPassword", true); }
    //     set { Config.Write("HibpCheckWeakPassword", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Randomize password query order at Have I been pwned?")]
    // [Description("If true, hashed password will be queried in random order.")]
    // [DefaultValue(false)]
    // public static bool HibpCheckWeakPasswordInRandomOrder {
    //     get { return Config.Read("HibpCheckWeakPasswordInRandomOrder", false); }
    //     set { Config.Write("HibpCheckWeakPasswordInRandomOrder", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Throttle interval for Have I been pwned?")]
    // [Description("Milliseconds between queries toward Have I been pwned? site.")]
    // [DefaultValue(100)]
    // public static int HibpThrottleInterval {
    //     get { return Math.Max(0, Math.Min(Config.Read("HibpThrottleInterval", 10), 10000)); }
    //     set { Config.Write("HibpThrottleInterval", value); }
    // }


    // [Category("Behavior")]
    // [DisplayName("Always use NTP for two-factor")]
    // [Description("If true, each time two-factor code is taken an NTP request will be sent.")]
    // [DefaultValue(true)]
    // public static bool AlwaysUseNtpForTwoFactor {
    //     get { return Config.Read("AlwaysUseNtpForTwoFactor", true); }
    //     set { Config.Write("AlwaysUseNtpForTwoFactor", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Show NTP drift warning")]
    // [Description("If true, warning will be shown if system time drifts from NTP server time.")]
    // [DefaultValue(false)]
    // public static bool ShowNtpDriftWarning {
    //     get { return Config.Read("ShowNtpDriftWarning", false); }
    //     set { Config.Write("ShowNtpDriftWarning", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("NTP drift warning seconds")]
    // [Description("Amount of drift allowed between system and NTP server.")]
    // [DefaultValue(10)]
    // public static int NtpDriftWarningSeconds {
    //     get { return Config.Read("NtpDriftWarningSeconds", LimitBetween(10, 1, 30, allowZero: false)); }
    //     set { Config.Write("NtpDriftWarningSeconds", LimitBetween(value, 1, 30, allowZero: false)); }
    // }

    // [Category("Behavior")]
    // [DisplayName("NTP server")]
    // [Description("NTP server to use.")]
    // public static string NtpServer {
    //     get { return Config.Read("NtpServer", DefaultNtpServer); }
    //     set { Config.Write("NtpServer", value); }
    // }

    // [Category("Behavior")]
    // [DisplayName("Try current password")]
    // [Description("Tries to open file using current document's password, if possible.")]
    // public static bool TryCurrentPassword {
    //     get { return Config.Read("TryCurrentPassword", false); }
    //     set { Config.Write("TryCurrentPassword", value); }
    // }


    // private static readonly string[] MonotypeFontDefaults = { "Consolas", "Lucida Console", "Nimbus Mono L", "Ubuntu Mono", "Courier New", "Courier" };
    // [Category("Visual")]
    // [DisplayName("Monotype font")]
    // [Description("Monotype font used for passwords.")]
    // public static Font MonotypeFont {
    //     get {
    //         var fontName = Config.Read("MonotypeFontName", null)?.Trim();
    //         var fontSize = SystemFonts.MessageBoxFont.SizeInPoints + 0.5F;

    //         if (!string.IsNullOrEmpty(fontName)) { // first try custom font name
    //             var newFont = new Font(fontName, fontSize, SystemFonts.MessageBoxFont.Style);
    //             if (newFont.Name.Equals(fontName, StringComparison.InvariantCultureIgnoreCase)) { return newFont; }
    //         }

    //         foreach (var defaultFontName in MonotypeFontDefaults) { // try a few others
    //             var newFont = new Font(defaultFontName, fontSize, SystemFonts.MessageBoxFont.Style);
    //             if (newFont.Name.Equals(defaultFontName, StringComparison.InvariantCultureIgnoreCase)) { return newFont; }
    //         }

    //         // give up and return generic monospace
    //         return new Font(FontFamily.GenericMonospace, fontSize, SystemFonts.MessageBoxFont.Style);
    //     }
    //     set {
    //         Config.Write("MonotypeFontName", value.Name);
    //     }
    // }


    // #region PasswordGenerator

    // public enum PasswordGeneratorTab {
    //     Classic = 0,
    //     Word = 1,
    //     Triplet = 2
    // }

    // [Browsable(false)]
    // public static PasswordGeneratorTab PasswordGeneratorTabSelection {
    //     get { return (PasswordGeneratorTab)LimitBetween(Config.Read("PasswordGeneratorTabSelection", (int)(PasswordGeneratorUseWord ? PasswordGeneratorTab.Word : PasswordGeneratorTab.Classic)), 0, 2, allowZero: true); }
    //     set { Config.Write("PasswordGeneratorTabSelection", (int)value); }
    // }

    // private static bool PasswordGeneratorUseWord {
    //     get { return Config.Read("PasswordGeneratorUseWord", true); }
    //     set { Config.Write("PasswordGeneratorUseWord", value); }
    // }


    internal static class PasswordGenerator {

        internal static class Classic {

            public static bool IncludeLowercaseLetters {
                get { return Config.Read("PasswordGenerator/Classic/IncludeLowercaseLetters", true); }
                set { Config.Write("PasswordGenerator/Classic/IncludeLowercaseLetters", value); }
            }

            public static bool IncludeUppercaseLetters {
                get { return Config.Read("PasswordGenerator/Classic/IncludeUppercaseLetters", true); }
                set { Config.Write("PasswordGenerator/Classic/IncludeUppercaseLetters", value); }
            }

            public static bool IncludeDigits {
                get { return Config.Read("PasswordGenerator/Classic/IncludeDigits", true); }
                set { Config.Write("PasswordGenerator/Classic/IncludeDigits", value); }
            }

            public static bool IncludeSpecialCharacters {
                get { return Config.Read("PasswordGenerator/Classic/IncludeSpecialCharacters", true); }
                set { Config.Write("PasswordGenerator/Classic/IncludeSpecialCharacters", value); }
            }

            public static bool ExcludeSimilarCharacters {
                get { return Config.Read("PasswordGenerator/Classic/ExcludeSimilarCharacters", false); }
                set { Config.Write("PasswordGenerator/Classic/ExcludeSimilarCharacters", value); }
            }

            public static bool ExcludeMovableCharacters {
                get { return Config.Read("PasswordGenerator/Classic/ExcludeMovableCharacters", false); }
                set { Config.Write("PasswordGenerator/Classic/ExcludeMovableCharacters", value); }
            }

            public static bool ExcludeUnpronounceable {
                get { return Config.Read("PasswordGenerator/Classic/ExcludeUnpronounceable", false); }
                set { Config.Write("PasswordGenerator/Classic/ExcludeUnpronounceable", value); }
            }

            public static bool ExcludeRepeatedCharacters {
                get { return Config.Read("PasswordGenerator/Classic/ExcludeRepeatedCharacters", false); }
                set { Config.Write("PasswordGenerator/Classic/ExcludeRepeatedCharacters", value); }
            }

            public static int PasswordLength {
                get { return LimitBetween(Config.Read("PasswordGenerator/Classic/PasswordLength", 14), minValue: 6, maxValue: 99, allowZero: false); }
                set { Config.Write("PasswordGenerator/Classic/PasswordLength", LimitBetween(value, minValue: 6, maxValue: 99, allowZero: false)); }
            }
        }

    }


    // #region Word

    // [Browsable(false)]
    // public static bool PasswordGeneratorWordIncludeUpperCase {
    //     get { return Config.Read("PasswordGeneratorWordIncludeUpperCase", false); }
    //     set { Config.Write("PasswordGeneratorWordIncludeUpperCase", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorWordIncludeNumber {
    //     get { return Config.Read("PasswordGeneratorWordIncludeNumber", true); }
    //     set { Config.Write("PasswordGeneratorWordIncludeNumber", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorWordIncludeSpecialCharacter {
    //     get { return Config.Read("PasswordGeneratorWordIncludeSpecialCharacter", true); }
    //     set { Config.Write("PasswordGeneratorWordIncludeSpecialCharacter", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorWordIncludeIncomplete {
    //     get { return Config.Read("PasswordGeneratorWordIncludeIncomplete", false); }
    //     set { Config.Write("PasswordGeneratorWordIncludeIncomplete", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorWordRestrictAddSpace {
    //     get { return Config.Read("PasswordGeneratorWordRestrictAddSpace", false); }
    //     set { Config.Write("PasswordGeneratorWordRestrictAddSpace", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorWordRestrictBreak {
    //     get { return Config.Read("PasswordGeneratorWordRestrictBreak", true); }
    //     set { Config.Write("PasswordGeneratorWordRestrictBreak", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorWordRestrictTitleCase {
    //     get { return Config.Read("PasswordGeneratorWordRestrictTitleCase", true); }
    //     set { Config.Write("PasswordGeneratorWordRestrictTitleCase", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorWordRestrictSuffixOnly {
    //     get { return Config.Read("PasswordGeneratorWordRestrictSuffixOnly", false); }
    //     set { Config.Write("PasswordGeneratorWordRestrictSuffixOnly", value); }
    // }

    // [Browsable(false)]
    // public static int PasswordGeneratorWordCount {
    //     get { return LimitBetween(Config.Read("PasswordGeneratorWordCount", 5), minValue: 1, maxValue: 9, allowZero: false); }
    //     set { Config.Write("PasswordGeneratorWordCount", LimitBetween(value, minValue: 1, maxValue: 9, allowZero: false)); }
    // }

    // #endregion Word

    // #region Triplet

    // [Browsable(false)]
    // public static bool PasswordGeneratorTripletIncludeNumber {
    //     get { return Config.Read("PasswordGeneratorTripletIncludeNumber", true); }
    //     set { Config.Write("PasswordGeneratorTripletIncludeNumber", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorTripletIncludeSpecialCharacter {
    //     get { return Config.Read("PasswordGeneratorTripletIncludeSpecialCharacter", true); }
    //     set { Config.Write("PasswordGeneratorTripletIncludeSpecialCharacter", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorTripletRandomUpperCase {
    //     get { return Config.Read("PasswordGeneratorTripletRandomUpperCase", false); }
    //     set { Config.Write("PasswordGeneratorTripletRandomUpperCase", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorTripletRandomLetterDrop {
    //     get { return Config.Read("PasswordGeneratorTripletRandomLetterDrop", false); }
    //     set { Config.Write("PasswordGeneratorTripletRandomLetterDrop", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorTripletRestrictTitleCase {
    //     get { return Config.Read("PasswordGeneratorTripletRestrictTitleCase", true); }
    //     set { Config.Write("PasswordGeneratorTripletRestrictTitleCase", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorTripletRestrictBreak {
    //     get { return Config.Read("PasswordGeneratorTripletRestrictBreak", true); }
    //     set { Config.Write("PasswordGeneratorTripletRestrictBreak", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorTripletRestrictSuffixOnly {
    //     get { return Config.Read("PasswordGeneratorTripletRestrictSuffixOnly", true); }
    //     set { Config.Write("PasswordGeneratorTripletRestrictSuffixOnly", value); }
    // }

    // [Browsable(false)]
    // public static bool PasswordGeneratorTripletRestrictAddSpace {
    //     get { return Config.Read("PasswordGeneratorTripletRestrictAddSpace", false); }
    //     set { Config.Write("PasswordGeneratorTripletRestrictAddSpace", value); }
    // }

    // [Browsable(false)]
    // public static int PasswordGeneratorTripletCount {
    //     get { return LimitBetween(Config.Read("PasswordGeneratorTripletCount", 6), minValue: 1, maxValue: 9, allowZero: false); }
    //     set { Config.Write("PasswordGeneratorTripletCount", LimitBetween(value, minValue: 1, maxValue: 9, allowZero: false)); }
    // }

    // #endregion Triplet

    // #endregion PasswordGenerator


    // #region Autotype

    // [Browsable(false)]
    // public static bool AutoTypeUseSendWait {
    //     get { return Config.Read("AutoTypeUseSendWait", true); }
    //     set { Config.Write("AutoTypeUseSendWait", value); }
    // }

    // [Browsable(false)]
    // public static int AutoTypeDelay { //in milliseconds
    //     get { return LimitBetween(Config.Read("AutoTypeDelay", 15), 5, 100); }
    //     set { Config.Write("AutoTypeDelay", LimitBetween(value, 5, 100)); }
    // }

    // [Browsable(false)]
    // public static int AutoTypeWindowOpacity {
    //     get { return LimitBetween(Config.Read("AutoTypeWindowOpacity", 100), minValue: 25, maxValue: 100, allowZero: false); }
    //     set { Config.Write("AutoTypeWindowOpacity", LimitBetween(value, minValue: 25, maxValue: 100, allowZero: false)); }
    // }

    // #endregion Autotype


    #region Helper

    public enum ThemeVariant {
        Default = 0,
        Light = 1,
        Dark = 2,
    }

    //private static readonly string DefaultNtpServer = Random.Shared.Next(0, 4).ToString(CultureInfo.InvariantCulture) + ".medo64.pool.ntp.org";

     private static int LimitBetween(int value, int minValue, int maxValue, bool allowZero = false) {
        if (allowZero && (value == 0)) { return 0; }
        if (value < minValue) { return minValue; }
        if (value > maxValue) { return maxValue; }
        return value;
    }

    #endregion Helper
}
