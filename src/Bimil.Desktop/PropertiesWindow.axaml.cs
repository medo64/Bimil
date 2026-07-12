namespace Bimil;

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Medo.Security.Cryptography.PasswordSafe;

internal partial class PropertiesWindow : Window {

    public PropertiesWindow(State state) {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

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

        var staticKey = StaticKey.GetStaticKeyAsBase58(state.Document);
        if (staticKey != null) {
            txtStaticKey.Text = staticKey;
            chbStaticKeyUse.IsChecked = true;
        } else {
            txtStaticKey.Text = "";
            chbStaticKeyUse.IsChecked = false;
        }
        chbStaticKeyUse.IsEnabled = !(Document?.IsReadOnly ?? true);
        chbStaticKeyUse.IsCheckedChanged += chbStaticKeyUse_IsCheckedChanged;  // assigned late to avoid triggering when set the first time

        AvaloniaHelpers.FocusControl(btnClose);
    }

    private Document? Document;


    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }

    private bool hadAnyStaticKeyChanges = false;

    public void chbStaticKeyUse_IsCheckedChanged(object? sender, RoutedEventArgs e) {
        hadAnyStaticKeyChanges = true;
        if (chbStaticKeyUse.IsChecked == true) {
            txtStaticKey.Text = StaticKey.GetNewStaticKeyAsBase58();
        } else {
            txtStaticKey.Text = "";
        }
    }


    public void btnSave_Click(object? sender, RoutedEventArgs e) {
        if (Document != null) {
            Document.Name = txtName.Text ?? "";
            Document.Description = txtDescription.Text ?? "";
            if (hadAnyStaticKeyChanges) {
                if (chbStaticKeyUse.IsChecked == true) {
                    StaticKey.RemoveStaticKey(Document);
                } else {
                    StaticKey.SetStaticKey(Document, txtStaticKey.Text);
                }
            }
        }

        txtStaticKey.Text = "";
        GC.Collect();

        Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }
}
