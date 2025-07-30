namespace Bimil;

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Medo.Security.Cryptography.PasswordSafe;

internal partial class PropertiesWindow : Window {

    public PropertiesWindow(Document? document) {
        InitializeComponent();

        Document = document;

        txtID.Text = document?.Uuid.ToString() ?? "";
        txtName.Text = document?.Name ?? "";
        txtDescription.Text = document?.Description ?? "";

        txtSaveApplication.Text = document?.LastSaveApplication ?? "";
        txtSaveUser.Text = document?.LastSaveUser ?? "";
        txtSaveHost.Text = document?.LastSaveHost ?? "";
        if ((document?.LastSaveTime != null) && (document?.LastSaveTime > DateTime.MinValue)) {
            txtSaveTime.Text = document.LastSaveTime.ToShortDateString() + " " + document.LastSaveTime.ToLongTimeString();
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
