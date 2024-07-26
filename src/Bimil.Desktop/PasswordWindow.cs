namespace Bimil.Desktop;

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

internal partial class PasswordWindow : Window {
    private enum PasswordWindowType { New, Enter, Change, ChangeInFile }

    public static PasswordWindow GetNewPasswordWindow(string title) {
        return new PasswordWindow(title, PasswordWindowType.New);
    }

    public static PasswordWindow GetEnterPasswordWindow(string title) {
        return new PasswordWindow(title, PasswordWindowType.Enter);
    }

    public static PasswordWindow GetChangePasswordWindow(string title, bool hasFile) {
        return new PasswordWindow(title, hasFile ? PasswordWindowType.ChangeInFile : PasswordWindowType.Change);
    }

    private PasswordWindow(string title, PasswordWindowType type) {
        InitializeComponent();
        Title = title;

        switch (type) {
            case PasswordWindowType.New:
                lblPasswordNew.IsVisible = true;
                txtPasswordNew.IsVisible = true;
                lblPasswordCompare.IsVisible = true;
                txtPasswordCompare.IsVisible = true;
                txtPasswordNew.TextChanged += (sender, e) => { CheckPasswordsAreSame(btnOK, txtPasswordNew, txtPasswordCompare, lblPasswordMismatch); };
                txtPasswordCompare.TextChanged += (sender, e) => { CheckPasswordsAreSame(btnOK, txtPasswordNew, txtPasswordCompare, lblPasswordMismatch); };
                btnOK.Click += (sender, e) => {
                    NewPassword = txtPasswordNew.Text ?? "";  // Text is null if not changed
                    Close();
                };
                Helpers.FocusControl(txtPasswordNew);  // FocusFirstControl(this);
                break;

            case PasswordWindowType.Enter:
                lblPasswordExisting.IsVisible = true;
                txtPasswordExisting.IsVisible = true;
                btnOK.Click += (sender, e) => {
                    ExistingPassword = txtPasswordExisting.Text ?? "";  // Text is null if not changed
                    Close();
                };
                Helpers.FocusControl(txtPasswordExisting);  // FocusFirstControl(this);
                break;

            case PasswordWindowType.Change:
                lblPasswordNew.IsVisible = true;
                txtPasswordNew.IsVisible = true;
                lblPasswordCompare.IsVisible = true;
                txtPasswordCompare.IsVisible = true;
                txtPasswordNew.TextChanged += (sender, e) => { CheckPasswordsAreSame(btnOK, txtPasswordNew, txtPasswordCompare, lblPasswordMismatch); };
                txtPasswordCompare.TextChanged += (sender, e) => { CheckPasswordsAreSame(btnOK, txtPasswordNew, txtPasswordCompare, lblPasswordMismatch); };
                btnOK.Click += (sender, e) => {
                    NewPassword = txtPasswordNew.Text ?? "";  // Text is null if not changed
                    Close();
                };
                Helpers.FocusControl(txtPasswordNew);  // FocusFirstControl(this);
                break;

            case PasswordWindowType.ChangeInFile:
                lblPasswordExisting.IsVisible = true;
                txtPasswordExisting.IsVisible = true;
                lblPasswordNew.IsVisible = true;
                txtPasswordNew.IsVisible = true;
                lblPasswordCompare.IsVisible = true;
                txtPasswordCompare.IsVisible = true;
                txtPasswordNew.TextChanged += (sender, e) => { CheckPasswordsAreSame(btnOK, txtPasswordNew, txtPasswordCompare, lblPasswordMismatch); };
                txtPasswordCompare.TextChanged += (sender, e) => { CheckPasswordsAreSame(btnOK, txtPasswordNew, txtPasswordCompare, lblPasswordMismatch); };
                btnOK.Click += (sender, e) => {
                    ExistingPassword = txtPasswordExisting.Text ?? "";  // Text is null if not changed
                    NewPassword = txtPasswordNew.Text ?? "";  // Text is null if not changed
                    Close();
                };
                Helpers.FocusControl(txtPasswordExisting);  // FocusFirstControl(this);
                break;

        }
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }


    private static void CheckPasswordsAreSame(Button okButton, TextBox textBox1, TextBox textBox2, Label mismatchLabel) {
        var areSame = string.Equals(textBox1.Text ?? "", textBox2.Text ?? "", StringComparison.Ordinal);
        okButton.IsEnabled = areSame;
        mismatchLabel.IsVisible = !areSame;
    }


    public string? ExistingPassword { get; private set; }
    public string? NewPassword { get; private set; }


    public void txtPassword_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            if (btnOK.IsEnabled) {
                btnOK.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }

    public void btnCancel_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
