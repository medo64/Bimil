namespace Bimil.Desktop;

using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using Bimil.Core;

internal partial class PasswordGeneratorWindow : Window {
    public PasswordGeneratorWindow() {
        InitializeComponent();

        var classicPassword = () => {
            if (!Settings.PasswordGenerator.Classic.IncludeLowercaseLetters
                && !Settings.PasswordGenerator.Classic.IncludeUppercaseLetters
                && !Settings.PasswordGenerator.Classic.IncludeDigits
                && !Settings.PasswordGenerator.Classic.IncludeSpecialCharacters) {
                Helpers.GetControl<CheckBox>(this, "chbIncludeLowercaseLetters").IsChecked = true;
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
            Helpers.GetControl<TextBox>(this, "txtPassword").Text = generator.GetNewPassword();
        };
        Helpers.ControlSetup.SetupCheckBox(this, "chbClassicIncludeLowercaseLetters", "Settings.PasswordGenerator.Classic.IncludeLowercaseLetters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbClassicIncludeUppercaseLetters", "Settings.PasswordGenerator.Classic.IncludeUppercaseLetters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbClassicIncludeDigits", "Settings.PasswordGenerator.Classic.IncludeDigits", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbClassicIncludeSpecialCharacters", "Settings.PasswordGenerator.Classic.IncludeSpecialCharacters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbClassicExcludeSimilarCharacters", "Settings.PasswordGenerator.Classic.ExcludeSimilarCharacters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbClassicExcludeMovableCharacters", "Settings.PasswordGenerator.Classic.ExcludeMovableCharacters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbClassicExcludeUnpronounceable", "Settings.PasswordGenerator.Classic.ExcludeUnpronounceable", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbClassicExcludeRepeatedCharacters", "Settings.PasswordGenerator.Classic.ExcludeRepeatedCharacters", classicPassword);
        Helpers.ControlSetup.SetupTextBoxFromInt32(this, "txtClassicLength", "Settings.PasswordGenerator.Classic.PasswordLength", 6, 99, classicPassword);

        var wordPassword = () => {
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
            Helpers.GetControl<TextBox>(this, "txtPassword").Text = generator.GetNewPassword();
        };
        Helpers.ControlSetup.SetupCheckBox(this, "chbWordIncludeNumber", "Settings.PasswordGenerator.Word.IncludeNumber", wordPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbWordIncludeSpecialCharacters", "Settings.PasswordGenerator.Word.IncludeSpecialCharacters", wordPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbWordIncludeSwappedCase", "Settings.PasswordGenerator.Word.IncludeSwappedCase", wordPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbWordDropOneCharacter", "Settings.PasswordGenerator.Word.DropOneCharacter", wordPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbWordUseTitlecase", "Settings.PasswordGenerator.Word.UseTitlecase", wordPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbWordRestrictWordBreakup", "Settings.PasswordGenerator.Word.RestrictWordBreakup", wordPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbWordRestrictAdjustmentsToSuffix", "Settings.PasswordGenerator.Word.RestrictAdjustmentsToSuffix", wordPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbWordUseSpacesSeparator", "Settings.PasswordGenerator.Word.UseSpacesSeparator", wordPassword);
        Helpers.ControlSetup.SetupTextBoxFromInt32(this, "txtWordCount", "Settings.PasswordGenerator.Word.WordCount", 2, 16, wordPassword);

        var tripletPassword = () => {
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
            Helpers.GetControl<TextBox>(this, "txtPassword").Text = generator.GetNewPassword();
        };
        Helpers.ControlSetup.SetupCheckBox(this, "chbTripletIncludeNumber", "Settings.PasswordGenerator.Triplet.IncludeNumber", tripletPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbTripletIncludeSpecialCharacters", "Settings.PasswordGenerator.Triplet.IncludeSpecialCharacters", tripletPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbTripletIncludeSwappedCase", "Settings.PasswordGenerator.Triplet.IncludeSwappedCase", tripletPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbTripletDropOneCharacter", "Settings.PasswordGenerator.Triplet.DropOneCharacter", tripletPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbTripletUseTitlecase", "Settings.PasswordGenerator.Triplet.UseTitlecase", tripletPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbTripletRestrictWordBreakup", "Settings.PasswordGenerator.Triplet.RestrictWordBreakup", tripletPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbTripletRestrictAdjustmentsToSuffix", "Settings.PasswordGenerator.Triplet.RestrictAdjustmentsToSuffix", tripletPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbTripletUseSpacesSeparator", "Settings.PasswordGenerator.Triplet.UseSpacesSeparator", tripletPassword);
        Helpers.ControlSetup.SetupTextBoxFromInt32(this, "txtTripletCount", "Settings.PasswordGenerator.Triplet.TripletCount", 2, 16, tripletPassword);

        var fillPassword = (Settings.PasswordGenerator.GeneratorKind kind) => {
            switch (kind) {
                case Settings.PasswordGenerator.GeneratorKind.Triplet: tripletPassword.Invoke(); break;
                case Settings.PasswordGenerator.GeneratorKind.Word: wordPassword.Invoke(); break;
                default: classicPassword.Invoke(); break;
            }
            Settings.PasswordGenerator.Kind = kind;
        };
        var tabKind = Helpers.GetControl<TabControl>(this, "tabKind");
        switch (Settings.PasswordGenerator.Kind) {
            case Settings.PasswordGenerator.GeneratorKind.Triplet: tabKind.SelectedIndex = 2; break;
            case Settings.PasswordGenerator.GeneratorKind.Word: tabKind.SelectedIndex = 1; break;
            default: tabKind.SelectedIndex = 0; break;
        }
        tabKind.SelectionChanged += (sender, e) => {
            switch (tabKind.SelectedIndex) {
                case 2: fillPassword.Invoke(Settings.PasswordGenerator.GeneratorKind.Triplet); break;
                case 1: fillPassword.Invoke(Settings.PasswordGenerator.GeneratorKind.Word); break;
                default: fillPassword.Invoke(Settings.PasswordGenerator.GeneratorKind.Classic); break;
            }
        };
        fillPassword.Invoke(Settings.PasswordGenerator.Kind);  // for first run

        Helpers.FocusControl(this, "btnCopy");
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { this.Close(); }
        base.OnKeyDown(e);
    }

}
