namespace Bimil;

using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

internal partial class OptionsWindow : Window {
    public OptionsWindow() {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

        LoadState();
        chbLoadLast.IsCheckedChanged += (sender, e) => { if (chbLoadLast.IsChecked == true) { chbShowStart.IsChecked = false; } };
        chbShowStart.IsCheckedChanged += (sender, e) => { if (chbShowStart.IsChecked == true) { chbLoadLast.IsChecked = false; } };

        AvaloniaHelpers.FocusControl(btnClose);
    }


    private void LoadState() {
        // Behavior
        chbCloseOnEscape.IsChecked = Settings.CloseOnEscape;
        chbLoadLast.IsChecked = Settings.LoadLast;
        chbShowStart.IsChecked = Settings.ShowStart;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            chbSyncPrimaryClipboard.IsChecked = Settings.SyncPrimaryClipboard;
        } else {
            chbSyncPrimaryClipboard.IsVisible = false;
        }

        // UI
        cmbTheme.SelectedIndex = (int)Settings.Theme;
    }

    public void btnDefaults_Click(object sender, RoutedEventArgs e) {
        // Behavior
        chbCloseOnEscape.IsChecked = Settings.Defaults.CloseOnEscape;
        chbLoadLast.IsChecked = Settings.Defaults.LoadLast;
        chbShowStart.IsChecked = Settings.Defaults.ShowStart;;
        chbSyncPrimaryClipboard.IsChecked = Settings.Defaults.SyncPrimaryClipboard;

        // UI
        cmbTheme.SelectedIndex = (int)Settings.Defaults.Theme;
    }

    public void btnSave_Click(object sender, RoutedEventArgs e) {
        // Behavior
        Settings.CloseOnEscape = chbCloseOnEscape.IsChecked!.Value;
        Settings.LoadLast = chbLoadLast.IsChecked!.Value;
        Settings.ShowStart = chbShowStart.IsChecked!.Value;
        Settings.SyncPrimaryClipboard = chbSyncPrimaryClipboard.IsChecked!.Value;

        // UI
        Settings.Theme = (Settings.ThemeVariant)cmbTheme.SelectedIndex;

        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
