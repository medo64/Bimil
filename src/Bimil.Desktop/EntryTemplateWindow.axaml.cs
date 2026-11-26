namespace Bimil;

using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

internal partial class EntryTemplateWindow : Window {

    public EntryTemplateWindow() {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

        cmbTemplates.Items.Add(new ItemEnvelope(
            "User name and password",
            Field.Create(FieldType.UserName),
            Field.Create(FieldType.Password),
            Field.Create(FieldType.Notes)
        ));
        cmbTemplates.Items.Add(new ItemEnvelope(
            "User name and password with 2FA",
            Field.Create(FieldType.UserName),
            Field.Create(FieldType.Password),
            Field.Create(FieldType.TwoFactorKey),
            Field.Create(FieldType.Notes)
        ));
        cmbTemplates.Items.Add(new ItemEnvelope(
            "Credit card",
            Field.Create(FieldType.CreditCardNumber),
            Field.Create(FieldType.CreditCardExpiration),
            Field.Create(FieldType.CreditCardVerificationValue),
            Field.Create(FieldType.CreditCardPin),
            Field.Create(FieldType.Notes)
        ));

        AddHandler(InputElement.KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);

        btnOk.Click += btnOk_Click;
        btnCancel.Click += btnCancel_Click;

        if (cmbTemplates.Items.Count > 0) { cmbTemplates.SelectedItem = cmbTemplates.Items[0]; }

        AvaloniaHelpers.FocusControl(cmbTemplates);
    }

    public EntryRecord? Entry { get; private set; }


    #region Events

    private void Window_KeyDown(object? sender, KeyEventArgs e) {
        Debug.WriteLine($"{GetType().Name}_KeyDown: {(e.KeyModifiers != KeyModifiers.None ? $"{e.KeyModifiers}+" : "")}{e.Key}");
        e.Handled = true;  // assume it is handled
        switch ((e.KeyModifiers, e.Key)) {
            case (KeyModifiers.None, Key.Escape): btnCancel_Click(null, null!); break;
            case (KeyModifiers.None, Key.Enter):
            case (KeyModifiers.Alt, Key.S):
                btnOk_Click(null, null!); break;
            default: e.Handled = false; break;
        }
    }

    public void btnOk_Click(object? sender, RoutedEventArgs e) {
        if (!btnOk.IsEnabled) { return; }
        if (cmbTemplates.SelectedItem is not ItemEnvelope item) { return; }
        Entry = (EntryRecord)EntryRecord.Create();
        Entry.Fields.AddRange(item.Fields);
        Close(true);
    }

    public void btnCancel_Click(object? sender, RoutedEventArgs e) {
        Close(false);
    }

    #endregion Events


    private class ItemEnvelope : TextBlock {

        public ItemEnvelope(string caption, params Field[] fields) {
            Fields = fields;
            Text = caption;
        }

        public Field[] Fields { get; }

    }

}
