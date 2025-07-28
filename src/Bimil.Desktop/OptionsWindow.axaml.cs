namespace Bimil;

using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

internal partial class OptionsWindow : Window {
    public OptionsWindow() {
        InitializeComponent();

        LoadState();
        chbLoadLast.IsCheckedChanged += (sender, e) => { if (chbLoadLast.IsChecked == true) { chbShowStart.IsChecked = false; } };
        chbShowStart.IsCheckedChanged += (sender, e) => { if (chbShowStart.IsChecked == true) { chbLoadLast.IsChecked = false; } };

        Helpers.FocusControl(btnClose);
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }


    private void LoadState() {
        // Behavior
        chbCloseOnEscape.IsChecked = Settings.CloseOnEscape;
        chbLoadLast.IsChecked = Settings.LoadLast;
        chbShowStart.IsChecked = Settings.ShowStart;

        // Special
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            chbSyncX11PrimaryClipboard.IsChecked = Settings.SyncX11PrimaryClipboard;
        } else {
            chbSyncX11PrimaryClipboard.IsVisible = false;
        }
        chbShowPasswordSafeCompatibilityWarnings.IsChecked = Settings.ShowPasswordSafeCompatibilityWarnings;
    }

    public void btnDefaults_Click(object sender, RoutedEventArgs e) {
        // Behavior
        chbCloseOnEscape.IsChecked = false;
        chbLoadLast.IsChecked = false;
        chbShowStart.IsChecked = true;

        // Special
        chbSyncX11PrimaryClipboard.IsChecked = true;
        chbShowPasswordSafeCompatibilityWarnings.IsChecked = false;
    }

    public void btnSave_Click(object sender, RoutedEventArgs e) {
        // Behavior
        Settings.CloseOnEscape = chbCloseOnEscape.IsChecked!.Value;
        Settings.LoadLast = chbLoadLast.IsChecked!.Value;
        Settings.ShowStart = chbShowStart.IsChecked!.Value;

        // Special
        Settings.SyncX11PrimaryClipboard = chbSyncX11PrimaryClipboard.IsChecked!.Value;
        Settings.ShowPasswordSafeCompatibilityWarnings = chbShowPasswordSafeCompatibilityWarnings.IsChecked!.Value;

        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
