namespace Bimil.Desktop;

using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

internal partial class OptionsWindow : Window {
    public OptionsWindow() {
        InitializeComponent();

        // Behavior
        Helpers.GetControl<CheckBox>(this, "chbCloseOnEscape").IsChecked = Settings.CloseOnEscape;

        // Special
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            Helpers.GetControl<CheckBox>(this, "chbSyncX11PrimaryClipboard").IsChecked = Settings.SyncX11PrimaryClipboard;
        } else {
            Helpers.GetControl<CheckBox>(this, "chbSyncX11PrimaryClipboard").IsVisible = false;
        }
        Helpers.GetControl<CheckBox>(this, "chbShowPasswordSafeCompatibilityWarnings").IsChecked = Settings.ShowPasswordSafeCompatibilityWarnings;

        Helpers.FocusControl(this, "btnClose");
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }


    public void btnDefaults_Click(object sender, RoutedEventArgs e) {
        // Behavior
        Helpers.GetControl<CheckBox>(this, "chbCloseOnEscape").IsChecked = false;

        // Special
        Helpers.GetControl<CheckBox>(this, "chbSyncX11PrimaryClipboard").IsChecked = true;
        Helpers.GetControl<CheckBox>(this, "chbShowPasswordSafeCompatibilityWarnings").IsChecked = false;
    }

    public void btnSave_Click(object sender, RoutedEventArgs e) {
        // Behavior
        Settings.CloseOnEscape = Helpers.GetControl<CheckBox>(this, "chbCloseOnEscape").IsChecked!.Value;

        // Special
        Settings.SyncX11PrimaryClipboard = Helpers.GetControl<CheckBox>(this, "chbSyncX11PrimaryClipboard").IsChecked!.Value;
        Settings.ShowPasswordSafeCompatibilityWarnings = Helpers.GetControl<CheckBox>(this, "chbShowPasswordSafeCompatibilityWarnings").IsChecked!.Value;

        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
