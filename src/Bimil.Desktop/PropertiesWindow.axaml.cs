namespace Bimil;

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

internal partial class PropertiesWindow : Window {

    public PropertiesWindow(Document document) {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

        Document = document;

        txtFileName.Text = document.File?.FullName ?? "";

        txtID.Text = document.Uuid.ToString();
        txtName.Text = document.Name ?? "";
        txtDescription.Text = document.Description ?? "";

        txtSaveApplication.Text = document.LastSaveApplication ?? "";
        txtSaveUser.Text = document.LastSaveUser ?? "";
        txtSaveHost.Text = document.LastSaveHost;
        if (document?.LastSaveTime != null) {
            txtSaveTime.Text = document.LastSaveTime.Value.ToShortDateString() + " " + document.LastSaveTime.Value.ToLongTimeString();
        }

     chbShowKey.IsCheckedChanged += chbShowKey_IsCheckedChanged; 

        AvaloniaHelpers.FocusControl(btnClose);
    }

    private Document Document;


    public void chbShowKey_IsCheckedChanged(object? sender, RoutedEventArgs e) {
        if (chbShowKey.IsChecked == true) {
            txtEncryptionKey.Text = Document.ActiveKeyBlock.KeyKAsBase58;
        } else {
            txtEncryptionKey.Text = "";
        }
    }


    public void btnSave_Click(object? sender, RoutedEventArgs e) {
        if (Document != null) {
            Document.Name = txtName.Text ?? "";
            Document.Description = txtDescription.Text ?? "";
        }

        txtEncryptionKey.Text = "";
        GC.Collect();

        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }
}
