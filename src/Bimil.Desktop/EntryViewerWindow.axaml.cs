namespace Bimil;

internal partial class EntryViewerWindow : EntryCommonWindow {

    public EntryViewerWindow(EntryRecord entry)
        : base(entry, isReadOnly: true, document: null) {

        Title = "Entry Viewer";
        btnOk.IsVisible = false;
        btnCancel.Content = "Close";
        btnFields.IsVisible = false;
        AvaloniaHelpers.FocusControl(btnCancel);
    }

}
