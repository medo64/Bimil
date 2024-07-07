namespace Bimil.Desktop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;

internal class App {

    private class ConsoleTraceListener : TraceListener {
        public override void Write(string? message) {
            Console.Write(message);
        }

        public override void WriteLine(string? message) {
            Console.WriteLine(message);
        }
    }

    [STAThread]
    public static void Main(string[] args) {
#if DEBUG
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { Trace.Listeners.Add(new ConsoleTraceListener()); }
#endif

        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<AppAvalonia>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

}
