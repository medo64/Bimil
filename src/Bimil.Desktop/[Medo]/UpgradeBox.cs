/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2024-07-16: Waiting for dialog close
//            Move window outside of bounds
//2024-07-09: Initial Avalonia version

namespace Medo.Avalonia;

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Interactivity;
using global::Avalonia.Layout;
using global::Avalonia.Media;
using global::Avalonia.Styling;
using global::Avalonia.Threading;
using Medo.Application;

/// <summary>
/// Simple upgrade dialog.
/// Requires Medo.Application.Upgrade class.
/// </summary>
public static class UpgradeBox {

    /// <summary>
    /// Checks for upgrade.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="serviceUri">Service URI (e.g. https://medo64.com/upgrade/).</param>
    public static void ShowDialog(Window owner, Uri serviceUri) {
        ShowDialog(owner, serviceUri, Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Checks for upgrade.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="serviceUri">Service URI (e.g. https://medo64.com/upgrade/).</param>
    /// <param name="assembly">Assembly to use for checking.</param>
    public static void ShowDialog(Window owner, Uri serviceUri, Assembly assembly) {
        var window = new Window() { MinWidth = 600, MaxWidth = 1200 };
        if (owner != null) {
            window.Icon = owner.Icon;
            window.ShowInTaskbar = false;
            if (owner.Topmost) { window.Topmost = true; }
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        } else {  // just in case null is passed, not ideal
            window.ShowInTaskbar = true;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        window.Background = GetBrush("SystemChromeLowColor", Brushes.White, Brushes.Black);
        window.CanResize = false;
        window.ShowActivated = true;
        window.SizeToContent = SizeToContent.WidthAndHeight;
        window.SystemDecorations = SystemDecorations.BorderOnly;
        window.ExtendClientAreaToDecorationsHint = true;
        window.Title = "Upgrade";

        window.Opened += (sender, e) => {  // adjust position as needed
            var screen = window.Screens.ScreenFromWindow(window);
            if ((screen == null) || (window.FrameSize == null)) { return; }

            var pos = window.Position;
            var size = PixelSize.FromSize(window.FrameSize.Value, screen.Scaling);
            var rect = new PixelRect(pos, size);
            Debug.WriteLine(rect);

            if (rect.Right > screen.Bounds.Right) { rect = rect.WithX(screen.Bounds.Right - rect.Width); }
            if (rect.X < screen.Bounds.X) { rect = rect.WithX(screen.Bounds.X); }
            if (rect.Bottom > screen.Bounds.Bottom) { rect = rect.WithY(screen.Bounds.Bottom - rect.Height); }
            if (rect.Y < screen.Bounds.Y) { rect = rect.WithY(screen.Bounds.Y); }
            if (window.Position != rect.Position) { window.Position = rect.Position; }
        };

        var statusTextBlock = new TextBlock() {
            Margin = new Thickness(0, 0, 0, 11),
            TextWrapping = TextWrapping.Wrap,
        };

        var mainStack = new StackPanel() { Margin = new Thickness(11) };
        mainStack.Children.Add(statusTextBlock);

        var buttonPanelLeft = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 11, 0) };
        var downloadButton = new Button() {
            Content = "Download",
            Margin = new Thickness(0, 0, 7, 0),
            IsVisible = false,
        };
        downloadButton.Click += DownloadClick;
        buttonPanelLeft.Children.Add(downloadButton);
        var upgradeButton = new Button() {
            Content = "Upgrade",
            Margin = new Thickness(0, 0, 7, 0),
            IsVisible = false,
        };
        upgradeButton.Click += UpgradeClick;
        buttonPanelLeft.Children.Add(upgradeButton);
        var closeButton = new Button() {
            Content = "Close",
            IsDefault = true,
            IsCancel = true,
            HorizontalAlignment = HorizontalAlignment.Right,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(21, 0, 0, 0),
        };
        closeButton.Click += CloseClick;
        var buttonDockPanel = new DockPanel() { Margin = new Thickness(11) };
        buttonDockPanel.Children.Add(buttonPanelLeft);
        buttonDockPanel.Children.Add(closeButton);

        var windowStack = new StackPanel();
        windowStack.Children.Add(new TextBlock {
            Background = GetBrush("SystemBaseHighColor", Brushes.DarkGray, Brushes.LightGray),
            Foreground = GetBrush("SystemAltHighColor", Brushes.White, Brushes.Black),
            FontSize = window.FontSize * 1.25,
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 0, 0, 0),
            Padding = new Thickness(11),
            Text = window.Title,
        });
        windowStack.Children.Add(new Border() { Child = mainStack });
        windowStack.Children.Add(buttonDockPanel);

        var windowBorder = new Border {
            BorderThickness = new Thickness(1),
            BorderBrush = GetBrush("SystemChromeHighColor", Brushes.Black, Brushes.White),
            Child = windowStack
        };

        var bag = new Bag(serviceUri!,  // send not visible when null
                          window,
                          statusTextBlock,
                          downloadButton,
                          upgradeButton);
        closeButton.Tag = bag;
        downloadButton.Tag = bag;
        upgradeButton.Tag = bag;

        ThreadPool.QueueUserWorkItem(delegate {
            CheckForUpgrade(bag, assembly);
        });

        window.Content = windowBorder;
        if (owner != null) {
            using var source = new CancellationTokenSource();
            window.ShowDialog(owner)  // trickery to await for dialog from non-async method
                .ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
        } else {
            window.Show();
        }
    }

    private static void CheckForUpgrade(Bag bag, Assembly assembly) {
        bag.SetStatusText("Checking for upgrade…");

        try {
            var file = Upgrade.GetUpgradeFile(bag.ServiceUri, assembly);
            if (file != null) {
                bag.SetStatusText("Upgrade is available.");
                bag.EnableDownload(file);
            } else {
                bag.SetStatusText("Upgrade not available.");
            }
        } catch (Exception ex) {
            bag.SetStatusText("Error checking for upgrade!\n" + ex.Message);
        }
    }

    private static void DownloadClick(object? sender, RoutedEventArgs e) {
        var button = (Button)sender!;
        button.IsEnabled = false;

        var bag = (Bag)button.Tag!;

        ThreadPool.QueueUserWorkItem(delegate {
            bag.SetStatusText("Downloading…");
            bag.UpgradeFile!.ProgressChanged += (s, e) => {
                bag.SetStatusText("Downloading (" + e.ProgressPercentage.ToString(CultureInfo.InvariantCulture) + "%)…");
            };

            var downloadedFile = bag.UpgradeFile!.DownloadFile();
            if (downloadedFile != null) {
                bag.EnableUpgrade(downloadedFile);
            } else {
                bag.SetStatusText("Error downloading upgrade file!");
            }
        });
    }

    private static void UpgradeClick(object? sender, RoutedEventArgs e) {
        var button = (Button)sender!;
        button.IsEnabled = false;

        var bag = (Bag)button.Tag!;
        bag.SetStatusText("Upgrading…");

        try {
            Process.Start(bag.DownloadedFile!.FullName);
            Environment.Exit(113);
        } catch (Exception ex) {
            bag.SetStatusText("Error during upgrade!\n" + ex.Message);
        }
    }

    private static void CloseClick(object? sender, RoutedEventArgs e) {
        var bag = (Bag)((Button)sender!).Tag!;
        bag.CancelSource.Cancel();
        bag.Window.Close();
    }


    private record Bag {
        public Bag(Uri serviceUri, Window window, TextBlock statusBlock, Button downloadButton, Button upgradeButton) {
            ServiceUri = serviceUri;
            Window = window;
            StatusBlock = statusBlock;
            DownloadButton = downloadButton;
            UpgradeButton = upgradeButton;
            CancelSource = new CancellationTokenSource();
        }

        private readonly TextBlock StatusBlock;
        private readonly Button DownloadButton;
        private readonly Button UpgradeButton;

        public Uri ServiceUri { get; }
        public Window Window { get; }
        public CancellationTokenSource CancelSource { get; }

        public UpgradeFile? UpgradeFile;
        public FileInfo? DownloadedFile;

        public void SetStatusText(string text) {
            Dispatcher.UIThread.Invoke(delegate {
                StatusBlock.Text = text;
            });
        }

        public void EnableDownload(UpgradeFile upgradeFile) {
            UpgradeFile = upgradeFile;
            Dispatcher.UIThread.Invoke(delegate {
                UpgradeButton.IsVisible = false;
                DownloadButton.IsVisible = true;
            });
        }

        public void EnableUpgrade(FileInfo downloadedFile) {
            DownloadedFile = downloadedFile;
            Dispatcher.UIThread.Invoke(delegate {
                DownloadButton.IsVisible = false;
                UpgradeButton.IsVisible = true;
            });
        }
    }

    private static ISolidColorBrush GetBrush(string name, ISolidColorBrush lightDefault, ISolidColorBrush darkDefault) {
        var variant = Application.Current?.ActualThemeVariant ?? ThemeVariant.Light;
        if (Application.Current?.Styles[0] is IResourceProvider provider && provider.TryGetResource(name, variant, out var resource)) {
            if (resource is Color color) {
                return new SolidColorBrush(color);
            }
        }
        Debug.WriteLine("[FeedbackBox] Cannot find brush " + name);
        return (variant == ThemeVariant.Light) ? lightDefault : darkDefault;
    }
}
