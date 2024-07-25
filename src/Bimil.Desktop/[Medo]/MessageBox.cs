/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2024-07-24: Added ShowQuestionDialog
//2024-07-22: Initial version

namespace Medo.Avalonia;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Layout;
using global::Avalonia.Media;
using global::Avalonia.Media.Imaging;
using global::Avalonia.Styling;
using global::Avalonia.Threading;

/// <summary>
/// Simple message dialog.
/// </summary>
public static class MessageBox {

    /// <summary>
    /// Opens a window formed with provided title, message, and buttons.
    /// The first button is always a default and the last button is always a cancel.
    /// If no buttons are given, only a OK button is shown.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="title">Title of the window.</param>
    /// <param name="message">Message to show.</param>
    /// <param name="buttonsText">Text for buttons.</param>
    public static int ShowInfoDialog(Window owner, string title, string message, params string[] buttonsText) {
        return ShowDialog(owner, GetBitmapFromBase64(Base64Info64L, Base64Info64D), title, message, buttonsText);
    }

    /// <summary>
    /// Opens a window formed with provided title, message, and buttons.
    /// The first button is always a default and the last button is always a cancel.
    /// If no buttons are given, only a OK button is shown.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="title">Title of the window.</param>
    /// <param name="message">Message to show.</param>
    /// <param name="buttonsText">Text for buttons.</param>
    public static int ShowWarningDialog(Window owner, string title, string message, params string[] buttonsText) {
        return ShowDialog(owner, GetBitmapFromBase64(Base64Warning64L, Base64Warning64D), title, message, buttonsText);
    }

    /// <summary>
    /// Opens a window formed with provided title, message, and buttons.
    /// The first button is always a default and the last button is always a cancel.
    /// If no buttons are given, only a OK button is shown.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="title">Title of the window.</param>
    /// <param name="message">Message to show.</param>
    /// <param name="buttonsText">Text for buttons.</param>
    public static int ShowErrorDialog(Window owner, string title, string message, params string[] buttonsText) {
        return ShowDialog(owner, GetBitmapFromBase64(Base64Error64L, Base64Error64D), title, message, buttonsText);
    }

    /// <summary>
    /// Opens a window formed with provided title, message, and buttons.
    /// The first button is always a default and the last button is always a cancel.
    /// If no buttons are given, only a OK button is shown.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="title">Title of the window.</param>
    /// <param name="message">Message to show.</param>
    /// <param name="buttonsText">Text for buttons.</param>
    public static int ShowCriticalDialog(Window owner, string title, string message, params string[] buttonsText) {
        return ShowDialog(owner, GetBitmapFromBase64(Base64Critical64L, Base64Critical64D), title, message, buttonsText);
    }

    /// <summary>
    /// Opens a window formed with provided title, message, and buttons.
    /// The first button is always a default and the last button is always a cancel.
    /// If no buttons are given, only a OK button is shown.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="title">Title of the window.</param>
    /// <param name="message">Message to show.</param>
    /// <param name="buttonsText">Text for buttons.</param>
    public static int ShowQuestionDialog(Window owner, string title, string message, params string[] buttonsText) {
        return ShowDialog(owner, GetBitmapFromBase64(Base64Question64L, Base64Question64D), title, message, buttonsText);
    }

    /// <summary>
    /// Opens a window formed with provided title, message, and buttons.
    /// The first button is always a default and the last button is always a cancel.
    /// If no buttons are given, only a OK button is shown.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="title">Title of the window.</param>
    /// <param name="message">Message to show.</param>
    /// <param name="buttonsText">Text for buttons.</param>
    public static int ShowDialog(Window owner, string title, string message, params string[] buttonsText) {
        return ShowDialog(owner, null, title, message, buttonsText);
    }

    private static int ShowDialog(Window owner, Bitmap? iconBitmap, string title, string message, params string[] buttonsText) {
        if (string.IsNullOrEmpty(title)) { title = "Message"; }
        if ((buttonsText == null) || (buttonsText.Length == 0)) { buttonsText = ["Ok"]; }

        var window = new Window() { MinWidth = 400, MaxWidth = 600, MinHeight = 100 };
        if (owner != null) {
            window.Icon = owner.Icon;
            window.ShowInTaskbar = false;
            if (owner.Topmost) { window.Topmost = true; }
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        } else {  // just in case null is passed, not ideal
            window.ShowInTaskbar = true;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        window.Background = GetBrush("SystemChromeLowColor", Brushes.White, Brushes.Black);
        window.CanResize = false;
        window.ShowActivated = true;
        window.SizeToContent = SizeToContent.WidthAndHeight;
        window.SystemDecorations = SystemDecorations.BorderOnly;
        window.ExtendClientAreaToDecorationsHint = true;
        window.Title = title;

        window.Opened += (sender, e) => {  // adjust position as needed
            var screen = window.Screens.ScreenFromWindow(window);
            if ((screen == null) || (window.FrameSize == null)) { return; }

            var pos = window.Position;
            var size = PixelSize.FromSize(window.FrameSize.Value, screen.Scaling);
            var rect = new PixelRect(pos, size);
            Debug.WriteLine(rect);

            if (rect.Right > screen.Bounds.Right) { rect = rect.WithX(screen.Bounds.Right - rect.Width); }
            if (rect.X < screen.Bounds.X) { rect = rect.WithX(screen.Bounds.X); }
            if (rect.Bottom > screen.Bounds.Bottom) { rect = rect.WithY(screen.Bounds.Bottom - rect.Height); }
            if (rect.Y < screen.Bounds.Y) { rect = rect.WithY(screen.Bounds.Y); }
            if (window.Position != rect.Position) { window.Position = rect.Position; }
        };

        Image? iconImage = null;
        if (iconBitmap != null) {
            iconImage = new Image() {
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 7, 0),
                Source = iconBitmap,
            };
        }

        var messageTextBlock = new TextBlock() {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 7, 0, 0),
        };

        var mainGrid = new Grid() { Margin = new Thickness(7, 7, 7, 7) };
        mainGrid.RowDefinitions.Add(new RowDefinition());
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        if (iconImage != null) {
            mainGrid.Children.Add(iconImage);
            Grid.SetRow(iconImage, 0);
            Grid.SetColumn(iconImage, 0);
        }
        mainGrid.Children.Add(messageTextBlock);
        Grid.SetRow(messageTextBlock, 0);
        Grid.SetColumn(messageTextBlock, 1);

        var buttonStack = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(11) };

        int selectedButton = 0;
        var buttonList = new List<Button>();
        for (var i = 0; i < buttonsText.Length; i++) {
            var button = new Button() {
                Content = buttonsText[i],
                IsDefault = (i == 0),  // first is always default
                IsCancel = (i == buttonsText.Length - 1),  // last is always cancel
                HorizontalAlignment = HorizontalAlignment.Right,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(7, 0, 0, 0),
                Tag = i,
            };
            button.Click += (s, e) => { selectedButton = (int)((Button)s!).Tag!; window.Close(); };
            buttonStack.Children.Add(button);
            buttonList.Add(button);
        }

        var windowStack = new StackPanel();
        windowStack.Children.Add(new TextBlock {  // title
            Background = GetBrush("SystemBaseHighColor", Brushes.DarkGray, Brushes.LightGray),
            Foreground = GetBrush("SystemAltHighColor", Brushes.White, Brushes.Black),
            FontSize = window.FontSize * 1.25,
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 0, 0, 0),
            Padding = new Thickness(11),
            Text = window.Title,
        });
        windowStack.Children.Add(new Border() { Child = mainGrid });
        windowStack.Children.Add(buttonStack);

        var windowBorder = new Border {
            BorderThickness = new Thickness(1),
            BorderBrush = GetBrush("SystemChromeHighColor", Brushes.Black, Brushes.White),
            Child = windowStack
        };

        window.Content = windowBorder;
        window.Opened += (s, e) => { buttonList[0].Focus(NavigationMethod.Tab); };
        window.KeyDown += (s, e) => { if (e.Key == Key.Escape) { selectedButton = buttonList.Count - 1; window.Close(); } };
        if (owner != null) {
            using var source = new CancellationTokenSource();
            window.ShowDialog(owner)  // trickery to await for dialog from non-async method
                .ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
        } else {
            window.Show();
        }

        return selectedButton;
    }


    private static ISolidColorBrush GetBrush(string name, ISolidColorBrush lightDefault, ISolidColorBrush darkDefault) {
        var variant = Application.Current?.ActualThemeVariant ?? ThemeVariant.Light;
        if (Application.Current?.Styles[0] is IResourceProvider provider && provider.TryGetResource(name, variant, out var resource)) {
            if (resource is Color color) {
                return new SolidColorBrush(color);
            }
        }
        Debug.WriteLine("[MessageBox] Cannot find brush " + name);
        return (variant == ThemeVariant.Light) ? lightDefault : darkDefault;
    }

    private static Bitmap GetBitmapFromBase64(string base64Light, string base64Dark) {
        var variant = Application.Current?.ActualThemeVariant ?? ThemeVariant.Light;
        var base64 = (variant == ThemeVariant.Light) ? base64Light : base64Dark;
        return new Bitmap(new MemoryStream(Convert.FromBase64String(base64)));
    }

    private static readonly string Base64Info64L = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAilBMVEUtR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3Ibg0nUAAAALXRSTlMABgwPEhUYHiQnKjAzPEhOVFdaXWBydZCTlpmcn6Kut73GyczP0urt8PP2+fw0nSEjAAABmklEQVRYw+2X25aCMAxFVVBEEVQEEbzSUVTI///ezJI1CjS965vnNT2bRZOmaa/3lUBjzxtrWp1lSq7w0JWkS0fNPYxz6CiPh9J2Ny0BUZm6UnZrXQFD1doS+/0LcHTxRf6oAq6qiGvvb0GobZ/tH+UgoXzE3L4zSOnM2Mr+ESR1xP8iAWklmD9AFh58x/EPSCCg/XZBp2xRhxZ0agubAmzoz2T/sYyObahzixTQs+p8pKC6pzwFNQCknRIqeWswetkupwit+3kdnKPno30oCJ7v/cy2Z3s8Rlr9CzTU7HKhDiBsADIdQNYAnHQApwagwJfcyUN3PFq8/APWwa/DrDYxECZBAHilYaIHmLwPYPwLxpvISiMfUIgLiQ9oFtJWB9As5ZUOYCU+znyAI24oXMBPqyPF6oBY3FS5gE5TxVsKD5B1B6tKDVC5ElcbD7CRuVw5AORyxa53NiD4xIBhPuIYD1nmY575oGk+6r5h2P7byoQ97ieW0YMjcxWePFSHIQpPnrrLhTtyq703sgsVH11PTT1v+n38ivQLroV3tTFbkBQAAAAASUVORK5CYII=";
    private static readonly string Base64Info64D = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAilBMVEWQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+H1zcNdAAAALXRSTlMABgwPEhUYHiQnKjAzPEhOVFdaXWBydZCTlpmcn6Kut73GyczP0urt8PP2+fw0nSEjAAABmklEQVRYw+2X25aCMAxFVVBEEVQEEbzSUVTI///ezJI1CjS965vnNT2bRZOmaa/3lUBjzxtrWp1lSq7w0JWkS0fNPYxz6CiPh9J2Ny0BUZm6UnZrXQFD1doS+/0LcHTxRf6oAq6qiGvvb0GobZ/tH+UgoXzE3L4zSOnM2Mr+ESR1xP8iAWklmD9AFh58x/EPSCCg/XZBp2xRhxZ0agubAmzoz2T/sYyObahzixTQs+p8pKC6pzwFNQCknRIqeWswetkupwit+3kdnKPno30oCJ7v/cy2Z3s8Rlr9CzTU7HKhDiBsADIdQNYAnHQApwagwJfcyUN3PFq8/APWwa/DrDYxECZBAHilYaIHmLwPYPwLxpvISiMfUIgLiQ9oFtJWB9As5ZUOYCU+znyAI24oXMBPqyPF6oBY3FS5gE5TxVsKD5B1B6tKDVC5ElcbD7CRuVw5AORyxa53NiD4xIBhPuIYD1nmY575oGk+6r5h2P7byoQ97ieW0YMjcxWePFSHIQpPnrrLhTtyq703sgsVH11PTT1v+n38ivQLroV3tTFbkBQAAAAASUVORK5CYII=";
    private static readonly string Base64Warning64L = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAA5FBMVEVLSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1LSw1l11ILAAAAS3RSTlMAAwYJDA8SFRgbHiEkKi0wMzY5PD9FSEtOUVRaXWNmaWxvcnV4e36Bh4qNkJOWmZyfpauusbS3ur3Aw8bMz9LV297h5Ofq7fP2+fyYYDdJAAAB6klEQVQYGe3BXUPSYAAG0GdIQwpBIEVCIpKMEEhsJJHoYjQcz///P63thX29g23ddOE5eHGQWsA/UD4Y5KqLrHJ3dNwhox6FK2SimhSsErIYcGeCDEoWPXWk940+P5FanQEtpLVgwDKHdFoM6SGVvMGQtYo0eowYIYXimi5T00y6NidIbkzXwzFQeKBrisQqG7rqsFUpnCOpe7p+w7Gia6EgmQaFRzgeKXSQiPJEwYBDp2DkkUSHW5aCv5651UcCeZM7x7Cp3Hku4rA+PaewlemZ4KA3Fj1N2Br0OcUhE/p0YHtPnzkOqNOvD9tn+r3DfnP63cL2lX56Dvu0GDCD7Z4BV9gj94sBOmxPDDBVxOsxyFIArBk0RCx1zZDXQIEhVglxRgx7C9QYNkWMkw3D2sAlI+qQmzLiBrhhxEKBzDmjdEXRGXUJCWVBidmMEkYeUR2mcI2IvMEU1kWE9Smz0rQVZW4RUlwzymzC1jQZtakgaMIoqwpH1WLUDwRUKDGEMKREA35zSpxBOKOEnoOnSZkyhAplOtjJ6ZS5gHBBGVPF1kdKaRA0Sn2B8MqkXBuONuWsIlzXjDMoAIUB43yC6zvjLZeMp8E1Y0YzuEbMaARXjRnVIIyZyRhbRwNmMDiCp9wdp9Qt48V/4w+w+ukPK6SgHwAAAABJRU5ErkJggg==";
    private static readonly string Base64Warning64D = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAA5FBMVEWxs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3exs3cuV6ycAAAAS3RSTlMAAwYJDA8SFRgbHiEkKi0wMzY5PD9FSEtOUVRaXWNmaWxvcnV4e36Bh4qNkJOWmZyfpauusbS3ur3Aw8bMz9LV297h5Ofq7fP2+fyYYDdJAAAB6klEQVQYGe3BXUPSYAAG0GdIQwpBIEVCIpKMEEhsJJHoYjQcz///P63thX29g23ddOE5eHGQWsA/UD4Y5KqLrHJ3dNwhox6FK2SimhSsErIYcGeCDEoWPXWk940+P5FanQEtpLVgwDKHdFoM6SGVvMGQtYo0eowYIYXimi5T00y6NidIbkzXwzFQeKBrisQqG7rqsFUpnCOpe7p+w7Gia6EgmQaFRzgeKXSQiPJEwYBDp2DkkUSHW5aCv5651UcCeZM7x7Cp3Hku4rA+PaewlemZ4KA3Fj1N2Br0OcUhE/p0YHtPnzkOqNOvD9tn+r3DfnP63cL2lX56Dvu0GDCD7Z4BV9gj94sBOmxPDDBVxOsxyFIArBk0RCx1zZDXQIEhVglxRgx7C9QYNkWMkw3D2sAlI+qQmzLiBrhhxEKBzDmjdEXRGXUJCWVBidmMEkYeUR2mcI2IvMEU1kWE9Smz0rQVZW4RUlwzymzC1jQZtakgaMIoqwpH1WLUDwRUKDGEMKREA35zSpxBOKOEnoOnSZkyhAplOtjJ6ZS5gHBBGVPF1kdKaRA0Sn2B8MqkXBuONuWsIlzXjDMoAIUB43yC6zvjLZeMp8E1Y0YzuEbMaARXjRnVIIyZyRhbRwNmMDiCp9wdp9Qt48V/4w+w+ukPK6SgHwAAAABJRU5ErkJggg==";
    private static readonly string Base64Error64L = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAllBMVEVsNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS2ZpB7zAAAAMXRSTlMABgkPEhgkLTY8P0JFSEtaXWNmaW9ydXiEh5OWmZ+lq66xtMPGzM/S1djb5+rt8/b8LVl7zAAAAfJJREFUWMPdV9F2gjAMpeBAJ7rJ1Kllm0ULqIjm/39uD0OlbQLlsCfvm4330iZpkjrOE4OFUfwjpfyKF9GYdWW/zEQJNZRi9tKB/i4BgXyzpE9TIJBOLeijPTRgP2qhuxtowcZt4vsZtCLzaf6wAAsUQ4r/WoIVyjHx/QtY4oLuwS/AGgXiBzeDDsjNWMTQCbGRfppd6AzB1d9aUrKjps80BcG0PR7VC7ow9qcqCGaccqls4GSer64gmOmnU30LEeafh4JgmKejmkCG+vemIBgaq7yWg0R8/hQEI6L9yMd1bXXLNAXBDMkKa/QEugLNh+y2PFBDrirQfIBBtR6CkTR4odfTM6wMS7BSMPj3XOJgo2DygVcm04IoMOxflW0H7QoYH3aVEW0kqgLKh/S/BHofobcTLcOYkGG0TaSESiTrVE6IVPauNF+9TKrC1bsZJM3XrrOiIO/rK5qvLyRoQQkQzxIlrR6x4LGcogpIUeVYQdLLOqfLOqfKutpYONVYFP5JadBLI8OQ1sbp1qY3V441V97UXHu39/4DhuPmPUec3kNW/zHPcUJLhUtIjrpnG/552DBsW3gy9xvHfd7G527Li2FyaKIfJu1PFjYn41nM7V5vboS6Io9cxxrBSut36SpwOsKbfsRbKeV3/BlNPOeJ8QvN/oFIuwzOsgAAAABJRU5ErkJggg==";
    private static readonly string Base64Error64D = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAllBMVEXcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5BNWAzLAAAAMXRSTlMABgkPEhgkLTY8P0JFSEtaXWNmaW9ydXiEh5OWmZ+lq66xtMPGzM/S1djb5+rt8/b8LVl7zAAAAfJJREFUWMPdV9F2gjAMpeBAJ7rJ1Kllm0ULqIjm/39uD0OlbQLlsCfvm4330iZpkjrOE4OFUfwjpfyKF9GYdWW/zEQJNZRi9tKB/i4BgXyzpE9TIJBOLeijPTRgP2qhuxtowcZt4vsZtCLzaf6wAAsUQ4r/WoIVyjHx/QtY4oLuwS/AGgXiBzeDDsjNWMTQCbGRfppd6AzB1d9aUrKjps80BcG0PR7VC7ow9qcqCGaccqls4GSer64gmOmnU30LEeafh4JgmKejmkCG+vemIBgaq7yWg0R8/hQEI6L9yMd1bXXLNAXBDMkKa/QEugLNh+y2PFBDrirQfIBBtR6CkTR4odfTM6wMS7BSMPj3XOJgo2DygVcm04IoMOxflW0H7QoYH3aVEW0kqgLKh/S/BHofobcTLcOYkGG0TaSESiTrVE6IVPauNF+9TKrC1bsZJM3XrrOiIO/rK5qvLyRoQQkQzxIlrR6x4LGcogpIUeVYQdLLOqfLOqfKutpYONVYFP5JadBLI8OQ1sbp1qY3V441V97UXHu39/4DhuPmPUec3kNW/zHPcUJLhUtIjrpnG/552DBsW3gy9xvHfd7G527Li2FyaKIfJu1PFjYn41nM7V5vboS6Io9cxxrBSut36SpwOsKbfsRbKeV3/BlNPOeJ8QvN/oFIuwzOsgAAAABJRU5ErkJggg==";
    private static readonly string Base64Critical64L = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAA6lBMVEVsNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS1sNS26720gAAAATXRSTlMAAwYJDA8SFRgbHiEkJyotMDM5PD9CSEtOUVRXWl1gY2Zpb3J1eHt+gYSHjZCTlpmcn6Woq660t7q9xsnMz9LV2Nve4eTn6u3w8/b5/Gru7X4AAALWSURBVBgZ3cFrQ5pgAAXgAwmbtYYMXZdhuWa5i2KZcy3JLA1QO///7wxeBfESL37d8+B/ppg1p9133X7bqZkKdmW2fKb4LRO70Drc0NGQW3nMLcZl5GRNudXUQi7vPb7Be488OnxTBzmUmKEEuRtmuIGU4jODr0CmzEwVyFwy0yVkbpnpFjJ9ZupDZsxMY8gEzBRA5pWR6yPTPB9yYXhumkfXjLxChpEGIntDCsM9RBqMQGZKMlAh2BRsCGpAcgqZgOQAcyUKJcwNSAaQ8UgOMGdQMDA3IOlBZkDyGXMVChXMPZMcQOaOIQvCDYUbCBZDd5BxGBpZAJQrLlwpAKwRQw5kbAovrvvK0OMjQ6+u+0LBhkzBZ5ptM80vQKrGlKGiDJlSQw5Vjwm33XaZ8KrIRT3+PeOG2e9jFbk53OAgm1YtYslhqNul0O0y5GCpWNWwRulz1iog9oshw6BgGAz9QqzQmrGvYFWVoacSFuoMGQYFw2CojoXSE0NnWKE8MTKrqxAMhgyDgmEwZEBQ6zNGnhWkfebCqKYicuGThkHBMEj/AhG1NuLCZ6T9ZMJr6IiYJ7peE3T9xEREb3hM/ESaFXBp2v6ILT62p1wKLKzQa70xl/5UsKbyh0vjXk3HpqJ9x8TDmYqEejZg4s4u4k37zYAx70qHoF95jAXNfWTTLkaMTduHwGF7ytjoQkMOX/pM9HpM3H9BXp9uueH2E3ax3wyYEjT3saPDDlM6h9hNucc1vTJyU6sP3OKhqiIP/fKFsUnz4KA5YezlUofMB2fK2OibhpD2bcTYxPmADOpJj4m+rWBBsftM9E5UbGV9/xsw0bWwwuoy4f/9bmFN4drj0qR5gA0HrQmXvOsC0n5waVzXsJVWH3PpB9K+MubaCt6k2C5jX5H2bkbh8RQSp48UZu+wwmGktQepvRYjDlYVJ6R/jFxOfXJSxJqj+4aGnLTG/TH+H/8AB6OQR2EMy74AAAAASUVORK5CYII=";
    private static readonly string Base64Critical64D = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAA6lBMVEXcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5Dcm5C3xjwzAAAATXRSTlMAAwYJDA8SFRgbHiEkJyotMDM5PD9CSEtOUVRXWl1gY2Zpb3J1eHt+gYSHjZCTlpmcn6Woq660t7q9xsnMz9LV2Nve4eTn6u3w8/b5/Gru7X4AAALWSURBVBgZ3cFrQ5pgAAXgAwmbtYYMXZdhuWa5i2KZcy3JLA1QO///7wxeBfESL37d8+B/ppg1p9133X7bqZkKdmW2fKb4LRO70Drc0NGQW3nMLcZl5GRNudXUQi7vPb7Be488OnxTBzmUmKEEuRtmuIGU4jODr0CmzEwVyFwy0yVkbpnpFjJ9ZupDZsxMY8gEzBRA5pWR6yPTPB9yYXhumkfXjLxChpEGIntDCsM9RBqMQGZKMlAh2BRsCGpAcgqZgOQAcyUKJcwNSAaQ8UgOMGdQMDA3IOlBZkDyGXMVChXMPZMcQOaOIQvCDYUbCBZDd5BxGBpZAJQrLlwpAKwRQw5kbAovrvvK0OMjQ6+u+0LBhkzBZ5ptM80vQKrGlKGiDJlSQw5Vjwm33XaZ8KrIRT3+PeOG2e9jFbk53OAgm1YtYslhqNul0O0y5GCpWNWwRulz1iog9oshw6BgGAz9QqzQmrGvYFWVoacSFuoMGQYFw2CojoXSE0NnWKE8MTKrqxAMhgyDgmEwZEBQ6zNGnhWkfebCqKYicuGThkHBMEj/AhG1NuLCZ6T9ZMJr6IiYJ7peE3T9xEREb3hM/ESaFXBp2v6ILT62p1wKLKzQa70xl/5UsKbyh0vjXk3HpqJ9x8TDmYqEejZg4s4u4k37zYAx70qHoF95jAXNfWTTLkaMTduHwGF7ytjoQkMOX/pM9HpM3H9BXp9uueH2E3ax3wyYEjT3saPDDlM6h9hNucc1vTJyU6sP3OKhqiIP/fKFsUnz4KA5YezlUofMB2fK2OibhpD2bcTYxPmADOpJj4m+rWBBsftM9E5UbGV9/xsw0bWwwuoy4f/9bmFN4drj0qR5gA0HrQmXvOsC0n5waVzXsJVWH3PpB9K+MubaCt6k2C5jX5H2bkbh8RQSp48UZu+wwmGktQepvRYjDlYVJ6R/jFxOfXJSxJqj+4aGnLTG/TH+H/8AB6OQR2EMy74AAAAASUVORK5CYII=";
    private static readonly string Base64Question64L = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAA51BMVEUtR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3ItR3JlFdg1AAAATHRSTlMAAwYJDA8SFRgbHiEkJyotMDM2OTw/QkhLTlRaXWBjZmlvcnV4e36Eh4qQk5aZnJ+ipairrrG0t7q9xsnMz9LY297h5Ofq7fDz9vn8Rve2iQAAAmBJREFUGBntwWtD0lAABuDXxWBOIJYakZAKeQnY0MoLsMhMJmzv//89GfOws8MZQz/7PHiTp+w4ZbyOfeD6j1x49N0DGy9S6oypGHdK2FTFDakRuhVswuxGzBB1TeSqT7nGtI4c7YhrRW2sYwyYa2AgkzXmBsYWMpgTbmRiQssYckNDAzo9poXXnY+OhS3Habp/mdKDRoMp09MSJJ9HlDWwohhQdmlCcTRnIihC1afsEKtqj0z0oShHlBxD50PIpaiMNJeSGwjG3idnG8I5Ey5SrJCSCmLGWcQnt7uIvZtxKbQga1Pi49l3xuY1xC6ZaEPmU+Ii1qDQQ+yECR8Sm7IeYh6FLmJtSmwkmpT9wsJOQGEXsS4lTSQ8ppxvAdi/p+Dh2ZgSD4kR0/647m8u/TQQq1A2QiJgttkxhGvKAiwVmM0rQvjCtAIEm1nmLSwdhEyzIVSZpQ7B6FJVhVBlhksINZ8rqhBsZnAQK7rUsCEUqPeA2P6UOgUsBdQaYmFnTp0AiRG1LrDwlVojJAbU6mOhTy0PiRa1rrBwRa0WEja1wjKe2HNq2ZD41LrbA/buqPULsg4zzOfM0IHMCvlCoYUUj1o3Z4enP6jjIa0ScdX9Lv6rjrkiqkDR54rZe8S2H6jqQ1UMqPIgnFARFLGiQdUphBYVDWj0qLiF4DGtBx1jSMURYo2IKUMDWuaEaZG7DaD4bc6UiYkM1piqe/+OirGFTMaAuQYG1mlHXCtqI0d9yjWmdeQyexEzRD0Tm6i4ITVCr4JNlTo+FX6nhBexmxf+jAsz/6Jp43VqjlPDmxz/ACIuPso9asyVAAAAAElFTkSuQmCC";
    private static readonly string Base64Question64D = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAA51BMVEWQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+GQr+EwL7FuAAAATHRSTlMAAwYJDA8SFRgbHiEkJyotMDM2OTw/QkhLTlRaXWBjZmlvcnV4e36Eh4qQk5aZnJ+ipairrrG0t7q9xsnMz9LY297h5Ofq7fDz9vn8Rve2iQAAAmBJREFUGBntwWtD0lAABuDXxWBOIJYakZAKeQnY0MoLsMhMJmzv//89GfOws8MZQz/7PHiTp+w4ZbyOfeD6j1x49N0DGy9S6oypGHdK2FTFDakRuhVswuxGzBB1TeSqT7nGtI4c7YhrRW2sYwyYa2AgkzXmBsYWMpgTbmRiQssYckNDAzo9poXXnY+OhS3Habp/mdKDRoMp09MSJJ9HlDWwohhQdmlCcTRnIihC1afsEKtqj0z0oShHlBxD50PIpaiMNJeSGwjG3idnG8I5Ey5SrJCSCmLGWcQnt7uIvZtxKbQga1Pi49l3xuY1xC6ZaEPmU+Ii1qDQQ+yECR8Sm7IeYh6FLmJtSmwkmpT9wsJOQGEXsS4lTSQ8ppxvAdi/p+Dh2ZgSD4kR0/647m8u/TQQq1A2QiJgttkxhGvKAiwVmM0rQvjCtAIEm1nmLSwdhEyzIVSZpQ7B6FJVhVBlhksINZ8rqhBsZnAQK7rUsCEUqPeA2P6UOgUsBdQaYmFnTp0AiRG1LrDwlVojJAbU6mOhTy0PiRa1rrBwRa0WEja1wjKe2HNq2ZD41LrbA/buqPULsg4zzOfM0IHMCvlCoYUUj1o3Z4enP6jjIa0ScdX9Lv6rjrkiqkDR54rZe8S2H6jqQ1UMqPIgnFARFLGiQdUphBYVDWj0qLiF4DGtBx1jSMURYo2IKUMDWuaEaZG7DaD4bc6UiYkM1piqe/+OirGFTMaAuQYG1mlHXCtqI0d9yjWmdeQyexEzRD0Tm6i4ITVCr4JNlTo+FX6nhBexmxf+jAsz/6Jp43VqjlPDmxz/ACIuPso9asyVAAAAAElFTkSuQmCC";

}
