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
    }

    public void btnDefaults_Click(object sender, RoutedEventArgs e) {
        // Behavior
        chbCloseOnEscape.IsChecked = Settings.Defaults.CloseOnEscape;
        chbLoadLast.IsChecked = Settings.Defaults.LoadLast;
        chbShowStart.IsChecked = Settings.Defaults.ShowStart;;

        // Special
        chbSyncX11PrimaryClipboard.IsChecked = Settings.Defaults.SyncX11PrimaryClipboard;
    }

    public void btnSave_Click(object sender, RoutedEventArgs e) {
        // Behavior
        Settings.CloseOnEscape = chbCloseOnEscape.IsChecked!.Value;
        Settings.LoadLast = chbLoadLast.IsChecked!.Value;
        Settings.ShowStart = chbShowStart.IsChecked!.Value;

        // Special
        Settings.SyncX11PrimaryClipboard = chbSyncX11PrimaryClipboard.IsChecked!.Value;

        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
