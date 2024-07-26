namespace Bimil.Desktop;

using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Medo.Avalonia;
using Medo.Configuration;

internal partial class StartWindow : Window {

    public StartWindow() {
        InitializeComponent();
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
        switch (e.Key) {
            case Key.Escape: Close(); break;
            case Key.Enter: btnOpen_Click(this, e); break;

            case Key.Up: if (lsbFiles.SelectedIndex > 0) { lsbFiles.SelectedIndex -= 1; } break;
            case Key.Down: if (lsbFiles.SelectedIndex < lsbFiles.Items.Count - 1) { lsbFiles.SelectedIndex += 1; } break;
            case Key.Left: {
                    if (btnOpen.IsFocused) {
                        Helpers.FocusControl(btnClose);
                    } else if (btnOpenReadonly.IsFocused) {
                        Helpers.FocusControl(btnOpen);
                    } else if (btnClose.IsFocused) {
                        Helpers.FocusControl(btnOpenReadonly);
                    }
                }
                break;
            case Key.Right: {
                    if (btnOpen.IsFocused) {
                        Helpers.FocusControl(btnOpenReadonly);
                    } else if (btnOpenReadonly.IsFocused) {
                        Helpers.FocusControl(btnClose);
                    } else if (btnClose.IsFocused) {
                        Helpers.FocusControl(btnOpen);
                    }
                }
                break;

            case Key.Delete:
                if (lsbFiles.SelectedItem is StackPanel stack) {
                    if (MessageBox.ShowQuestionDialog(this,
                                                     "Remove from recent files",
                                                     "Do you really want to remove file from the most-recent list?",
                                                     "Yes", "No") == 0) {
                        var file = (FileInfo)stack.Tag!;
                        RecentFiles.Remove(file);
                        lsbFiles.Items.Remove(stack);
                    }
                }
                break;

            default: base.OnKeyDown(e); break;
        }
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
