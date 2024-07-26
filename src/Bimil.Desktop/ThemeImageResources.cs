namespace Bimil.Desktop;

using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;

internal class ThemeImageResources {

#pragma warning disable CA1822 // Mark members as static

    #region Toolbar

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

    #endregion Toolbar

    #region SecurityLevel

    public Bitmap? SecurityLevelLowX2 => GetAssetBitmapX2("SecurityLow");
    public Bitmap? SecurityLevelMediumX2 => GetAssetBitmapX2("SecurityMedium");
    public Bitmap? SecurityLevelHighX2 => GetAssetBitmapX2("SecurityHigh");

    #endregion SecurityLevel

#pragma warning restore CA1822 // Mark members as static


    internal static void Setup(MainWindow mainWindow, DockPanel menu) {
        menu.DataContext = new ThemeImageResources();
        var scale = mainWindow?.Screens?.ScreenFromWindow(mainWindow)?.Scaling ?? 1;
        AssetSize = (scale * 24) switch {
            >= 64.0 => 64,
            >= 48.0 => 48,
            >= 32.0 => 32,
            _ => 24
        };
        Debug.WriteLine($"Assets are {AssetSize}x{AssetSize} pixels");

        var app = AppAvalonia.Current;
        if (app != null) {
            app.ActualThemeVariantChanged += (object? sender, EventArgs e) => {
                menu.DataContext = new ThemeImageResources();  // force refresh
            };
        }
    }

    private static readonly Lazy<ThemeImageResources> _default = new(() => new ThemeImageResources());
    internal static ThemeImageResources Default {
        get {
            if (AssetSize == 0) { throw new InvalidOperationException(); }  // only works after SetupMenu is already called (so that AssetSize is set)
            return _default.Value;
        }
    }


    private static int AssetSize = 24;

    private static Bitmap GetAssetBitmap(string baseName) {
        return new Bitmap(AssetLoader.Open(GetAssetUri(baseName, AssetSize)));
    }

    private static Bitmap GetAssetBitmapX2(string baseName) {  // twice the size
        var assetSize = (AssetSize * 2) switch {
            >= 64 => 64,
            >= 48 => 48,
            >= 32 => 32,
            _ => 24
        };
        return new Bitmap(AssetLoader.Open(GetAssetUri(baseName, assetSize)));
    }

    private static Uri GetAssetUri(string baseName, int size) {
        var suffix = ((AppAvalonia.Current?.ActualThemeVariant ?? ThemeVariant.Light) == ThemeVariant.Light) ? "L" : "D";
        return new Uri("avares://Bimil/Assets/Images/" + baseName + "_" + size.ToString(CultureInfo.InvariantCulture) + suffix + ".png");
    }
}
