namespace Bimil;

internal partial class EntryEditorWindow : EntryCommonWindow {

    public EntryEditorWindow(EntryRecord entry, Document document)
        : base(entry, isReadOnly: false, document) {

        AvaloniaHelpers.FocusControl(txtTitle);
    }

}
