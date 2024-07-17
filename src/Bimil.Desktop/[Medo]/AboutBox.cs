/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2024-07-16: Waiting for dialog close
//            Move window outside of bounds
//2024-07-07: Adjusted border color
//2023-12-20: Cleaned up a bit
//2023-03-12: Initial version

namespace Medo.Avalonia;

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Interactivity;
using global::Avalonia.Layout;
using global::Avalonia.Media;
using global::Avalonia.Media.Imaging;
using global::Avalonia.Styling;
using global::Avalonia.Threading;

/// <summary>
/// Simple about dialog.
/// </summary>
public static class AboutBox {

    /// <summary>
    /// Opens a window and returns only when the newly opened window is closed.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    public static void ShowDialog(Window owner) {
        ShowDialog(owner, webpage: null);
    }

    /// <summary>
    /// Opens a window and returns only when the newly opened window is closed.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="webpage">URI of program's web page.</param>
    public static void ShowDialog(Window owner, Uri? webpage) {
        var window = new Window() { MinWidth = 300 };
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
        window.Title = "About";

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

        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        Image? iconImage = null;
        if (owner?.Icon != null) {
            iconImage = new Image() {
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 7, 0),
            };
            var ms = new MemoryStream();
            owner.Icon.Save(ms);
            ms.Position = 0;
            var bitmap = new Bitmap(ms);
            iconImage.Source = bitmap.CreateScaledBitmap(new PixelSize(32, 32));
        }

        var productText = GetAppProduct(assembly);
        var productTextBlock = new TextBlock() {
            Text = productText,
            FontSize = window.FontSize * 2,
            FontStyle = FontStyle.Oblique,
        };

        var productVersionTextBlock = new TextBlock() {
            Text = GetAppVersion(assembly, minorMajorOnly: true),
            FontSize = window.FontSize * 1.5,
            Margin = new Thickness(0, 7, 0, 0),
        };

        var titleStack = new StackPanel() { Orientation = Orientation.Horizontal };
        if (iconImage != null) { titleStack.Children.Add(iconImage); }
        titleStack.Children.Add(productTextBlock);
        titleStack.Children.Add(new TextBlock() { Text = " " });
        titleStack.Children.Add(productVersionTextBlock);

        var titleAndVersionTextBlock = new TextBlock() {
            Text = GetAppTitle(assembly) + " " + GetAppVersion(assembly),
            Margin = new Thickness(0, 7, 0, 0),
        };

        var dotNetFrameworkTextBlock = new TextBlock() {
            Text = $".NET Framework {Environment.Version}",
            Margin = new Thickness(0),
        };

        var osVersionTextBlock = new TextBlock() {
            Text = $"{Environment.OSVersion}",
            Margin = new Thickness(0),
        };

        TextBlock? osPrettyNameBlock = null;
        var osPrettyName = GetOSPrettyName();
        if (osPrettyName != null) {
            osPrettyNameBlock = new TextBlock() {
                Text = osPrettyName,
                Margin = new Thickness(0),
            };
        }

        var mainStack = new StackPanel() { Margin = new Thickness(11) };
        mainStack.Children.Add(titleStack);
        mainStack.Children.Add(titleAndVersionTextBlock);
        mainStack.Children.Add(dotNetFrameworkTextBlock);
        mainStack.Children.Add(osVersionTextBlock);
        if (osPrettyNameBlock != null) { mainStack.Children.Add(osPrettyNameBlock); }

        var copyrightText = GetAppCopyright(assembly);
        if (!string.IsNullOrEmpty(copyrightText)) {
            var copyright = new TextBlock() { Text = copyrightText, Margin = new Thickness(0, 7, 0, 0) };
            mainStack.Children.Add(copyright);
        }

        var buttonPanelLeft = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 11, 0) };

        var readmePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "readme.txt");
        if (File.Exists(readmePath)) {
            var readmeButton = new Button() {
                Content = "Read me",
                Margin = new Thickness(0, 0, 7, 0),
                Tag = readmePath,
            };
            readmeButton.Click += ReadmeClick;
            buttonPanelLeft.Children.Add(readmeButton);
        }

        if (webpage != null) {
            var webpageButton = new Button() {
                Content = "Web page",
                Tag = webpage,
                Margin = new Thickness(0, 0, 7, 0),
            };
            webpageButton.Click += WebpageClick;
            buttonPanelLeft.Children.Add(webpageButton);
        }

        var closeButton = new Button() {
            Content = "Close",
            IsDefault = true,
            IsCancel = true,
            HorizontalAlignment = HorizontalAlignment.Right,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(21, 0, 0, 0),
            Tag = window,
        };
        closeButton.Click += CloseClick;

        var buttonDockPanel = new DockPanel() { Margin = new Thickness(11) };
        buttonDockPanel.Children.Add(buttonPanelLeft);
        buttonDockPanel.Children.Add(closeButton);

        var windowStack = new StackPanel();
        windowStack.Children.Add(new Border() { Child = mainStack });
        windowStack.Children.Add(buttonDockPanel);

        var windowBorder = new Border {
            BorderThickness = new Thickness(1),
            BorderBrush = GetBrush("SystemChromeHighColor", Brushes.Black, Brushes.White),
            Child = windowStack
        };

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


    private static void WebpageClick(object? sender, RoutedEventArgs e) {
        if (sender is Control control && control.Tag is Uri webpageUrl) {
            try {
                var url = webpageUrl.ToString();
                Process.Start(new ProcessStartInfo {
                    FileName = url,
                    UseShellExecute = true
                });
            } catch (System.ComponentModel.Win32Exception) { }
        }
    }

    private static void ReadmeClick(object? sender, RoutedEventArgs e) {
        if (sender is Control control && control.Tag is string readmePath) {
            try {
                Process.Start(new ProcessStartInfo {
                    FileName = readmePath,
                    UseShellExecute = true
                });
            } catch (System.ComponentModel.Win32Exception) { }
        }
    }

    private static void CloseClick(object? sender, RoutedEventArgs e) {
        if (sender is Control control && control.Tag is Window window) {
            window.Close();
        }
    }


    private static string GetAppProduct(Assembly assembly) {
        var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
        if ((productAttributes != null) && (productAttributes.Length >= 1)) {
            return ((AssemblyProductAttribute)productAttributes[^1]).Product;
        } else {
            return GetAppTitle(assembly);
        }
    }

    private static string GetAppTitle(Assembly assembly) {
        var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
        if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
            return ((AssemblyTitleAttribute)titleAttributes[^1]).Title;
        } else {
            return assembly.GetName().Name ?? "";
        }
    }

    private static string GetAppVersion(Assembly assembly, bool minorMajorOnly = false) {
        var version = assembly.GetName().Version;
        if (version != null) {
            if (minorMajorOnly) {
                return $"{version.Major}.{version.Minor}";
            } else {
                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
        }
        return "";
    }

    private static string? GetAppCopyright(Assembly assembly) {
        var copyrightAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
        if ((copyrightAttributes != null) && (copyrightAttributes.Length >= 1)) {
            return ((AssemblyCopyrightAttribute)copyrightAttributes[^1]).Copyright;
        }
        return null;
    }

    private static readonly string[] NewLineArray = ["\n\r", "\n", "\r"];

    private static string? GetOSPrettyName() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            try {
                var osReleaseLines = File.ReadAllText("/etc/os-release").Split(NewLineArray, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in osReleaseLines) {
                    if (line.StartsWith("PRETTY_NAME=", StringComparison.OrdinalIgnoreCase)) {
                        var text = line[12..].Trim();
                        if (text.StartsWith('"') && text.EndsWith('"')) {
                            return text[1..^1];
                        }
                    }
                }
            } catch (SystemException) { }
        }
        return null;
    }


    private static ISolidColorBrush GetBrush(string name, ISolidColorBrush lightDefault, ISolidColorBrush darkDefault) {
        var variant = Application.Current?.ActualThemeVariant ?? ThemeVariant.Light;
        if (Application.Current?.Styles[0] is IResourceProvider provider && provider.TryGetResource(name, variant, out var resource)) {
            if (resource is Color color) {
                return new SolidColorBrush(color);
            }
        }
        Debug.WriteLine("[AboutBox] Cannot find brush " + name);
        return (variant == ThemeVariant.Light) ? lightDefault : darkDefault;
    }
}
