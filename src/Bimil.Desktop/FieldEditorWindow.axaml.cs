namespace Bimil;

using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

internal partial class FieldEditorWindow : Window {

    public FieldEditorWindow(Field[] fields) {
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
            lsbFields.Items.Add(new ItemEnvelope(field, i));
        }

        AddHandler(InputElement.KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);

        lsbFields.DoubleTapped += btnEdit_Click;
        lsbFields.SelectionChanged += lsbFields_SelectionChanged;
        lsbFields.AddHandler(InputElement.PointerPressedEvent, lsbFields_PointerPressed, RoutingStrategies.Bubble, handledEventsToo: true);
        lsbFields.AddHandler(InputElement.PointerMovedEvent, lsbFields_PointerMoved, RoutingStrategies.Bubble, handledEventsToo: true);
        lsbFields.AddHandler(InputElement.PointerReleasedEvent, lsbFields_PointerReleased, RoutingStrategies.Bubble, handledEventsToo: true);
        lsbFields.AddHandler(InputElement.PointerExitedEvent, lsbFields_PointerExited, RoutingStrategies.Bubble, handledEventsToo: true);

        btnAdd.Click += btnAdd_Click;
        btnEdit.Click += btnEdit_Click;
        btnRemove.Click += btnRemove_Click;
        btnOk.Click += btnOk_Click;
        btnCancel.Click += btnCancel_Click;

        if (lsbFields.Items.Count > 0) { lsbFields.SelectedIndex = 0; }

        AvaloniaHelpers.FocusControl(lsbFields);
    }

    public List<Field> Fields;


    #region Events

    private void Window_KeyDown(object? sender, KeyEventArgs e) {
        Debug.WriteLine($"{GetType().Name}_KeyDown: {(e.KeyModifiers != KeyModifiers.None ? $"{e.KeyModifiers}+" : "")}{e.Key}");
        e.Handled = true;  // assume it is handled
        switch ((e.KeyModifiers, e.Key)) {
            case (KeyModifiers.None, Key.Escape): btnCancel_Click(null, null!); break;
            case (KeyModifiers.None, Key.Insert): btnAdd_Click(null, null!); break;
            case (KeyModifiers.None, Key.F4): btnEdit_Click(null, null!); break;
            case (KeyModifiers.None, Key.Delete): btnRemove_Click(null, null!); break;
            case (KeyModifiers.Alt, Key.Up): btnMoveUp_Click(null, null!); break;
            case (KeyModifiers.Alt, Key.Down): btnMoveDown_Click(null, null!); break;
            case (KeyModifiers.Alt, Key.S): btnOk_Click(null, null!); break;
            default: e.Handled = false; break;
        }
    }

    public void lsbFields_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
        var item = lsbFields.SelectedItem as ItemEnvelope;
        var hasSelection = item is not null;
        btnEdit.IsEnabled = hasSelection;
        btnRemove.IsEnabled = hasSelection;
    }

    public async void btnAdd_Click(object? sender, RoutedEventArgs e) {
        var frm = new FieldWindow();
        if (await frm.ShowDialog<bool>(this)) {
            var newField = frm.Field!;
            lsbFields.Items.Add(new ItemEnvelope(newField, Fields.Count));
            Fields.Add(newField);
            btnOk.IsEnabled = true;
        }
    }

    public async void btnEdit_Click(object? sender, RoutedEventArgs e) {
        if (lsbFields.SelectedItem is not ItemEnvelope item) { return; }
        var itemIndex = lsbFields.Items.IndexOf(item);
        var frm = new FieldWindow(item.Field);
        if (await frm.ShowDialog<bool>(this)) {
            var newField = frm.Field!;
            Fields[item.FieldIndex] = newField;
            lsbFields.Items[itemIndex] = new ItemEnvelope(newField, item.FieldIndex);
            btnOk.IsEnabled = true;
        }
    }

    public void btnRemove_Click(object? sender, RoutedEventArgs e) {
        if (lsbFields.SelectedItem is not ItemEnvelope item) { return; }
        var itemIndex = lsbFields.Items.IndexOf(item);
        lsbFields.Items.RemoveAt(itemIndex);
        Fields.RemoveAt(item.FieldIndex);
        for (var i = itemIndex; i < lsbFields.Items.Count; i++) {
            var iItem = (ItemEnvelope)(lsbFields.Items[i])!;
            iItem.FieldIndex -= 1;
        }
        btnOk.IsEnabled = true;
    }

    public void btnMoveUp_Click(object? sender, RoutedEventArgs e) {
        if (lsbFields.SelectedItem is not ItemEnvelope itemSrc) { return; }
        var itemSrcIndex = lsbFields.Items.IndexOf(itemSrc);
        if (itemSrcIndex == 0) { return; }
        var itemDst = (ItemEnvelope)lsbFields.Items[itemSrcIndex - 1]!;
        var itemDstIndex = lsbFields.Items.IndexOf(itemDst);

        Debug.WriteLine($"Reorder: {itemDstIndex}/{itemDst.FieldIndex} <- {itemSrcIndex}/{itemSrc.FieldIndex}");
        (Fields[itemSrc.FieldIndex], Fields[itemDst.FieldIndex]) = (Fields[itemDst.FieldIndex], Fields[itemSrc.FieldIndex]);
        (itemSrc.FieldIndex, itemDst.FieldIndex) = (itemDst.FieldIndex, itemSrc.FieldIndex);
        (lsbFields.Items[itemSrcIndex], lsbFields.Items[itemDstIndex]) = (lsbFields.Items[itemDstIndex], lsbFields.Items[itemSrcIndex]);
        lsbFields.SelectedItem = itemSrc;

        btnOk.IsEnabled = true;
    }

    public void btnMoveDown_Click(object? sender, RoutedEventArgs e) {
        if (lsbFields.SelectedItem is not ItemEnvelope itemSrc) { return; }
        var itemSrcIndex = lsbFields.Items.IndexOf(itemSrc);
        if (itemSrcIndex == lsbFields.Items.Count - 1) { return; }
        var itemDst = (ItemEnvelope)lsbFields.Items[itemSrcIndex + 1]!;
        var itemDstIndex = lsbFields.Items.IndexOf(itemDst);

        Debug.WriteLine($"Reorder: {itemSrcIndex}/{itemSrc.FieldIndex} -> {itemDstIndex}/{itemDst.FieldIndex}");
        (Fields[itemSrc.FieldIndex], Fields[itemDst.FieldIndex]) = (Fields[itemDst.FieldIndex], Fields[itemSrc.FieldIndex]);
        (itemSrc.FieldIndex, itemDst.FieldIndex) = (itemDst.FieldIndex, itemSrc.FieldIndex);
        (lsbFields.Items[itemSrcIndex], lsbFields.Items[itemDstIndex]) = (lsbFields.Items[itemDstIndex], lsbFields.Items[itemSrcIndex]);
        lsbFields.SelectedItem = itemSrc;

        btnOk.IsEnabled = true;
    }

    public void btnOk_Click(object? sender, RoutedEventArgs e) {
        if (!btnOk.IsEnabled) { return; }
        Close(true);
    }

    public void btnCancel_Click(object? sender, RoutedEventArgs e) {
        Close(false);
    }

    #endregion Events

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
            btnOk.IsEnabled = true;
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


    private class ItemEnvelope : TextBlock {

        public ItemEnvelope(Field field, int fieldIndex) {
            Field = field;
            FieldIndex = fieldIndex;
            Text = Field.Caption;
        }

        public Field Field { get; }
        public int FieldIndex { get; set; }

    }

}
