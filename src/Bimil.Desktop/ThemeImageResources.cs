namespace Bimil.Desktop;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia;
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

    private static Bitmap GetAssetBitmap(string baseName, bool grayscale = false) {
        var bitmap = new Bitmap(AssetLoader.Open(GetAssetUri(baseName, AssetSize)));

        if (grayscale) {
            var width = bitmap.PixelSize.Width;
            var height = bitmap.PixelSize.Height;

            var buffer = new byte[width * height * 4];
            var bufferPtr = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            var stride = 4 * width;
            bitmap.CopyPixels(default, bufferPtr.AddrOfPinnedObject(), buffer.Length, stride);

            for (var i = 0; i < buffer.Length; i += 4) {
                var b = buffer[i];
                var g = buffer[i + 1];
                var r = buffer[i + 2];

                var grey = byte.CreateSaturating(0.299 * r + 0.587 * g + 0.114 * b);

                buffer[i] = grey;
                buffer[i + 1] = grey;
                buffer[i + 2] = grey;
            }

            // Write the modified pixel data back to the WriteableBitmap
            var writableBitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888);
            using (var stream = writableBitmap.Lock()) {
                Marshal.Copy(buffer, 0, stream.Address, buffer.Length);
            }
            bitmap = writableBitmap;
        }

        return bitmap;
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
