namespace Bimil.Desktop;

using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;

internal class ThemeImageResources {

    private ThemeImageResources(bool isDarkThemeVariant, bool isEnabled) {
        FileNew = GetAssetBitmap("FileNew", isDarkThemeVariant, grayscale: !isEnabled);
        FileOpen = GetAssetBitmap("FileOpen", isDarkThemeVariant, grayscale: !isEnabled);
        FileSave = GetAssetBitmap("FileSave", isDarkThemeVariant, grayscale: !isEnabled);
        FileProperties = GetAssetBitmap("FileEdit", isDarkThemeVariant, grayscale: !isEnabled);
        ItemAdd = GetAssetBitmap("ItemAdd", isDarkThemeVariant, grayscale: !isEnabled);
        ItemEdit = GetAssetBitmap("ItemEdit", isDarkThemeVariant, grayscale: !isEnabled);
        ItemView = GetAssetBitmap("ItemView", isDarkThemeVariant, grayscale: !isEnabled);
        ItemRemove = GetAssetBitmap("ItemRemove", isDarkThemeVariant, grayscale: !isEnabled);
        Find = GetAssetBitmap("Find", isDarkThemeVariant, grayscale: !isEnabled);
        PasswordGenerator = GetAssetBitmap("PasswordGenerate", isDarkThemeVariant, grayscale: !isEnabled);
        App = GetAssetBitmap("App", isDarkThemeVariant, grayscale: !isEnabled);
        AppHasUpgrade = GetAssetBitmap("AppHasUpgrade", isDarkThemeVariant, grayscale: !isEnabled);

        SecurityLevelLowX2 = GetAssetBitmapX2("SecurityLow", isDarkThemeVariant);
        SecurityLevelMediumX2 = GetAssetBitmapX2("SecurityMedium", isDarkThemeVariant);
        SecurityLevelHighX2 = GetAssetBitmapX2("SecurityHigh", isDarkThemeVariant);
    }

    #region Toolbar

    public Bitmap FileNew { get; private init; }
    public Bitmap FileOpen { get; private init; }
    public Bitmap FileSave { get; private init; }
    public Bitmap FileProperties { get; private init; }

    public Bitmap ItemAdd { get; private init; }
    public Bitmap ItemEdit { get; private init; }
    public Bitmap ItemView { get; private init; }
    public Bitmap ItemRemove { get; private init; }

    public Bitmap Find { get; private init; }
    public Bitmap PasswordGenerator { get; private init; }

    public Bitmap App { get; private init; }
    public Bitmap AppHasUpgrade { get; private init; }

    #endregion Toolbar

    #region SecurityLevel

    public Bitmap SecurityLevelLowX2 { get; private init; }
    public Bitmap SecurityLevelMediumX2 { get; private init; }
    public Bitmap SecurityLevelHighX2 { get; private init; }

    #endregion SecurityLevel


    public static ThemeImageResources? Enabled { get; private set; }
    public static ThemeImageResources? Disabled { get; private set; }
    public static event EventHandler<EventArgs>? Updated;

    internal static void Setup(MainWindow mainWindow) {
        var scale = mainWindow?.Screens?.ScreenFromWindow(mainWindow)?.Scaling ?? 1;
        AssetSize = (scale * 24) switch {
            >= 64.0 => 64,
            >= 48.0 => 48,
            >= 32.0 => 32,
            _ => 24
        };
        Debug.WriteLine($"Assets are {AssetSize}x{AssetSize} pixels");

        var isDarkThemeVariant = !((AppAvalonia.Current?.ActualThemeVariant ?? ThemeVariant.Light) == ThemeVariant.Light);
        Enabled = new ThemeImageResources(isDarkThemeVariant, isEnabled: true);
        Disabled = new ThemeImageResources(isDarkThemeVariant, isEnabled: false);
        Updated?.Invoke(null, EventArgs.Empty);

        var app = AppAvalonia.Current;
        if (app != null) {
            app.ActualThemeVariantChanged += (object? sender, EventArgs e) => {
                var isDarkThemeVariant = !((AppAvalonia.Current?.ActualThemeVariant ?? ThemeVariant.Light) == ThemeVariant.Light);
                Enabled = new ThemeImageResources(isDarkThemeVariant, isEnabled: true);
                Disabled = new ThemeImageResources(isDarkThemeVariant, isEnabled: false);
                Updated?.Invoke(null, EventArgs.Empty);
            };
        }
    }

    public static void Update() {
        Updated?.Invoke(null, EventArgs.Empty);
    }


    private static int AssetSize = 24;

    private static Bitmap GetAssetBitmap(string baseName, bool isDarkThemeVariant, bool grayscale = false) {
        var bitmap = new Bitmap(AssetLoader.Open(GetAssetUri(baseName, isDarkThemeVariant, AssetSize)));

        if (grayscale) {
            var width = bitmap.PixelSize.Width;
            var height = bitmap.PixelSize.Height;

            var buffer = new byte[width * height * 4];
            var bufferPtr = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try {
                var stride = 4 * width;
                bitmap.CopyPixels(default, bufferPtr.AddrOfPinnedObject(), buffer.Length, stride);

                for (var i = 0; i < buffer.Length; i += 4) {
                    var b = buffer[i + 0];
                    var g = buffer[i + 1];
                    var r = buffer[i + 2];

                    var offset = isDarkThemeVariant ? -64 : +64;
                    var grey = byte.CreateSaturating(0.299 * r + 0.587 * g + 0.114 * b + offset);

                    buffer[i + 0] = grey;
                    buffer[i + 1] = grey;
                    buffer[i + 2] = grey;
                }

                var writableBitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888);
                using (var stream = writableBitmap.Lock()) {
                    Marshal.Copy(buffer, 0, stream.Address, buffer.Length);
                }

                bitmap = writableBitmap;
            } finally {
                bufferPtr.Free();
            }
        }

        return bitmap;
    }

    private static Bitmap GetAssetBitmapX2(string baseName, bool isDarkThemeVariant) {  // twice the size
        var assetSize = (AssetSize * 2) switch {
            >= 64 => 64,
            >= 48 => 48,
            >= 32 => 32,
            _ => 24
        };
        return new Bitmap(AssetLoader.Open(GetAssetUri(baseName, isDarkThemeVariant, assetSize)));
    }

    private static Uri GetAssetUri(string baseName, bool isDarkThemeVariant, int size) {
        var suffix = isDarkThemeVariant ? "D" : "L";
        return new Uri("avares://Bimil/Assets/Images/" + baseName + "_" + size.ToString(CultureInfo.InvariantCulture) + suffix + ".png");
    }
}
