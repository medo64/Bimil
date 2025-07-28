namespace Bimil;

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Medo.Security.Cryptography.PasswordSafe;
using Metsys.Bson;

internal partial class EntryWindow : Window {

    public EntryWindow() {
        InitializeComponent();
        Replenishment.FillGroups(cmbGroups);

        Title = "New";
        AvaloniaHelpers.FocusControl(txtTitle);
    }

    public EntryWindow(Entry entry, bool readOnly = false) {
        InitializeComponent();
        Replenishment.FillGroups(cmbGroups);

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

                default: {
                        grdRecords.RowDefinitions.Add(new RowDefinition());
                        var label = new Label() {
                            Content = Helpers.GetRecordCaption(record.RecordType),
                        };
                        var textBox = new TextBox() {
                            Text = record.Text,
                        };
                        var row = grdRecords.RowDefinitions.Count - 1;
                        grdRecords.Children.Add(label);
                        Grid.SetRow(label, row);
                        Grid.SetColumn(label, 0);
                        grdRecords.Children.Add(textBox);
                        Grid.SetRow(textBox, row);
                        Grid.SetColumn(textBox, 1);
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


    public void btnOK_Click(object sender, RoutedEventArgs e) {
    }

    public void btnCancel_Click(object sender, RoutedEventArgs e) {
        Close();
    }

    public void btnAutoType_Click(object sender, RoutedEventArgs e) {
    }

    public void btnFields_Click(object sender, RoutedEventArgs e) {
    }

}
