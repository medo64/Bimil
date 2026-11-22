namespace Bimil;

using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

internal partial class OptionsGridWindow : Window {
    public OptionsGridWindow() {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

        AvaloniaHelpers.FocusControl(btnClose);
        prgSettings.DataContext = Settings.Current;
    }


    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
