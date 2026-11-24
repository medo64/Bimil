namespace Bimil;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

internal partial class FieldsWindow : Window {

    public FieldsWindow(Field[] fields) {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);

        Fields = [.. fields];

        InputElement.KeyDownEvent.AddClassHandler<TopLevel>((sender, e) => {
            if ((e.KeyModifiers == KeyModifiers.None) && (e.Key == Key.Enter)) {
                OnKeyDown(e);
            }
        }, handledEventsToo: true);

        for (var i = 0; i < fields.Length; i++) {
            var field = fields[i];
            if (!Filtering.IsFieldAppendable(field)) { continue; }
            lsbFields.Items.Add(new ItemEnvelope(field, i) { Text = field.Caption });
        }

        lsbFields.SelectionChanged += lsbFields_SelectionChanged;
        lsbFields.AddHandler(InputElement.PointerPressedEvent, lsbFields_PointerPressed, RoutingStrategies.Bubble, handledEventsToo: true);
        lsbFields.AddHandler(InputElement.PointerMovedEvent, lsbFields_PointerMoved, RoutingStrategies.Bubble, handledEventsToo: true);
        lsbFields.AddHandler(InputElement.PointerReleasedEvent, lsbFields_PointerReleased, RoutingStrategies.Bubble, handledEventsToo: true);
        lsbFields.AddHandler(InputElement.PointerExitedEvent, lsbFields_PointerExited, RoutingStrategies.Bubble, handledEventsToo: true);

        btnAdd.Click += btnAdd_Click;
        btnRemove.Click += btnRemove_Click;
        btnSave.Click += btnSave_Click;
        btnClose.Click += btnClose_Click;

        AvaloniaHelpers.FocusControl(btnClose);
    }

    public List<Field> Fields;


    #region Drag&Drop

    private int? DragSrcIndex;

    private async void lsbFields_PointerPressed(object? sender, PointerPressedEventArgs e) {
        if (e.Properties.IsLeftButtonPressed) {
            var itemIndex = GetIndexAt(e.GetPosition(lsbFields));
            if (itemIndex is not null) { DragSrcIndex = itemIndex; }
        }
    }

    private async void lsbFields_PointerMoved(object? sender, PointerEventArgs e) {
        if (DragSrcIndex is null) { return; }

        var dragDstIndex = GetIndexAt(e.GetPosition(lsbFields));
        if (dragDstIndex is not null) {
            if (DragSrcIndex != dragDstIndex) {
                SetCursor(DragSrcIndex > dragDstIndex ? StandardCursorType.TopSide : StandardCursorType.BottomSide);
            } else {
                SetCursor(StandardCursorType.DragMove);
            }
        } else {
            SetCursor(null);
        }
    }

    private async void lsbFields_PointerExited(object? sender, PointerEventArgs e) {
        SetCursor(null);
    }

    private async void lsbFields_PointerReleased(object? sender, PointerReleasedEventArgs e) {
        if (DragSrcIndex is null) { return; }
        var dragDstIndex = GetIndexAt(e.GetPosition(lsbFields));
        if ((dragDstIndex is not null) && (dragDstIndex != DragSrcIndex)) {
            if (DragSrcIndex < dragDstIndex) {
                Debug.WriteLine($"Drag&Drop: {DragSrcIndex} -> {dragDstIndex}");
                for (var pivot = DragSrcIndex.Value; pivot < dragDstIndex; pivot++) {  // do it one by one
                    Debug.WriteLine($"Drag&Drop: Swap {pivot}/{pivot + 1}");
                    var item1 = (ItemEnvelope)lsbFields.Items[pivot]!;
                    var item2 = (ItemEnvelope)lsbFields.Items[pivot + 1]!;
                    lsbFields.Items.Insert(pivot, item2);
                    lsbFields.Items.RemoveAt(pivot + 2);
                    (Fields[item1.FieldIndex], Fields[item2.FieldIndex]) = (Fields[item2.FieldIndex], Fields[item1.FieldIndex]);
                    (item1.FieldIndex, item2.FieldIndex) = (item2.FieldIndex, item1.FieldIndex);
                }
            } else {
                Debug.WriteLine($"Drag&Drop: {dragDstIndex} <- {DragSrcIndex}");
                for (var pivot = DragSrcIndex.Value - 1; pivot >= dragDstIndex; pivot--) {  // do it one by one
                    Debug.WriteLine($"Drag&Drop: Swap {pivot + 1}/{pivot}");
                    var item1 = (ItemEnvelope)lsbFields.Items[pivot]!;
                    var item2 = (ItemEnvelope)lsbFields.Items[pivot + 1]!;
                    lsbFields.Items.Insert(pivot, item2);
                    lsbFields.Items.RemoveAt(pivot + 2);
                    (Fields[item1.FieldIndex], Fields[item2.FieldIndex]) = (Fields[item2.FieldIndex], Fields[item1.FieldIndex]);
                    (item1.FieldIndex, item2.FieldIndex) = (item2.FieldIndex, item1.FieldIndex);
                }
            }
            btnSave.IsEnabled = true;
        }
        DragSrcIndex = null;
        SetCursor(null);
    }

    private StandardCursorType? LastCursor;

    private void SetCursor(StandardCursorType? newCursorType) {
        if (LastCursor != newCursorType) {
            Debug.WriteLine($"Cursor: {newCursorType?.ToString() ?? "(null)"}");
            if (newCursorType != null) {
                lsbFields.Cursor = new Cursor(newCursorType.Value);
            } else {
                lsbFields.Cursor = null;
            }
            LastCursor = newCursorType;
        }
    }

    private int? GetIndexAt(Point position) {
        var hit = lsbFields.InputHitTest(position) as Visual;
        var item = hit?.FindAncestorOfType<ListBoxItem>();
        if (item == null) { return null; }

        var itemIndex = lsbFields.IndexFromContainer(item);
        return (itemIndex >= 0) ? itemIndex : null;
    }

    #endregion Drag&Drop

    #region Events

    public void lsbFields_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        var item = lsbFields.SelectedItem as ItemEnvelope;
        var hasSelection = item is not null;
        btnRemove.IsEnabled = hasSelection;
    }

    public void btnAdd_Click(object? sender, RoutedEventArgs e) {
        // TODO
        btnSave.IsEnabled = true;
    }

    public void btnRemove_Click(object? sender, RoutedEventArgs e) {
        if (lsbFields.SelectedItem is not ItemEnvelope item) { return; }
        lsbFields.Items.Remove(item);
        Fields.RemoveAt(item.FieldIndex);
        btnSave.IsEnabled = true;
    }

    public void btnSave_Click(object? sender, RoutedEventArgs e) {
        Close(true);
    }

    public void btnClose_Click(object? sender, RoutedEventArgs e) {
        Close(false);
    }

    #endregion Events


    private class ItemEnvelope : TextBlock {

        public ItemEnvelope(Field field, int fieldIndex) {
            Field = field;
            FieldIndex = fieldIndex;
        }

        public Field Field { get; }
        public int FieldIndex { get; set; }

    }

}
