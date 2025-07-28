namespace Bimil;

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Medo.X11;

internal partial class PasswordGeneratorWindow : Window {
    public PasswordGeneratorWindow() {
        InitializeComponent();

        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbClassicIncludeLowercaseLetters, "Settings.PasswordGenerator.Classic.IncludeLowercaseLetters", GenerateNewClassicPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbClassicIncludeUppercaseLetters, "Settings.PasswordGenerator.Classic.IncludeUppercaseLetters", GenerateNewClassicPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbClassicIncludeDigits, "Settings.PasswordGenerator.Classic.IncludeDigits", GenerateNewClassicPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbClassicIncludeSpecialCharacters, "Settings.PasswordGenerator.Classic.IncludeSpecialCharacters", GenerateNewClassicPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbClassicExcludeSimilarCharacters, "Settings.PasswordGenerator.Classic.ExcludeSimilarCharacters", GenerateNewClassicPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbClassicExcludeMovableCharacters, "Settings.PasswordGenerator.Classic.ExcludeMovableCharacters", GenerateNewClassicPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbClassicExcludeUnpronounceable, "Settings.PasswordGenerator.Classic.ExcludeUnpronounceable", GenerateNewClassicPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbClassicExcludeRepeatedCharacters, "Settings.PasswordGenerator.Classic.ExcludeRepeatedCharacters", GenerateNewClassicPassword);
        AvaloniaHelpers.ControlSetup.SetupTextBoxFromInt32(txtClassicLength, "Settings.PasswordGenerator.Classic.PasswordLength", 6, 99, GenerateNewClassicPassword);

        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbWordIncludeNumber, "Settings.PasswordGenerator.Word.IncludeNumber", GenerateNewWordPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbWordIncludeSpecialCharacters, "Settings.PasswordGenerator.Word.IncludeSpecialCharacters", GenerateNewWordPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbWordIncludeSwappedCase, "Settings.PasswordGenerator.Word.IncludeSwappedCase", GenerateNewWordPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbWordDropOneCharacter, "Settings.PasswordGenerator.Word.DropOneCharacter", GenerateNewWordPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbWordUseTitlecase, "Settings.PasswordGenerator.Word.UseTitlecase", GenerateNewWordPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbWordRestrictWordBreakup, "Settings.PasswordGenerator.Word.RestrictWordBreakup", GenerateNewWordPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbWordRestrictAdjustmentsToSuffix, "Settings.PasswordGenerator.Word.RestrictAdjustmentsToSuffix", GenerateNewWordPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbWordUseSpacesSeparator, "Settings.PasswordGenerator.Word.UseSpacesSeparator", GenerateNewWordPassword);
        AvaloniaHelpers.ControlSetup.SetupTextBoxFromInt32(txtWordCount, "Settings.PasswordGenerator.Word.WordCount", 2, 16, GenerateNewWordPassword);

        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbTripletIncludeNumber, "Settings.PasswordGenerator.Triplet.IncludeNumber", GenerateNewTripletPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbTripletIncludeSpecialCharacters, "Settings.PasswordGenerator.Triplet.IncludeSpecialCharacters", GenerateNewTripletPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbTripletIncludeSwappedCase, "Settings.PasswordGenerator.Triplet.IncludeSwappedCase", GenerateNewTripletPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbTripletDropOneCharacter, "Settings.PasswordGenerator.Triplet.DropOneCharacter", GenerateNewTripletPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbTripletUseTitlecase, "Settings.PasswordGenerator.Triplet.UseTitlecase", GenerateNewTripletPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbTripletRestrictWordBreakup, "Settings.PasswordGenerator.Triplet.RestrictWordBreakup", GenerateNewTripletPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbTripletRestrictAdjustmentsToSuffix, "Settings.PasswordGenerator.Triplet.RestrictAdjustmentsToSuffix", GenerateNewTripletPassword);
        AvaloniaHelpers.ControlSetup.SetupCheckBox(chbTripletUseSpacesSeparator, "Settings.PasswordGenerator.Triplet.UseSpacesSeparator", GenerateNewTripletPassword);
        AvaloniaHelpers.ControlSetup.SetupTextBoxFromInt32(txtTripletCount, "Settings.PasswordGenerator.Triplet.TripletCount", 2, 16, GenerateNewTripletPassword);

        tabKind.SelectedIndex = Settings.PasswordGenerator.Kind switch {
            Settings.PasswordGenerator.GeneratorKind.Triplet => 2,
            Settings.PasswordGenerator.GeneratorKind.Word => 1,
            _ => 0,
        };
        tabKind.SelectionChanged += (sender, e) => {
            switch (tabKind.SelectedIndex) {
                case 2: GenerateNewPassword(Settings.PasswordGenerator.GeneratorKind.Triplet); break;
                case 1: GenerateNewPassword(Settings.PasswordGenerator.GeneratorKind.Word); break;
                default: GenerateNewPassword(Settings.PasswordGenerator.GeneratorKind.Classic); break;
            }
        };
        GenerateNewPassword(Settings.PasswordGenerator.Kind);  // for first run

        AvaloniaHelpers.FocusControl(btnCopy);
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }


    #region Password

    private void GenerateNewPassword(Settings.PasswordGenerator.GeneratorKind kind) {  // also saves password
        switch (kind) {
            case Settings.PasswordGenerator.GeneratorKind.Triplet: GenerateNewTripletPassword(); break;
            case Settings.PasswordGenerator.GeneratorKind.Word: GenerateNewWordPassword(); break;
            default: GenerateNewClassicPassword(); break;
        }
        Settings.PasswordGenerator.Kind = kind;
    }

    private void GenerateNewClassicPassword() {
        if (!Settings.PasswordGenerator.Classic.IncludeLowercaseLetters
            && !Settings.PasswordGenerator.Classic.IncludeUppercaseLetters
            && !Settings.PasswordGenerator.Classic.IncludeDigits
            && !Settings.PasswordGenerator.Classic.IncludeSpecialCharacters) {
            chbClassicIncludeLowercaseLetters.IsChecked = true;
            Settings.PasswordGenerator.Classic.IncludeLowercaseLetters = true;
        }
        var generator = new ClassicPasswordGenerator() {
            IncludeLowercaseLetters = Settings.PasswordGenerator.Classic.IncludeLowercaseLetters,
            IncludeUppercaseLetters = Settings.PasswordGenerator.Classic.IncludeUppercaseLetters,
            IncludeDigits = Settings.PasswordGenerator.Classic.IncludeDigits,
            IncludeSpecialCharacters = Settings.PasswordGenerator.Classic.IncludeSpecialCharacters,
            ExcludeSimilarCharacters = Settings.PasswordGenerator.Classic.ExcludeSimilarCharacters,
            ExcludeMovableCharacters = Settings.PasswordGenerator.Classic.ExcludeMovableCharacters,
            ExcludeUnpronounceable = Settings.PasswordGenerator.Classic.ExcludeUnpronounceable,
            ExcludeRepeatedCharacters = Settings.PasswordGenerator.Classic.ExcludeRepeatedCharacters,
            PasswordLength = Settings.PasswordGenerator.Classic.PasswordLength,
        };
        GenerateNewPassword(generator);
    }

    private void GenerateNewWordPassword() {
        var generator = new WordPasswordGenerator() {
            IncludeNumber = Settings.PasswordGenerator.Word.IncludeNumber,
            IncludeSpecialCharacters = Settings.PasswordGenerator.Word.IncludeSpecialCharacters,
            IncludeSwappedCase = Settings.PasswordGenerator.Word.IncludeSwappedCase,
            DropOneCharacter = Settings.PasswordGenerator.Word.DropOneCharacter,
            UseTitlecase = Settings.PasswordGenerator.Word.UseTitlecase,
            RestrictWordBreakup = Settings.PasswordGenerator.Word.RestrictWordBreakup,
            RestrictAdjustmentsToSuffix = Settings.PasswordGenerator.Word.RestrictAdjustmentsToSuffix,
            UseSpacesSeparator = Settings.PasswordGenerator.Word.UseSpacesSeparator,
            WordCount = Settings.PasswordGenerator.Word.WordCount,
        };
        GenerateNewPassword(generator);
    }

    private void GenerateNewTripletPassword() {
        var generator = new TripletPasswordGenerator() {
            IncludeNumber = Settings.PasswordGenerator.Triplet.IncludeNumber,
            IncludeSpecialCharacters = Settings.PasswordGenerator.Triplet.IncludeSpecialCharacters,
            IncludeSwappedCase = Settings.PasswordGenerator.Triplet.IncludeSwappedCase,
            DropOneCharacter = Settings.PasswordGenerator.Triplet.DropOneCharacter,
            UseTitlecase = Settings.PasswordGenerator.Triplet.UseTitlecase,
            RestrictWordBreakup = Settings.PasswordGenerator.Triplet.RestrictWordBreakup,
            RestrictAdjustmentsToSuffix = Settings.PasswordGenerator.Triplet.RestrictAdjustmentsToSuffix,
            UseSpacesSeparator = Settings.PasswordGenerator.Triplet.UseSpacesSeparator,
            TripletCount = Settings.PasswordGenerator.Triplet.TripletCount,
        };
        GenerateNewPassword(generator);
    }

    private void GenerateNewPassword(PasswordGenerator generator) {
        txtPassword.Text = generator.GetNewPassword();

        switch (generator.GetEstimatedSecurityLevel()) {
            case PasswordSecurityLevel.High: ThemeImageResources.SetImage(imgGeneratorLevel, "SecurityHigh", doubleScale: true); break;
            case PasswordSecurityLevel.Medium: ThemeImageResources.SetImage(imgGeneratorLevel, "SecurityMedium", doubleScale: true); break;
            default: ThemeImageResources.SetImage(imgGeneratorLevel, "SecurityLow", doubleScale: true); break;
        }

        var duration = generator.GetEstimatedCrackDuration();
        var totalDays = (long)duration.TotalDays;
        var totalHours = (long)duration.TotalHours;
        var totalMinutes = (long)duration.TotalMinutes;
        var totalSeconds = (long)duration.TotalSeconds;
        var durationText = totalDays switch {
            > 3650000 => "An eternity to crack",
            > 36500 => "About " + (totalDays / 36500).ToString(CultureInfo.CurrentUICulture) + " centuries to crack",
            > 365 => "About " + (totalDays / 365).ToString(CultureInfo.CurrentUICulture) + " years to crack",
            > 31 => "About " + (totalDays / 31).ToString(CultureInfo.CurrentUICulture) + " months to crack",
            > 7 => "About " + (totalDays / 7).ToString(CultureInfo.CurrentUICulture) + " weeks to crack",
            > 1 => "About " + (totalDays / 7).ToString(CultureInfo.CurrentUICulture) + " days to crack",
            _ => totalHours switch {
                > 1 => "About " + totalHours.ToString(CultureInfo.CurrentUICulture) + " hours to crack",
                _ => totalMinutes switch {
                    > 1 => "About " + totalMinutes.ToString(CultureInfo.CurrentUICulture) + " minutes to crack",
                    _ => totalSeconds switch {
                        > 1 => totalSeconds.ToString(CultureInfo.CurrentUICulture) + " seconds to crack",
                        _ => "Less then a second to crack",
                    },
                },
            },
        };
        lblGeneratorStats.Content = durationText;
        ToolTip.SetTip(lblGeneratorStats, generator.GetEstimatedCombinationCount().ToString("0.00E0", CultureInfo.CurrentUICulture) + "\n" + generator.GetEstimatedEntropyInBits().ToString(CultureInfo.CurrentUICulture) + " bits of entropy");
    }

    #endregion Password

    #region Buttons

    public void btnRegenerate_Click(object sender, RoutedEventArgs e) {
        GenerateNewPassword(Settings.PasswordGenerator.Kind);
    }

    public async void btnCopy_Click(object sender, RoutedEventArgs e) {
        if (Clipboard != null) {
            var text = txtPassword.Text!;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Settings.SyncX11PrimaryClipboard) {  // set both primary and clipboard on X11
                X11Clipboard.Primary.SetText(text);  // no fallback
                if (X11Clipboard.Clipboard.IsAvailable) {
                    X11Clipboard.Clipboard.SetText(text);
                    return;  // skip call to Avalonia clipboard
                }
            }

            await Clipboard.SetTextAsync(text);
        } else {
            Debug.WriteLine("Clipboard is not available.");
        }
    }

    #endregion Buttons

}
