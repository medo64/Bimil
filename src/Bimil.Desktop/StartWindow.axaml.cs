namespace Bimil;

using System;
using System.Collections;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Medo;
using Medo.Avalonia;

internal partial class StartWindow : Window {

    public StartWindow() {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);
        InputElement.KeyDownEvent.AddClassHandler<TopLevel>((sender, e) => {
            if ((e.KeyModifiers == KeyModifiers.None) && (e.Key == Key.Enter)) {
                OnKeyDown(e);
            }
        }, handledEventsToo: true);

        lsbFiles.SelectionChanged += (sender, e) => {
            var selectedStack = lsbFiles.SelectedItem as StackPanel;
            var hasSelection = (selectedStack?.Tag != null);
            btnOpen.IsEnabled = hasSelection;
            btnOpenReadonly.IsEnabled = hasSelection;
            mnuOpen.IsEnabled = hasSelection;
            mnuOpenReadOnly.IsEnabled = hasSelection;
            mnuRemove.IsEnabled = hasSelection;
        };


        lsbFiles.DoubleTapped += (sender, e) => {
            var point = e.GetPosition(lsbFiles);
            var hitTestResult = lsbFiles.InputHitTest(point);
            if (hitTestResult is ScrollContentPresenter) { return; }  // avoid double-tap on empty area

            if (lsbFiles.SelectedItem is StackPanel stack) {
                if ((stack.Children.Count > 1) && (stack.Children[1] is TextBlock pathBlock)) {
                    var path = pathBlock.Text;
                    if (File.Exists(path)) {
                        SelectedFile = new FileInfo(path);
                        Close();
                    }
                }
            }
        };

        foreach (var file in Config.Recent.Files) {
            var stack = new StackPanel() { Tag = file };
            var titleBlock = new Label() { Content = file.Name, FontSize = FontSize * 1.25 };
            var pathBlock = new TextBlock() { Text = file.FullName, FontSize = FontSize * 0.75 };
            if (!file.Exists) { pathBlock.Foreground = Brushes.Red; }

            stack.Children.Add(titleBlock);
            stack.Children.Add(pathBlock);
            lsbFiles.Items.Add(stack);
        }

        if (lsbFiles.Items.Count > 0) {
            lsbFiles.SelectedIndex = 0;
            AvaloniaHelpers.FocusControl(btnOpen);
        } else {
            var stack = new StackPanel() { Tag = null };
            var titleBlock = new Label() { Content = "No recent files", FontSize = FontSize * 1.25 };
            stack.Children.Add(titleBlock);
            lsbFiles.Items.Add(stack);
            AvaloniaHelpers.FocusControl(btnClose);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        switch ((e.Key, e.KeyModifiers)) {
            case (Key.Enter, KeyModifiers.None): btnOpen_Click(this, e); break;

            case (Key.Up, KeyModifiers.None): if (lsbFiles.SelectedIndex > 0) { lsbFiles.SelectedIndex -= 1; } break;
            case (Key.Down, KeyModifiers.None): if (lsbFiles.SelectedIndex < lsbFiles.Items.Count - 1) { lsbFiles.SelectedIndex += 1; } break;
            case (Key.Left, KeyModifiers.None): {
                    if (btnOpen.IsFocused) {
                        AvaloniaHelpers.FocusControl(btnClose);
                    } else if (btnOpenReadonly.IsFocused) {
                        AvaloniaHelpers.FocusControl(btnOpen);
                    } else if (btnClose.IsFocused) {
                        AvaloniaHelpers.FocusControl(btnOpenReadonly);
                    }
                }
                break;
            case (Key.Right, KeyModifiers.None): {
                    if (btnOpen.IsFocused) {
                        AvaloniaHelpers.FocusControl(btnOpenReadonly);
                    } else if (btnOpenReadonly.IsFocused) {
                        AvaloniaHelpers.FocusControl(btnClose);
                    } else if (btnClose.IsFocused) {
                        AvaloniaHelpers.FocusControl(btnOpen);
                    }
                }
                break;

            case (Key.Delete, KeyModifiers.None):
                mnuRemove_Click(null, e);
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

    public void mnuRemove_Click(object? sender, RoutedEventArgs e) {
        if (lsbFiles.SelectedItem is StackPanel stack) {
            if (MessageBox.ShowQuestionDialog(this,
                                             "Remove from recent files",
                                             "Do you really want to remove file from the most-recent list?",
                                             "Yes", "No") == 0) {
                var file = (FileInfo)stack.Tag!;
                Config.Recent.Files.Remove(file);
                lsbFiles.Items.Remove(stack);
            }
        }
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
