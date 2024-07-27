namespace Bimil.Desktop;

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Bimil.Core;

internal partial class PropertiesWindow : Window {
    public PropertiesWindow() {
        InitializeComponent();

        txtID.Text = State.Document?.Uuid.ToString() ?? "";
        txtName.Text = State.Document?.Name ?? "";
        txtDescription.Text = State.Document?.Description ?? "";

        txtSaveApplication.Text = State.Document?.LastSaveApplication ?? "";
        txtSaveUser.Text = State.Document?.LastSaveUser ?? "";
        txtSaveHost.Text = State.Document?.LastSaveHost ?? "";
        if ((State.Document?.LastSaveTime != null) && (State.Document?.LastSaveTime > DateTime.MinValue)) {
            txtSaveTime.Text = State.Document.LastSaveTime.ToShortDateString() + " " + State.Document.LastSaveTime.ToLongTimeString();
        }

        // TODO: static keys

        Helpers.FocusControl(btnClose);
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }


    public void btnSave_Click(object sender, RoutedEventArgs e) {
        if (State.Document != null) {
            State.Document.Name = txtName.Text ?? "";
            State.Document.Description = txtDescription.Text ?? "";
        }
        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }
}
