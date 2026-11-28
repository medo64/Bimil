namespace Bimil;

using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

internal partial class FindWindow : Window {
    public FindWindow(Document document) {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);
        AvaloniaHelpers.FocusControl(txtFilter);

        Document = document;

        txtFilter.TextChanged += (_, _) => { ReplenishEntries(); };
        chbSearchSensitive.IsCheckedChanged += (_, _) => { ReplenishEntries(); };
        btnClose.Click += (_, _) => { Close(); };
        lsbEntries.DoubleTapped += (_, _) => { lsbEntries_DoubleTapped(); };
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        switch ((e.KeyModifiers, e.Key)) {
            case (KeyModifiers.None, Key.Escape):
                Close();
                break;

            case (KeyModifiers.None, Key.Enter):
                lsbEntries_DoubleTapped();
                break;

            default: base.OnKeyDown(e); return;
        }

        e.Handled = true;
    }

    private readonly Document Document;


    private void ReplenishEntries() {
        var includeSensitive = chbSearchSensitive.IsChecked ?? false;
        var words = (txtFilter.Text ?? "").Split(' ', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

        lsbEntries.Items.Clear();
        if (words.Length == 0) { return; }

        foreach (var entry in Document.GetEntryList()) {
            var matchCount = 0;
            foreach (var word in words) {
                foreach (var field in entry.Fields) {
                    if (field is IReadableIsSensitive fieldWithSensitivity) {
                        if (fieldWithSensitivity.IsSensitive && !includeSensitive) { continue; }
                    }

                    if (field is IReadableCaption fieldWithCaption && fieldWithCaption.Caption.Contains(word, StringComparison.CurrentCultureIgnoreCase)) { matchCount += 1; break; }
                    if (field is IReadableText fieldWithText && fieldWithText.Text.Contains(word, StringComparison.CurrentCultureIgnoreCase)) { matchCount += 1; break; }
                }
            }

            if (matchCount == words.Length) {
                var titleBlock = new TextBlock() {
                    Text = entry.Title,
                    FontSize = FontSize * 1.25,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                var groupBlock = new TextBlock() {
                    Text = entry.Group ?? "(no group)",
                    TextAlignment = TextAlignment.Right,
                    FontSize = FontSize * 0.75,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                var dock = new DockPanel();
                dock.Children.Add(titleBlock);
                dock.Children.Add(groupBlock);
                DockPanel.SetDock(titleBlock, Dock.Left);
                DockPanel.SetDock(groupBlock, Dock.Right);

                var newLBItem = new ListBoxItem { Content = dock, Tag = entry };
                lsbEntries.Items.Add(newLBItem);
            }

            if ((lsbEntries.SelectedIndex < 0) && (lsbEntries.Items.Count > 0)) {
                lsbEntries.SelectedIndex = 0;
            }
        }
    }

    private async void lsbEntries_DoubleTapped() {
        if (lsbEntries.SelectedItem is ListBoxItem { Tag: EntryRecord selectedEntry }) {
            var frm = new EntryEditorWindow(selectedEntry, startReadonly: true, Document);
            await frm.ShowDialog(this);
        }

    }
}
