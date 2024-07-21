namespace Bimil.Desktop;

using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

internal partial class OptionsWindow : Window {
    public OptionsWindow() {
        InitializeComponent();

        Helpers.GetControl<CheckBox>(this, "chbCloseOnEscape").IsChecked = Settings.CloseOnEscape;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            Helpers.GetControl<CheckBox>(this, "chbSyncX11PrimaryClipboard").IsChecked = Settings.SyncX11PrimaryClipboard;
        } else {
            Helpers.GetControl<CheckBox>(this, "chbSyncX11PrimaryClipboard").IsVisible = false;
        }

        Helpers.FocusControl(this, "btnClose");
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }


    #region Buttons

    public void btnDefaults_Click(object sender, RoutedEventArgs e) {
        Helpers.GetControl<CheckBox>(this, "chbCloseOnEscape").IsChecked = false;
        Helpers.GetControl<CheckBox>(this, "chbSyncX11PrimaryClipboard").IsChecked = true;
    }

    public void btnSave_Click(object sender, RoutedEventArgs e) {
        Settings.CloseOnEscape = Helpers.GetControl<CheckBox>(this, "chbCloseOnEscape").IsChecked!.Value;
        Settings.SyncX11PrimaryClipboard = Helpers.GetControl<CheckBox>(this, "chbSyncX11PrimaryClipboard").IsChecked!.Value;
        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

    #endregion Buttons

}
