namespace Bimil.Desktop;
using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;

internal class ThemeToolbarImageResources {

#pragma warning disable CA1822 // Mark members as static

    public Bitmap? FileNew => GetAssetBitmap("FileNew");
    public Bitmap? FileOpen => GetAssetBitmap("FileOpen");
    public Bitmap? FileSave => GetAssetBitmap("FileSave");
    public Bitmap? FileEdit => GetAssetBitmap("FileEdit");

    public Bitmap? ItemAdd => GetAssetBitmap("ItemAdd");
    public Bitmap? ItemEdit => GetAssetBitmap("ItemEdit");
    public Bitmap? ItemView => GetAssetBitmap("ItemView");
    public Bitmap? ItemRemove => GetAssetBitmap("ItemRemove");

    public Bitmap? Find => GetAssetBitmap("Find");
    public Bitmap? PasswordGenerate => GetAssetBitmap("PasswordGenerate");

    public Bitmap? App => GetAssetBitmap("App");

#pragma warning restore CA1822 // Mark members as static


    internal static void Setup(MainWindow mainWindow) {
        var menuPanel = mainWindow.FindControl<DockPanel>("Menu") ?? throw new InvalidOperationException("Cannot find menu.");
        menuPanel.DataContext = new ThemeToolbarImageResources();
        var scale = mainWindow?.Screens?.ScreenFromWindow(mainWindow)?.Scaling ?? 1;
        AssetSize = (int)(scale * 24) switch {
            >= 64 => 64,
            >= 48 => 48,
            >= 32 => 32,
            >= 24 => 24,
            _ => 24
        };
        Debug.WriteLine($"Assets are {AssetSize}x{AssetSize} pixels");

        var app = AppAvalonia.Current;
        if (app != null) {
            app.ActualThemeVariantChanged += (object? sender, EventArgs e) => {
                menuPanel.DataContext = new ThemeToolbarImageResources();  // force refresh
            };
        }
    }


    private static int AssetSize = 24;

    private static Bitmap GetAssetBitmap(string baseName) {
        return new Bitmap(AssetLoader.Open(GetAssetUri(baseName, AssetSize)));
    }

    private static Uri GetAssetUri(string baseName, int size) {
        var suffix = ((AppAvalonia.Current?.ActualThemeVariant ?? ThemeVariant.Light) == ThemeVariant.Light) ? "L" : "D";
        return new Uri("avares://Bimil/Assets/Toolbar/" + baseName + "_" + size.ToString(CultureInfo.InvariantCulture) + suffix + ".png");
    }
}
