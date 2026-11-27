namespace Bimil;

using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

internal partial class GroupRenameWindow : Window {

    public GroupRenameWindow(string group, Document document) {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

        NewGroup = group;
        Document = document;

        txtGroupOld.Text = group;
        txtGroupNew.Text = group;

        AddHandler(InputElement.KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);

        txtGroupNew.TextChanged += (_, _) => { btnOk.IsEnabled = !(txtGroupOld.Text ?? "").Equals(txtGroupNew.Text ?? "", StringComparison.Ordinal); };

        btnOk.Click += btnOk_Click;
        btnCancel.Click += btnCancel_Click;

        AvaloniaHelpers.FocusControl(txtGroupNew);
    }

    public string NewGroup { get; private set; }
    private readonly Document Document;


    #region Events

    private void Window_KeyDown(object? sender, KeyEventArgs e) {
        Debug.WriteLine($"{GetType().Name}_KeyDown: {(e.KeyModifiers != KeyModifiers.None ? $"{e.KeyModifiers}+" : "")}{e.Key}");
        e.Handled = true;  // assume it is handled
        switch ((e.KeyModifiers, e.Key)) {
            case (KeyModifiers.None, Key.Escape): btnCancel_Click(null, null!); break;
            case (KeyModifiers.None, Key.Return):
            case (KeyModifiers.Alt, Key.S): btnOk_Click(null, null!); break;
            default: e.Handled = false; break;
        }
    }

    public void btnOk_Click(object? sender, RoutedEventArgs e) {
        if (!btnOk.IsEnabled) { return; }

        var oldGroup = txtGroupOld.Text ?? "";
        var newGroup = txtGroupNew.Text ?? "";

        foreach (var entry in Document.GetEntryList()) {
            if (entry.Group.Equals(oldGroup, StringComparison.Ordinal)) {
                entry.Group = newGroup;
            }
        }

        NewGroup = newGroup;
        Close(true);
    }

    public void btnCancel_Click(object? sender, RoutedEventArgs e) {
        Close(false);
    }

    #endregion Events

}
