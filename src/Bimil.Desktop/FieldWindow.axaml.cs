namespace Bimil;

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;

internal partial class FieldWindow : Window {

    public FieldWindow(Field? field = null) {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

        ItemEnvelope? selectedItem = null;

        foreach (var type in Enum.GetValues<FieldType>()) {
            if (type == FieldType.EndOfEntry) { continue; }
            var item = Field.Create(type);
            if (Filtering.IsFieldAppendable(item)) {
                var newItem = new ItemEnvelope(item);
                if (item.Type == field?.Type) { selectedItem = newItem; }
                cmbFields.Items.Add(newItem);
            }
        }

        Field = field?.Clone();
        btnSave.Content = Field is null ? "_Add" : "_Edit";
        Title = Field is null ? "Add Field" : "Edit Field";

        txtCaption.Text = Field?.Caption ?? "Custom";
        if (field is IReadableIsSensitive fieldSensitivity) {
            chbIsSensitive.IsChecked = fieldSensitivity.IsSensitive;
        }

        cmbFields.SelectionChanged += cmbFields_SelectionChanged;
        btnSave.Click += btnSave_Click;
        btnCancel.Click += btnCancel_Click;

        if (cmbFields.Items.Count > 0) { cmbFields.SelectedItem = selectedItem ?? cmbFields.Items[0]; }

        AvaloniaHelpers.FocusControl(btnCancel);
    }

    public Field? Field { get; private set; }


    #region Events

    public void cmbFields_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        if (cmbFields.SelectedItem is not ItemEnvelope item) { return; }
        var isCustomText = item.Field.Type == FieldType.CustomTextField;
        lblCaption.IsVisible = isCustomText;
        txtCaption.IsVisible = isCustomText;
        lblIsSensitive.IsVisible = isCustomText;
        chbIsSensitive.IsVisible = isCustomText;
    }

    public void btnSave_Click(object? sender, RoutedEventArgs e) {
        if (cmbFields.SelectedItem is not ItemEnvelope item) { return; }
        var newField = item.Field;
        if (newField is IWritableText writableText && Field is IReadableText readableText) { writableText.Text = readableText.Text; }
        if (newField is IWritableCaption writableCaption) { writableCaption.Caption = txtCaption.Text ?? "Custom"; }
        if (newField is IWritableIsSensitive writableIsSensitive) { writableIsSensitive.IsSensitive = chbIsSensitive.IsChecked ?? false; }
        Field = newField;
        Close(true);
    }

    public void btnCancel_Click(object? sender, RoutedEventArgs e) {
        Close(false);
    }

    #endregion Events


    private class ItemEnvelope : TextBlock {

        public ItemEnvelope(Field field) {
            Field = field;
            Text = field.Caption;
        }

        public Field Field { get; }

    }

}
