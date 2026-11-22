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

    protected override async void OnKeyDown(KeyEventArgs e) {
        switch ((e.KeyModifiers, e.Key)) {
            case (KeyModifiers.None, Key.Escape):
                btnClose_Click(null!, null!);
                break;

            case (KeyModifiers.Control, Key.OemComma):
                var frm = new OptionsGridWindow();
                await frm.ShowDialog(this);
                Close();
                break;

            default: base.OnKeyDown(e); return;
        }

        e.Handled = true;
    }


    private void LoadState() {
        // Behavior
        chbCloseOnEscape.IsChecked = Settings.Current.CloseOnEscape;
        chbLoadLast.IsChecked = Settings.Current.LoadLast;
        chbShowStart.IsChecked = Settings.Current.ShowStart;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            chbSyncPrimaryClipboard.IsChecked = Settings.Current.SyncPrimaryClipboard;
        } else {
            chbSyncPrimaryClipboard.IsVisible = false;
        }

        // UI
        cmbTheme.SelectedIndex = (int)Settings.Current.Theme;
    }

    public void btnDefaults_Click(object sender, RoutedEventArgs e) {
        // Behavior
        chbCloseOnEscape.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.CloseOnEscape));
        chbLoadLast.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.LoadLast));
        chbShowStart.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.ShowStart));
        chbSyncPrimaryClipboard.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.SyncPrimaryClipboard));

        // UI
        cmbTheme.SelectedIndex = (int)Settings.FindDefault<Settings.ThemeVariant>(nameof(Settings.Current.Theme));
    }

    public void btnSave_Click(object sender, RoutedEventArgs e) {
        // Behavior
        Settings.Current.CloseOnEscape = chbCloseOnEscape.IsChecked!.Value;
        Settings.Current.LoadLast = chbLoadLast.IsChecked!.Value;
        Settings.Current.ShowStart = chbShowStart.IsChecked!.Value;
        Settings.Current.SyncPrimaryClipboard = chbSyncPrimaryClipboard.IsChecked!.Value;

        // UI
        Settings.Current.Theme = (Settings.ThemeVariant)cmbTheme.SelectedIndex;

        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
