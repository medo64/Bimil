namespace Bimil;

using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Medo;
using Medo.Avalonia;

internal partial class EntryCommonWindow : Window {

    protected EntryCommonWindow(EntryRecord entry, bool isReadOnly, Document? document) {
        InitializeComponent();
        Entry = (EntryRecord)entry.Clone();
        IsReadOnly = isReadOnly;
        Document = document;

        txtTitle.Text = Entry.Title;
        txtTitle.IsReadOnly = isReadOnly;

        if (isReadOnly) {
            txtGroup.Text = Entry.Group;
            txtGroup.IsReadOnly = isReadOnly;
            acbGroup.IsVisible = false;
            txtGroup.IsVisible = true;
        } else {
            if (document is not null) {
                btnRenameGroup.IsVisible = true;
                Replenishment.FillGroups(document, acbGroup);
            }
            acbGroup.Text = Entry.Group;
        }

        AddHandler(InputElement.KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);

        btnAutoType.Click += btnAutoType_Click;
        btnFields.Click += btnFields_Click;
        btnOk.Click += btnOk_Click;
        btnCancel.Click += btnCancel_Click;

        txtTitle.TextChanged += (_, _) => {
            Entry.Title = txtTitle.Text ?? "";
        };
        acbGroup.TextChanged += (_, _) => {
            Entry.Group = acbGroup.Text ?? "";
        };

        ReplenishEntry(Entry);
    }

    public EntryRecord Entry { get; }
    private readonly bool IsReadOnly;
    private readonly Document? Document;


    #region Events

    private void Window_KeyDown(object? sender, KeyEventArgs e) {
        Debug.WriteLine($"{GetType().Name}_KeyDown: {(e.KeyModifiers != KeyModifiers.None ? $"{e.KeyModifiers}+" : "")}{e.Key}");
        e.Handled = true;  // assume it is handled
        switch ((e.KeyModifiers, e.Key)) {
            case (KeyModifiers.None, Key.Escape): btnCancel_Click(null, null!); break;
            case (KeyModifiers.None, Key.F2): if (!IsReadOnly) { btnFields_Click(null, null!); } break;
            case (KeyModifiers.Alt, Key.S): if (!IsReadOnly) { btnOk_Click(null, null!); } break;
            default: e.Handled = false; break;
        }
    }

    public void btnAutoType_Click(object? sender, RoutedEventArgs e) {
    }

    public async void btnFields_Click(object? sender, RoutedEventArgs e) {
        var fields = Entry != null ? Entry.Fields.ToArray() : [];
        var frm = new FieldEditorWindow(fields);
        if (await frm.ShowDialog<bool>(this)) {
            if (Entry is not null) {
                Entry.Fields.Clear();
                Entry.Fields.AddRange(frm.Fields);
                ReplenishEntry(Entry);
            }
        }
    }

    public void btnOk_Click(object? sender, RoutedEventArgs e) {
        Close(true);
    }

    public void btnCancel_Click(object? sender, RoutedEventArgs e) {
        Close(false);
    }

    #endregion Events


    private void AddUnknownText(TextField field, bool isReadOnly) {
        var control = AddRow<TextBox>(field.Caption);
        control.Text = field.Text;
        control.IsReadOnly = isReadOnly;
        control.TextChanged += (_, _) => {
            field.Text = control.Text ?? "";
        };
    }

    private TextBox AddPlainText(TextField field, bool isReadOnly) {
        var control = AddPlainText(field.Caption, field.Text, isReadOnly);
        control.TextChanged += (_, _) => {
            field.Text = control.Text ?? "";
        };
        return control;
    }

    private TextBox AddPlainText(CustomTextField field, bool isReadOnly) {
        var control = AddPlainText(field.Caption, field.Text, isReadOnly);
        control.TextChanged += (_, _) => {
            field.Text = control.Text ?? "";
        };
        return control;
    }

    private TextBox AddPlainText(string caption, string text, bool isReadOnly) {
        var buttonCopy = GetButton("EditCopy");
        var control = AddRow<TextBox>(caption, buttonCopy);
        control.IsReadOnly = isReadOnly;
        control.Text = text;

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

    private TextBox AddPasswordText(TextField field, bool isReadOnly) {
        var control = AddPasswordText(field.Caption, field.Text, isReadOnly);
        control.TextChanged += (_, _) => {
            field.Text = control.Text ?? "";
        };
        return control;
    }

    private TextBox AddPasswordText(CustomTextField field, bool isReadOnly) {
        var control = AddPasswordText(field.Caption, field.Text, isReadOnly);
        control.TextChanged += (_, _) => {
            field.Text = control.Text ?? "";
        };
        return control;
    }

    private TextBox AddPasswordText(string caption, string text, bool isReadOnly) {
        var buttonView = GetButton("EditView");
        var buttonCopy = GetButton("EditCopy");
        var control = AddRow<TextBox>(caption, buttonView, buttonCopy);
        control.IsReadOnly = isReadOnly;
        control.PasswordChar = '•';
        control.Text = text;

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
            data.Add(DataTransferItem.CreateText(control.Text ?? ""));
            Clipboard?.SetDataAsync(data);
        };

        return control;
    }

    private TextBox AddUrlText(TextField field, string defaultProtocol, bool isReadOnly) {
        var buttonLink = GetButton("LinkUrl");
        var buttonCopy = GetButton("EditCopy");
        var control = AddRow<TextBox>(field.Caption, buttonLink, buttonCopy);
        control.IsReadOnly = isReadOnly;
        control.Text = field.Text;

        control.TextChanged += (sender, args) => {
            var hasData = !string.IsNullOrEmpty(control.Text);
            buttonLink.IsEnabled = hasData;
            buttonCopy.IsEnabled = hasData;
        };

        buttonLink.Click += (sender, args) => {
            if (Uri.TryCreate(control.Text, UriKind.Absolute, out var uri)) {
            } else if (Uri.TryCreate(defaultProtocol + "://" + control.Text, UriKind.Absolute, out uri)) {  // try with default protocol
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

        control.TextChanged += (_, _) => {
            field.Text = control.Text ?? "";
        };
        return control;
    }

    private (TextBox, TextBox) AddTwoFactorText(BinaryField field, bool isReadOnly) {
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
        control2FA.IsReadOnly = isReadOnly;
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
            var otp = AvaloniaHelpers.GetOtp(this, Otp.ConvertSecretFromBase32(control.Text));
            MessageBox.ShowInfoDialog(this, "Two-factor code", "Code: " + otp.GetCodeAsText(CodeOutputFormat.Spaced) + "\n\n" + otp.Time.ToLocalTime().ToString("yyyy-MM-dd\nHH:mm:ss"));
        };

        buttonCopy.Click += (sender, args) => {
            var otp = AvaloniaHelpers.GetOtp(this, Otp.ConvertSecretFromBase32(control.Text));
            var data = new DataTransfer();
            data.Add(DataTransferItem.CreateText(otp.GetCodeAsText()));
            Clipboard?.SetDataAsync(data);
        };

        control.TextChanged += (_, _) => {
            // TODO
        };
        return (control, control2FA);
    }

    private TextBox AddMultilineText(TextField field, int lineCount, bool isReadOnly) {
        var control = AddRow<TextBox>(field.Caption);
        control.IsReadOnly = isReadOnly;
        control.AcceptsReturn = true;
        control.TextWrapping = TextWrapping.Wrap;
        control.MaxHeight = control.MinHeight * lineCount;
        control.MinHeight = control.MinHeight * (lineCount - 1);
        control.Text = field.Text;
        control.TextChanged += (_, _) => {
            field.Text = control.Text ?? "";
        };
        return control;
    }

    private int NextTabIndex = 0;

    private T AddRow<T>(string? caption, params Button[] buttons) where T : Control, new() {
        grdRecords.RowDefinitions.Add(new RowDefinition());
        var label = new Label() {
            Content = (caption ?? "Unknown") + ":",
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
            Grid.SetColumn(panel, 2);

            control.SizeChanged += (sender, args) => {
                foreach (var button in buttons) {
                    button.MaxWidth = control.Bounds.Height;
                    button.MaxHeight = control.Bounds.Height;
                }
            };
        } else {
            grdRecords.Children.Add(control);
            Grid.SetRow(control, row);
            Grid.SetColumn(control, 2);
        }

        return control;
    }

    private static Button GetButton(string bitmapName) {
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

    private void ReplenishEntry(EntryRecord entry) {
        for (var i = grdRecords.Children.Count - 1; i >= 5; i--) {
            grdRecords.Children.RemoveAt(i);
        }
        for (var i = grdRecords.RowDefinitions.Count - 1; i >= 4; i--) {
            grdRecords.RowDefinitions.RemoveAt(i);
        }

        foreach (var field in entry.Fields) {
            if (!Filtering.IsFieldAppendable(field)) { continue; }

            switch (field.Type) {
                case FieldType.UserName: {
                        if (field is TextField textField) {
                            var control = AddPlainText(textField, IsReadOnly);
                            AvaloniaHelpers.RegisterForAlternateFont(control);
                        }
                    }
                    break;

                case FieldType.Password: {
                        if (field is TextField textField) {
                            var control = AddPasswordText(textField, IsReadOnly);
                            AvaloniaHelpers.RegisterForAlternateFont(control);
                        }
                    }
                    break;

                case FieldType.Url: {
                        if (field is TextField textField) {
                            AddUrlText(textField, defaultProtocol: "http", IsReadOnly);
                        }
                    }
                    break;

                case FieldType.EmailAddress: {
                        if (field is TextField textField) {
                            var control = AddUrlText(textField, defaultProtocol: "mailto", IsReadOnly);
                            AvaloniaHelpers.RegisterForAlternateFont(control);
                        }
                    }
                    break;

                case FieldType.TwoFactorKey: {
                        if (field is BinaryField binaryField) {
                            var (textBox, textBoxCode) = AddTwoFactorText(binaryField, IsReadOnly);
                            AvaloniaHelpers.RegisterForTwoFactorView(textBox, textBoxCode, binaryField);
                        }
                    }
                    break;

                case FieldType.Notes: {
                        if (field is TextField textField) {
                            AddMultilineText(textField, Settings.Current.NotesLineCount, IsReadOnly);
                        }
                    }
                    break;

                case FieldType.CustomTextField: {
                        if (field is CustomTextField customTextField) {
                            var control = customTextField.IsSensitive ? AddPasswordText(customTextField, IsReadOnly) : AddPlainText(customTextField, IsReadOnly);
                            AvaloniaHelpers.RegisterForAlternateFont(control);
                        }
                    }
                    break;

                default: {
                        if (field is TextField textField) {
                            AddUnknownText(textField, IsReadOnly);
                        }
                    }
                    break;
            }
        }
    }

}
