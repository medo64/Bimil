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

internal partial class EntryWindow : Window {

    public EntryWindow(Document document) {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

        Document = document;

        Replenishment.FillGroups(document, cmbGroups);

        Title = "New";
        AvaloniaHelpers.FocusControl(txtTitle);
    }

    public readonly Document Document;


    public EntryWindow(Document document, EntryRecord entry, bool readOnly = false) {
        InitializeComponent();
        Document = document;
        Entry = entry;

        Replenishment.FillGroups(document, cmbGroups);

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

        foreach (var field in entry.Fields) {
            switch (field.Type) {
                case FieldType.Uuid:
                case FieldType.Group:
                case FieldType.Title:
                case FieldType.CreationTime:
                case FieldType.LastAccessTime:
                case FieldType.LastModificationTime:
                case FieldType.PasswordExpiryTime:
                case FieldType.PasswordModificationTime:
                case FieldType.PasswordHistory:
                case FieldType.Autotype:
                    continue;

                case FieldType.UserName: {
                        if (field is TextField textField) {
                            var control = AddPlainText(textField);
                            AvaloniaHelpers.RegisterForAlternateFont(control);
                        }
                    }
                    break;

                case FieldType.Password: {
                        if (field is TextField textField) {
                            var control = AddPasswordText(textField);
                            AvaloniaHelpers.RegisterForAlternateFont(control);
                        }
                    }
                    break;

                case FieldType.Url: {
                        if (field is TextField textField) {
                            AddUrlText(textField);
                        }
                    }
                    break;

                case FieldType.EmailAddress: {
                        if (field is TextField textField) {
                            var control = AddUrlText(textField, protocol: "mailto");
                            AvaloniaHelpers.RegisterForAlternateFont(control);
                        }
                    }
                    break;

                case FieldType.TwoFactorKey: {
                        if (field is BinaryField binaryField) {
                            var (textBox, textBoxCode) = AddTwoFactorText(binaryField);
                            AvaloniaHelpers.RegisterForTwoFactorView(textBox, textBoxCode, binaryField);
                        }
                    }
                    break;

                case FieldType.Notes: {
                        if (field is TextField textField) {
                            AddMultilineText(textField, Settings.NotesLineCount);
                        }
                    }
                    break;

                default: {
                        if (field is TextField textField) {
                            AddUnknownText(textField);
                        }
                    }
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

    private readonly EntryRecord? Entry;

    public void btnOK_Click(object sender, RoutedEventArgs e) {
    }

    public void btnCancel_Click(object sender, RoutedEventArgs e) {
        Close();
    }

    public void btnAutoType_Click(object sender, RoutedEventArgs e) {
    }

    public async void btnFields_Click(object sender, RoutedEventArgs e) {
        var fields = Entry != null ? Entry.Fields.ToArray() : [];
        var frm = new FieldsWindow(fields);
        await frm.ShowDialog(this);
    }


    private void AddUnknownText(TextField field) {
        var control = AddRow<TextBox>(field.Caption);
        control.Text = field.Text;
    }

    private TextBox AddPlainText(TextField field) {
        var buttonCopy = GetButton("EditCopy");
        var control = AddRow<TextBox>(field.Caption, buttonCopy);
        control.Text = field.Text;

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


    private TextBox AddPasswordText(TextField text) {
        var buttonView = GetButton("EditView");
        var buttonCopy = GetButton("EditCopy");
        var control = AddRow<TextBox>(text.Caption, buttonView, buttonCopy);
        control.PasswordChar = '•';
        control.Text = text.Text;

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
            data.Add(DataTransferItem.CreateText(text.Text));
            Clipboard?.SetDataAsync(data);
        };

        return control;
    }

    private TextBox AddUrlText(TextField field, string protocol = "http") {
        var buttonLink = GetButton("LinkUrl");
        var buttonCopy = GetButton("EditCopy");
        var control = AddRow<TextBox>(field.Caption, buttonLink, buttonCopy);
        control.Text = field.Text;

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

    private (TextBox, TextBox) AddTwoFactorText(BinaryField field) {
        TimeBasedOtp otp;
        byte[] secret = field?.Data.GetBytes() ?? [];
        try {
            otp = new TimeBasedOtp(secret);
        } finally {
            Array.Clear(secret, 0, secret.Length);
        }

        var buttonView = GetButton("EditView");
        var buttonShow = GetButton("LinkCode");
        var buttonCopy = GetButton("EditCopy2FA");
        var control = AddRow<TextBox>(field?.Caption ?? "", buttonView, buttonShow, buttonCopy);

        var control2FA = new TextBox();
        Grid.SetColumn(control2FA, Grid.GetColumn(control));
        Grid.SetRow(control2FA, Grid.GetRow(control));
        control2FA.TabIndex = control.TabIndex;
        control2FA.IsReadOnly = true;
        control2FA.IsVisible = false;
        control2FA.TextAlignment = TextAlignment.Center;
        var panel = (Panel)control.Parent!;
        panel.Children.Insert(panel.Children.IndexOf(control), control2FA);

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

        return (control, control2FA);
    }

    private TextBox AddMultilineText(TextField text, int lineCount) {
        var control = AddRow<TextBox>(text.Caption);
        control.AcceptsReturn = true;
        control.TextWrapping = TextWrapping.Wrap;
        control.MaxHeight = control.MinHeight * lineCount;
        control.MinHeight = control.MinHeight * (lineCount - 1);
        control.Text = text.Text;
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

        var control = new T() {
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
