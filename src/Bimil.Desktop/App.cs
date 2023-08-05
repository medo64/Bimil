using System;
using Avalonia;

namespace Bimil.Desktop;

internal class App {

    [STAThread]
    public static void Main(string[] args)
        => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<AppAvalonia>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

}
