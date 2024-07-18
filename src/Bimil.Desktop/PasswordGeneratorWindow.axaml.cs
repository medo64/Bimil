namespace Bimil.Desktop;

using System.Globalization;
using Avalonia.Controls;

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
            var generator = new Bimil.Core.ClassicPasswordGenerator() {
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
        Helpers.ControlSetup.SetupCheckBox(this, "chbIncludeLowercaseLetters", "Settings.PasswordGenerator.Classic.IncludeLowercaseLetters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbIncludeUppercaseLetters", "Settings.PasswordGenerator.Classic.IncludeUppercaseLetters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbIncludeDigits", "Settings.PasswordGenerator.Classic.IncludeDigits", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbIncludeSpecialCharacters", "Settings.PasswordGenerator.Classic.IncludeSpecialCharacters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbExcludeSimilarCharacters", "Settings.PasswordGenerator.Classic.ExcludeSimilarCharacters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbExcludeMovableCharacters", "Settings.PasswordGenerator.Classic.ExcludeMovableCharacters", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbExcludeUnpronounceable", "Settings.PasswordGenerator.Classic.ExcludeUnpronounceable", classicPassword);
        Helpers.ControlSetup.SetupCheckBox(this, "chbExcludeRepeatedCharacters", "Settings.PasswordGenerator.Classic.ExcludeRepeatedCharacters", classicPassword);
        Helpers.ControlSetup.SetupTextBoxFromInt32(this, "txtPasswordLength", "Settings.PasswordGenerator.Classic.PasswordLength", 6, 99, classicPassword);
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { this.Close(); }
        base.OnKeyDown(e);
    }

}
