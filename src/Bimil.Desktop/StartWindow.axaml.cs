namespace Bimil.Desktop;

using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Medo.Configuration;
using Metsys.Bson;

internal partial class StartWindow : Window {

    public StartWindow() {
        InitializeComponent();
        var lsbFiles = Helpers.GetControl<ListBox>(this, "lsbFiles");
        var btnOpen = Helpers.GetControl<Button>(this, "btnOpen");
        var btnOpenReadonly = Helpers.GetControl<Button>(this, "btnOpenReadonly");
        var btnClose = Helpers.GetControl<Button>(this, "btnClose");

        lsbFiles.SelectionChanged += (sender, e) => {
            var hasSelection = (lsbFiles.SelectedItem != null);
            btnOpen.IsEnabled = hasSelection;
            btnOpenReadonly.IsEnabled = hasSelection;
        };

        lsbFiles.DoubleTapped += (sender, e) => {
            if (lsbFiles.SelectedItem is StackPanel stack) {
                if (stack.Children[1] is TextBlock pathBlock) {
                    var path = pathBlock.Text;
                    if (File.Exists(path)) {
                        SelectedFile = new FileInfo(path);
                        Close();
                    }
                }
            }
        };

        foreach (var file in RecentFiles.GetFiles()) {
            var stack = new StackPanel() { Tag = file };
            var titleBlock = new Label() { Content = file.Name, FontSize = FontSize * 1.25 };
            var pathBlock = new TextBlock() { Text = file.FullName, FontSize = FontSize * 0.75 };
            if (!file.Exists) { pathBlock.Foreground = Brushes.Red; }

            stack.Children.Add(titleBlock);
            stack.Children.Add(pathBlock);
            lsbFiles.Items.Add(stack);
            lsbFiles.DoubleTapped += (sender, e) => {
                SelectedFile = file;
                Close();
            };
        }

        if (lsbFiles.Items.Count > 0) {
            lsbFiles.SelectedIndex = 0;
            Helpers.FocusControl(btnOpen);
        } else {
            Helpers.FocusControl(btnClose);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.Key == Key.Escape) { Close(); }
        base.OnKeyDown(e);
    }


    public FileInfo? SelectedFile { get; private set; }
    public bool SelectedReadonly { get; private set; }


    public void btnOpen_Click(object sender, RoutedEventArgs e) {
        if (lsbFiles.SelectedItem is StackPanel stack) {
            SelectedFile = (FileInfo)stack.Tag!;
            Close();
        }
    }

    public void btnOpenReadonly_Click(object sender, RoutedEventArgs e) {
        if (lsbFiles.SelectedItem is StackPanel stack) {
            SelectedFile = (FileInfo)stack.Tag!;
            SelectedReadonly = true;
            Close();
        }
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
