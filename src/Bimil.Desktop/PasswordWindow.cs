namespace Bimil.Desktop;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Bimil.Core;
using Medo.X11;

internal partial class PasswordWindow : Window {
    private enum PasswordWindowType { New, Enter, Change }

    public static PasswordWindow GetNewPasswordWindow() {
        return new PasswordWindow(PasswordWindowType.New);
    }

    public static PasswordWindow GetEnterPasswordWindow() {
        return new PasswordWindow(PasswordWindowType.Enter);
    }

    public static PasswordWindow GetChangePasswordWindow() {
        return new PasswordWindow(PasswordWindowType.Change);
    }

    private PasswordWindow(PasswordWindowType type) {
        InitializeComponent();

        var lblPasswordExisting = Helpers.GetControl<Label>(this, "lblPasswordExisting");
        var txtPasswordExisting = Helpers.GetControl<TextBox>(this, "txtPasswordExisting");
        var lblPasswordNew = Helpers.GetControl<Label>(this, "lblPasswordNew");
        var txtPasswordNew = Helpers.GetControl<TextBox>(this, "txtPasswordNew");
        var lblPasswordCompare = Helpers.GetControl<Label>(this, "lblPasswordCompare");
        var txtPasswordCompare = Helpers.GetControl<TextBox>(this, "txtPasswordCompare");
        var lblPasswordMismatch = Helpers.GetControl<Label>(this, "lblPasswordMismatch");
        var btnOK = Helpers.GetControl<Button>(this, "btnOK");

        switch (type) {
            case PasswordWindowType.New:
                lblPasswordNew.IsVisible = true;
                txtPasswordNew.IsVisible = true;
                lblPasswordCompare.IsVisible = true;
                txtPasswordCompare.IsVisible = true;
                txtPasswordNew.TextChanged += (sender, e) => { CheckPasswordsAreSame(btnOK, txtPasswordNew, txtPasswordCompare, lblPasswordMismatch); };
                txtPasswordCompare.TextChanged += (sender, e) => { CheckPasswordsAreSame(btnOK, txtPasswordNew, txtPasswordCompare, lblPasswordMismatch); };
                btnOK.Click += (sender, e) => { Password = txtPasswordNew.Text; Close(); };
                Helpers.FocusControl(this, "txtPasswordNew");  // FocusFirstControl(this);
                break;

            case PasswordWindowType.Enter:
                lblPasswordExisting.IsVisible = true;
                txtPasswordExisting.IsVisible = true;
                btnOK.Click += (sender, e) => { Password = txtPasswordExisting.Text; Close(); };
                Helpers.FocusControl(this, "txtPasswordExisting");  // FocusFirstControl(this);
                break;

            case PasswordWindowType.Change:
                break;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }


    private void CheckPasswordsAreSame(Button okButton, TextBox textBox1, TextBox textBox2, Label mismatchLabel) {
        var areSame = string.Equals(textBox1.Text, textBox2.Text, StringComparison.Ordinal);
        okButton.IsEnabled = areSame;
        mismatchLabel.IsVisible = !areSame;
    }


    public string? Password { get; private set; }


    public void txtPassword_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            var btnOK = Helpers.GetControl<Button>(this, "btnOK");
            btnOK.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }

    public void btnCancel_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
