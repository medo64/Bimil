namespace Bimil;

using System;
using System.Net.Http;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Medo.Security.Cryptography.PasswordSafe;
using Metsys.Bson;

internal partial class EntryWindow : Window {

    public EntryWindow(State state) {
        InitializeComponent();
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

                case RecordType.Notes:
                    AddMultilineText(record);
                    break;

                default:
                    AddPlainText(record);
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


    private void AddPlainText(Record record) {
        var control = AddRow<TextBox>(Helpers.GetRecordCaption(record.RecordType));
        control.Text = record.Text;
    }

    private void AddMultilineText(Record record) {
        var control = AddRow<TextBox>(Helpers.GetRecordCaption(record.RecordType));
        control.AcceptsReturn = true;
        control.TextWrapping = TextWrapping.Wrap;
        control.MaxHeight = control.MinHeight * 3;
        control.MinHeight = control.MinHeight * 2;
        control.Text = record.Text;
    }

    private T AddRow<T>(string? caption, params Button[] buttons) where T : Control, new() {
        grdRecords.RowDefinitions.Add(new RowDefinition());
        var label = new Label() {
            Content = caption ?? "Unknown",
        };

        var row = grdRecords.RowDefinitions.Count - 1;
        grdRecords.Children.Add(label);
        Grid.SetRow(label, row);
        Grid.SetColumn(label, 0);

        var control = new T();
        if ((buttons is not null) && (buttons.Length > 0)) {
            var panel = new DockPanel() {
                LastChildFill = true,
            };
            foreach (var button in buttons) {
                panel.Children.Add(button);
                DockPanel.SetDock(button, Dock.Right);
            }
            panel.Children.Add(control);
            grdRecords.Children.Add(panel);
            Grid.SetRow(panel, row);
            Grid.SetColumn(panel, 1);
        } else {
            grdRecords.Children.Add(control);
            Grid.SetRow(control, row);
            Grid.SetColumn(control, 1);
        }

        return control;
    }

}
