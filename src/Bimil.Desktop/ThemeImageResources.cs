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

    private ThemeImageResources(bool isDarkThemeVariant, bool isEnabled) {
        SecurityLevelLowX2 = GetAssetBitmapX2("SecurityLow", isDarkThemeVariant);
        SecurityLevelMediumX2 = GetAssetBitmapX2("SecurityMedium", isDarkThemeVariant);
        SecurityLevelHighX2 = GetAssetBitmapX2("SecurityHigh", isDarkThemeVariant);
    }

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
            >= 24.0 => 24,
            _ => 16
        };
        Debug.WriteLine($"Assets are {AssetSize}x{AssetSize} pixels");

        IsDarkThemeVariant = !((AppAvalonia.Current?.ActualThemeVariant ?? ThemeVariant.Light) == ThemeVariant.Light);
        Enabled = new ThemeImageResources(IsDarkThemeVariant, isEnabled: true);
        Disabled = new ThemeImageResources(IsDarkThemeVariant, isEnabled: false);
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

    public static void SetImage(Button button, string bitmapName) {
        Image? image = null;
        if (button.Content is Image childT) {
            image = childT;
        } else if (button.Content is Panel panel) {
            foreach (var childControl in panel.Children) {
                if (childControl is Image childControlT) {
                    image = childControlT;
                }
            }
        }
        if (image != null) {
            var bitmap = GetAssetBitmap(bitmapName, IsDarkThemeVariant, !button.IsEnabled);
            image.Source = bitmap;
            image.Height = bitmap.Size.Height;
            button.Height = double.NaN;
        }
    }

    public static void Update() {
        Updated?.Invoke(null, EventArgs.Empty);
    }


    private static bool IsDarkThemeVariant = false;
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
