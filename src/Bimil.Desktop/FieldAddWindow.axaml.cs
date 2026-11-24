namespace Bimil;

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;

internal partial class FieldAddWindow : Window {

    public FieldAddWindow() {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);
        btnAdd.Click += btnAdd_Click;
        btnCancel.Click += btnCancel_Click;

        foreach (var type in Enum.GetValues<FieldType>()) {
            if (type == FieldType.EndOfEntry) { continue; }
            var field = Field.Create(type);
            if (Filtering.IsFieldAppendable(field)) {
                cmbFields.Items.Add(new ItemEnvelope(field) { Text = field.Caption });
            }
        }
        if (cmbFields.Items.Count > 0) { cmbFields.SelectedItem = cmbFields.Items[0]; }

        AvaloniaHelpers.FocusControl(btnAdd);
    }

    public Field? Field { get; private set; }


    #region Events

    public void btnAdd_Click(object? sender, RoutedEventArgs e) {
        if (cmbFields.SelectedItem is not ItemEnvelope item) { return; }
        Field = item.Field;
        Close(true);
    }

    public void btnCancel_Click(object? sender, RoutedEventArgs e) {
        Close(false);
    }

    #endregion Events


    private class ItemEnvelope : TextBlock {

        public ItemEnvelope(Field field) {
            Field = field;
        }

        public Field Field { get; }

    }

}
