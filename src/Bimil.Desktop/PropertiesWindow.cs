namespace Bimil;

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Medo.Security.Cryptography.PasswordSafe;

internal partial class PropertiesWindow : Window {

    public PropertiesWindow(State state) {
        InitializeComponent();

        PropertyChanged += (sender, e) => {  // close on minimize
            if (e.Property == Window.WindowStateProperty && WindowState == WindowState.Minimized) {
                Dispatcher.UIThread.InvokeAsync(() => {
                    WindowState = WindowState.Normal;
                    Close();
                });
            }
        };

        Document = state.Document;

        txtFileName.Text = state.File?.FullName ?? "";

        txtID.Text = state.Document?.Uuid.ToString() ?? "";
        txtName.Text = state.Document?.Name ?? "";
        txtDescription.Text = state.Document?.Description ?? "";

        txtSaveApplication.Text = state.Document?.LastSaveApplication ?? "";
        txtSaveUser.Text = state.Document?.LastSaveUser ?? "";
        txtSaveHost.Text = state.Document?.LastSaveHost ?? "";
        if ((state.Document?.LastSaveTime != null) && (state.Document?.LastSaveTime > DateTime.MinValue)) {
            txtSaveTime.Text = state.Document.LastSaveTime.ToShortDateString() + " " + state.Document.LastSaveTime.ToLongTimeString();
        }

        // TODO: static keys

        AvaloniaHelpers.FocusControl(btnClose);
    }

    private Document? Document;


    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }


    public void btnSave_Click(object sender, RoutedEventArgs e) {
        if (Document != null) {
            Document.Name = txtName.Text ?? "";
            Document.Description = txtDescription.Text ?? "";
        }
        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }
}
