namespace Bimil;

using System.Diagnostics;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Medo;

internal static class Helper {

    public static async void SetClipboardText(string? text, IClipboard? avaloniaClipboard) {
        text ??= "";

        if (PTClipboard.Main.IsAvailable) {
            if (Settings.Current.SyncPrimaryClipboard) {  // set both primary and clipboard
                PTClipboard.SetText(text);
            } else {
                PTClipboard.Main.SetText(text);
            }
        } else if (avaloniaClipboard != null) {  // Avalonia fallback
            Debug.WriteLine("Clipboard falback");
            var data = new DataTransfer();
            data.Add(DataTransferItem.CreateText(text));
            await avaloniaClipboard.SetDataAsync(data);
        } else {
            Debug.WriteLine("Clipboard not available");
        }
    }

}
