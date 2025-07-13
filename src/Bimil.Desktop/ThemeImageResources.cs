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

    public static ThemeImageResources? Enabled { get; private set; }
    public static ThemeImageResources? Disabled { get; private set; }
    public static event EventHandler<EventArgs>? Updated;

    internal static void Setup(MainWindow mainWindow) {
        var scale = mainWindow?.Screens?.ScreenFromWindow(mainWindow)?.Scaling ?? 1;
        AssetSize = (scale * 16) switch {
            >= 56.0 => 64,
            >= 40.0 => 48,
            >= 28.0 => 32,
            >= 20.0 => 24,
            _ => 16
        };
        Debug.WriteLine($"Assets are {AssetSize}x{AssetSize} pixels");

        IsDarkThemeVariant = !((AppAvalonia.Current?.ActualThemeVariant ?? ThemeVariant.Light) == ThemeVariant.Light);
        Updated?.Invoke(null, EventArgs.Empty);

        var app = AppAvalonia.Current;
        if (app != null) {
            app.ActualThemeVariantChanged += (object? sender, EventArgs e) => {
                var isDarkThemeVariant = !((AppAvalonia.Current?.ActualThemeVariant ?? ThemeVariant.Light) == ThemeVariant.Light);
                Updated?.Invoke(null, EventArgs.Empty);
            };
        }
    }

    public static void SetImage(Button button, string bitmapName, bool doubleScale = false) {
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
            var bitmap = GetAssetBitmap(bitmapName, IsDarkThemeVariant, grayscale: !button.IsEnabled, doubleScale);
            image.Source = bitmap;
            var scaledSize = AssetSize * (doubleScale ? 2 : 1);
            image.Height = scaledSize;
            image.Width = scaledSize;
            button.Height = double.NaN;
        }
    }

    public static void SetImage(Image image, string bitmapName, bool doubleScale = false) {
        var bitmap = GetAssetBitmap(bitmapName, IsDarkThemeVariant, doubleScale: doubleScale);
        image.Source = bitmap;
        var scaledSize = AssetSize * (doubleScale ? 2 : 1);
        image.Height = scaledSize;
        image.Width= scaledSize;
    }


    public static void Update() {
        Updated?.Invoke(null, EventArgs.Empty);
    }


    private static bool IsDarkThemeVariant = false;
    private static int AssetSize = 24;


    private static Bitmap GetAssetBitmap(string baseName, bool isDarkThemeVariant, bool grayscale = false, bool doubleScale = false) {
        var bitmap = new Bitmap(AssetLoader.Open(GetAssetUri(baseName, isDarkThemeVariant, Math.Min(AssetSize * (doubleScale ? 2 : 1), 64))));

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

    private static Uri GetAssetUri(string baseName, bool isDarkThemeVariant, int size) {
        var suffix = isDarkThemeVariant ? "D" : "L";
        return new Uri("avares://Bimil/Assets/Images/" + baseName + "_" + size.ToString(CultureInfo.InvariantCulture) + suffix + ".png");
    }
}
