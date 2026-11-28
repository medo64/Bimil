namespace Bimil;

using System.ComponentModel;
using System.Reflection;
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

        // Behavior
        SetTooltip(chbCloseOnEscape, nameof(Settings.Current.CloseOnEscape));
        SetTooltip(chbLoadLast, nameof(Settings.Current.LoadLast));
        SetTooltip(chbShowStart, nameof(Settings.Current.ShowStart));
        SetTooltip(chbSyncPrimaryClipboard, nameof(Settings.Current.SyncPrimaryClipboard));

        // UI
        SetTooltip(cmbTheme, nameof(Settings.Current.Theme));
        SetTooltip(chbEditableByDefault, nameof(Settings.Current.EditableByDefault));

        // Two-factor
        SetTooltip(chbUseNtp, nameof(Settings.Current.AlwaysUseNtpForTwoFactor));
        SetTooltip(txtNtpServer, nameof(Settings.Current.NtpServer));
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
        chbEditableByDefault.IsChecked = Settings.Current.EditableByDefault;

        // Two-factor
        chbUseNtp.IsChecked = Settings.Current.AlwaysUseNtpForTwoFactor;
        txtNtpServer.Text = Settings.Current.NtpServer;
    }

    public void btnDefaults_Click(object sender, RoutedEventArgs e) {
        // Behavior
        chbCloseOnEscape.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.CloseOnEscape));
        chbLoadLast.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.LoadLast));
        chbShowStart.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.ShowStart));
        chbSyncPrimaryClipboard.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.SyncPrimaryClipboard));

        // UI
        cmbTheme.SelectedIndex = (int)Settings.FindDefault<Settings.ThemeVariant>(nameof(Settings.Current.Theme));
        chbEditableByDefault.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.EditableByDefault));

        // Two-factor
        chbUseNtp.IsChecked = Settings.FindDefault<bool>(nameof(Settings.Current.AlwaysUseNtpForTwoFactor));
        txtNtpServer.Text = Settings.FindDefault<string>(nameof(Settings.Current.NtpServer));
    }

    public void btnSave_Click(object sender, RoutedEventArgs e) {
        // Behavior
        Settings.Current.CloseOnEscape = chbCloseOnEscape.IsChecked!.Value;
        Settings.Current.LoadLast = chbLoadLast.IsChecked!.Value;
        Settings.Current.ShowStart = chbShowStart.IsChecked!.Value;
        Settings.Current.SyncPrimaryClipboard = chbSyncPrimaryClipboard.IsChecked!.Value;

        // UI
        Settings.Current.Theme = (Settings.ThemeVariant)cmbTheme.SelectedIndex;
        Settings.Current.EditableByDefault = chbEditableByDefault.IsChecked!.Value;

        // Two-factor
        Settings.Current.AlwaysUseNtpForTwoFactor = chbUseNtp.IsChecked ?? false;
        Settings.Current.NtpServer = txtNtpServer.Text ?? "";

        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }


    public static void SetTooltip(Control control, string propertyName) {
        var property = typeof(Settings).GetProperty(propertyName);
        if (property == null) { return; }

        var attribute = property.GetCustomAttribute<DescriptionAttribute>();
        if (attribute == null) { return; }

        ToolTip.SetTip(control, attribute.Description);
    }

}
