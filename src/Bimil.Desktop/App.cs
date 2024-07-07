namespace Bimil.Desktop;
using System;
using Avalonia;

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
