namespace Bimil;

using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Medo;
using Medo.Avalonia;
using Medo.Security.Cryptography;
using Medo.Security.Cryptography.PasswordSafe;

internal partial class EntryWindow : Window {

    public EntryWindow(State state) {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

        State = state;

        Replenishment.FillGroups(state, cmbGroups);

        Title = "New";
        AvaloniaHelpers.FocusControl(txtTitle);
    }

    public readonly State State;


    public EntryWindow(State state, Entry entry, bool readOnly = false) {
        InitializeComponent();
        State = state;

        Replenishment.FillGroups(state, cmbGroups);

        if (readOnly) {
            Title = "View";
            btnOK.IsVisible = false;
            btnCancel.Content = "Close";
            btnFields.IsVisible = false;
            AvaloniaHelpers.FocusControl(btnCancel);
        } else {
            Title = "Edit";
            AvaloniaHelpers.FocusControl(txtTitle);
        }

        txtTitle.Text = entry.Title;
        cmbGroups.Text = entry.Group;

        foreach (var record in entry.Records) {
            switch (record.RecordType) {
                case RecordType.Uuid:
                case RecordType.Group:
                case RecordType.Title:
                case RecordType.CreationTime:
                case RecordType.LastAccessTime:
                case RecordType.LastModificationTime:
                case RecordType.PasswordExpiryTime:
                case RecordType.PasswordModificationTime:
                case RecordType.PasswordHistory:
                case RecordType.Autotype:
                    continue;

                case RecordType.UserName: {
                        var control = AddPlainText(record);
                        AvaloniaHelpers.RegisterForAlternateFont(control);
                    }
                    break;

                case RecordType.Password: {
                        var control = AddPasswordText(record);
                        AvaloniaHelpers.RegisterForAlternateFont(control);
                    }
                    break;

                case RecordType.Url:
                    AddUrlText(record);
                    break;

                case RecordType.EmailAddress: {
                        var control = AddUrlText(record, protocol: "mailto");
                        AvaloniaHelpers.RegisterForAlternateFont(control);
                    }
                    break;

                case RecordType.TwoFactorKey:
                    AddTwoFactorText(record);
                    break;

                case RecordType.Notes:
                    AddMultilineText(record);
                    break;

                default:
                    AddUnknownText(record);
                    break;
            }
        }

    }


    protected override void OnKeyDown(KeyEventArgs e) {
        switch (e.Key) {
            case Key.Escape: Close(); break;

            default: base.OnKeyDown(e); break;
        }
    }


    public void btnOK_Click(object sender, RoutedEventArgs e) {
    }

    public void btnCancel_Click(object sender, RoutedEventArgs e) {
        Close();
    }

    public void btnAutoType_Click(object sender, RoutedEventArgs e) {
    }

    public void btnFields_Click(object sender, RoutedEventArgs e) {
    }


    private void AddUnknownText(Record record) {
        var control = AddRow<TextBox>(record.Caption);
        control.Text = record.Text;
    }

    private TextBox AddPlainText(Record record) {
        var buttonCopy = GetButton("EditCopy");
        var control = AddRow<TextBox>(record.Caption, buttonCopy);
        control.Text = record.Text;

        control.TextChanged += (sender, args) => {
            var hasData = !string.IsNullOrEmpty(control.Text);
            buttonCopy.IsEnabled = hasData;
        };

        buttonCopy.Click += (sender, args) => {
            var data = new DataTransfer();
            data.Add(DataTransferItem.CreateText(control.Text));
            Clipboard?.SetDataAsync(data);
        };

        return control;
    }


    private TextBox AddPasswordText(Record record) {
        var buttonView = GetButton("EditView");
        var buttonCopy = GetButton("EditCopy");
        var control = AddRow<TextBox>(record.Caption, buttonView, buttonCopy);
        control.PasswordChar = '•';
        control.Text = record.Text;

        control.TextChanged += (sender, args) => {
            var hasData = !string.IsNullOrEmpty(control.Text);
            buttonView.IsEnabled = hasData;
            buttonCopy.IsEnabled = hasData;
        };

        buttonView.Click += (sender, args) => {
            control.RevealPassword = !control.RevealPassword;
        };

        buttonCopy.Click += (sender, args) => {
            var data = new DataTransfer();
            data.Add(DataTransferItem.CreateText(record.Text));
            Clipboard?.SetDataAsync(data);
        };

        return control;
    }

    private TextBox AddUrlText(Record record, string protocol = "http") {
        var buttonLink = GetButton("LinkUrl");
        var buttonCopy = GetButton("EditCopy");
        var control = AddRow<TextBox>(record.Caption, buttonLink, buttonCopy);
        control.Text = record.Text;

        control.TextChanged += (sender, args) => {
            var hasData = !string.IsNullOrEmpty(control.Text);
            buttonLink.IsEnabled = hasData;
            buttonCopy.IsEnabled = hasData;
        };

        buttonLink.Click += (sender, args) => {
            if (Uri.TryCreate(control.Text, UriKind.Absolute, out var uri)) {
            } else if (Uri.TryCreate(protocol + "://" + control.Text, UriKind.Absolute, out uri)) {  // try with default protocol
            } else { return; }  // cannot figure the URL

            try {
                Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
            } catch { }
        };

        buttonCopy.Click += (sender, args) => {
            var data = new DataTransfer();
            data.Add(DataTransferItem.CreateText(control.Text));
            Clipboard?.SetDataAsync(data);
        };

        return control;
    }

    private TextBox AddTwoFactorText(Record record) {
        TimeBasedOtp otp;
        byte[] secret = record?.GetBytes() ?? [];
        try {
            otp = new TimeBasedOtp(secret);
        } finally {
            Array.Clear(secret, 0, secret.Length);
        }

        var buttonView = GetButton("EditView");
        var buttonShow = GetButton("LinkCode");
        var buttonCopy = GetButton("EditCopy2FA");
        var control = AddRow<TextBox>(record?.Caption ?? "", buttonView, buttonShow, buttonCopy);
        control.PasswordChar = '•';
        control.Text = otp.GetSecretAsText();

        control.TextChanged += (sender, args) => {
            var hasData = !string.IsNullOrEmpty(control.Text);
            buttonView.IsEnabled = hasData;
            buttonShow.IsEnabled = hasData;
            buttonCopy.IsEnabled = hasData;
        };

        buttonView.Click += (sender, args) => {
            control.RevealPassword = !control.RevealPassword;
        };

        buttonShow.Click += (sender, args) => {
            var time = DateTime.UtcNow;  // TODO: check against server
            var otp = new TimeBasedOtp(control.Text) {
                Time = time
            };

            MessageBox.ShowInfoDialog(this, "Two-factor code", "Code: " + otp.GetCodeAsText(CodeOutputFormat.Spaced) + "\n\n" + time.ToString("yyyy-MM-dd\nHH:mm:ss"));
        };

        buttonCopy.Click += (sender, args) => {
            var time = DateTime.UtcNow;  // TODO: check against server
            var otp = new TimeBasedOtp(control.Text) {
                Time = time
            };

            var data = new DataTransfer();
            data.Add(DataTransferItem.CreateText(otp.GetCodeAsText()));
            Clipboard?.SetDataAsync(data);
        };

        return control;
    }

    private TextBox AddMultilineText(Record record) {
        var control = AddRow<TextBox>(record.Caption);
        control.AcceptsReturn = true;
        control.TextWrapping = TextWrapping.Wrap;
        control.MaxHeight = control.MinHeight * 3;
        control.MinHeight = control.MinHeight * 2;
        control.Text = record.Text;
        return control;
    }

    private int NextTabIndex = 0;

    private T AddRow<T>(string? caption, params Button[] buttons) where T : Control, new() {
        grdRecords.RowDefinitions.Add(new RowDefinition());
        var label = new Label() {
            Content = caption ?? "Unknown",
            TabIndex = NextTabIndex++,
        };

        var row = grdRecords.RowDefinitions.Count - 1;
        grdRecords.Children.Add(label);
        Grid.SetRow(label, row);
        Grid.SetColumn(label, 0);

        var control = new T(){
            TabIndex = NextTabIndex++,
        };
        if ((buttons is not null) && (buttons.Length > 0)) {
            var panel = new DockPanel() {
                LastChildFill = true,
            };
            foreach (var button in buttons) {
                button.TabIndex = NextTabIndex++;
                panel.Children.Insert(0, button);
                DockPanel.SetDock(button, Dock.Right);
            }
            panel.Children.Add(control);
            grdRecords.Children.Add(panel);
            Grid.SetRow(panel, row);
            Grid.SetColumn(panel, 1);

            control.SizeChanged += (sender, args) => {
                foreach (var button in buttons) {
                    button.MaxWidth = control.Bounds.Height;
                    button.MaxHeight = control.Bounds.Height;
                }
            };
        } else {
            grdRecords.Children.Add(control);
            Grid.SetRow(control, row);
            Grid.SetColumn(control, 1);
        }

        return control;
    }

    private Button GetButton(string bitmapName) {
        var button = new Button {
            Padding = new Thickness(2),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
        };
        button.SizeChanged += (sender, args) => {
            var bitmap = ThemeImageResources.GetBitmap(bitmapName, button, out var scale);
            var image = new Image() {
                Width = bitmap.Size.Width / scale,
                Height = bitmap.Size.Height / scale,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Source = bitmap
            };
            button.Content = image;
        };
        return button;
    }

}
