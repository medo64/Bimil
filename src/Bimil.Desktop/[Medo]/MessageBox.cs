/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

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

    private static string Base64Info64L = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAANKSURBVHhe5dtPp1VRGMfxO2oUEdHojiIiomnz7qhRRESv4dD7iIiIuC+gaaOmERG9jIiIiKjfN2tfe5/127v951l7n2MNPpzznL3X86x1z91r7T/n5Pb9XdVssCY2WBMbrIkNruSG3E147bYpzgaDncpjeSWf5Lv86cFnbMO27MO+rs0wNhjgmjyTj+I6OgVt0BZtulyL2OACN4W/3i9xnVmCNmmbHC73LDY4wxV5Lr/FFR+JHOQip6tlEhuc6Ey+iiu2JHKS29U0mg1OsJM1/up9yE0NrrZRbHCES/JGXFFboBZqcrUOssH/uC4RR/do1ERtruZeNjiAA88XcQUcAmqbdHC0wR58xd6LS3xIqHH0v4MN9nghLuEholbXh4wNGg/EJZrrnTCFsdQFr4m5beeiZteXDhvcc1W+iUsyFdPWI3F5wGdR0yo1U7vLc8EG97wUl2CO1+JytLGN23cOanc5LthgC6epkQudMSs3tnH7zkHtg6faNtjCyYdreK61BwD0weX5xwYTFhXRZ3WDxSTRg04fehdINpiwxnYNLsFX8qG4fOCzyH+5Ru/5gg0mXJlxjUV4K/eEozR4TcxtG4G+uD72DgBzs2vomNnLa1kgeSKukWNGn7K+ZoEkci4+FHYNkgWSD+IaOWb0KetrFkiilr7OT+Gg5PCZ2ycCfcr6mgXksrgGonDO7vKi9LUG+tbJ2XmTlJ4BthyAbCbovEluids5ypYDQN86OTtvkuoHoPp/geoPgig5DW41AKOnQZRcCG01AJMWQiXv+mw1AJOWwk/FNRJhqwGgT1nOLJCUnAm2GoBsBkAWaGFt7hpaaosB+Cwu3+AA8FiKa2ypLQaAvrh8gwNQ4qIo1h6A2RdFUeLCyNoDYI/+DRts4YGk6Ku0aw4AtQ8+VGWDeyJvjWHNAVh8awyRN0ex1gBQc8jNUUTeHl9rAMJujzeqfkAC1T8ig6ofkmqwqKj2MbkGX7FqH5Rsq/ZR2Tae6Kj2YekGBx6mnjW+DeQg16SDXR8bXKD0DyY4sTnIH0zsa34yE3FRhTaO5iczDpeieDjhXOjMD3EdBZ+xDduyj72MFckGV3JHmp/N8dptU5wN1sQGa2KDNbHBeuxO/gIiJrGMJP37aAAAAABJRU5ErkJggg==";
    private static string Base64Info64D = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAANLSURBVHhe5du9qtRAGMbxU1kJgiBYWQmClWDrBVhZCYIgeA2C93FAEARB8AK0tLIVBEHwDk4rCIIgCII+f5kckp0nMR/vJFmm+MHuu8m878zZk5l87Mnp27Oq2WBNbLAmNlgTG1zJdbmd8NptU5wNBrsmD+WFfJLv8qcHn7EN27IP+7o2w9hggCvyVD6K6+gUtEFbtOlyLWKDC9wQ/nq/xHVmCdqkbXK43LPY4AyX5FR+iys+EjnIRU5XyyQ2ONFd+Squ2JLISW5X02g2OMETWeOv3ofc1OBqG8UGR7ggr8QVtQVqoSZX6yAb/I+rEnF0j0ZN1OZq7mWDAzjwfBFXwB5Q26SDow324Cv2XlziPaHG0f8ONtjjmbiEe0Strg8ZGzTuiUs01zthCmOpC14Tc9vORc2uLx02eOCyfBOXZCqmrQfi8oDPoqZVaqZ2l+ecDR54Li7BHC/F5WhjG7fvHNTucpyzwRZOUyMXOmNWbmzj9p2D2gdPtW2whZMP1/Bcaw8A6IPL848NJiwqos/qBotJogedPvQukGwwYY3tGlyCr+R9cfnAZ5H/co3e8wUbTLgy4xqL8EbuCEdp8JqY2zYCfXF97B0A5mbX0DGzl9eyQPJIXCPHjD5lfc0CSeRcvBd2DZIFkg/iGjlm9CnraxZIopa+zk/hoOTwmdsnAn3K+poF5KK4BqJwzu7yovS1BvrWydl5k5SeAbYcgGwm6LxJborbOcqWA0DfOjk7b5LqB6D6f4HqD4IoOQ1uNQCjp0GUXAhtNQCTFkIl7/psNQCTlsKPxTUSYasBoE9ZziyQlJwJthqAbAZAFmhhbe4aWmqLAfgsLt/gAPBYimtsqS0GgL64fIMDUOKiKNYegNkXRVHiwsjaA2CP/g0bbOGBpOirtGsOALUPPlRlgwcib41hzQFYfGsMkTdHsdYAUHPIzVFE3h5fawDCbo83qn5AAtU/IoOqH5JqsKio9jG5Bl+xah+UbKv2Udk2nuio9mHpBgcepp41vg3kINekg10fG1yg9A8mOLHZ5Q8mDjU/mYm4qEIbR/OTGYdLUTyc8FrozA9xHQWfsQ3bso+9jBXJBldyS5qfzfHabVOcDdbEBmtigzWxwXqcnfwFJaRSuMOBfakAAAAASUVORK5CYII=";
    private static string Base64Warning64L = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAQlSURBVHhe5dtPpB1XAMfxRwmlTwillFKyCqWUUkpXJTxCySpkFUopJZRSSlellFJKCVllVUIppYQQsgohZFVKKSFkFUop7e9Xc64z531n5vyb6z2z+PDu7845Z87Me/f8mfsOjo4ONw3DLcFwSzDcEgz36Jy8nGR7heHKXpCP5U/5d/BEPhE6flUYruiM/Cih4ym/R+VWg+GKPhPqeOxTobKrwHAl/nt/JtTp2N9yXqiO7jBcyTdCHSa3hOroDsMV+I76zlJnp7wrVFdXGK7gtlAn59wXqqsrDDvznaQO5rgiVGc3GHb2UKhzOf4QD51UbxcYduQ7SB0r4aGT6u4Cw05ekni2V+u5eAilNpph2EnOpCfX90JtNMOwg1fFd446E3hS9NNgaYL0j1wQaqsJhh38INSR4IG8IuF4rwid0bHBzxK30QWGjd4U3zHqRECTnLeFjo1dlLRcEwwb/Sp08sFToXLmZTGVCTykejlNZatg2OBI6MRjj4TKmt+jMrGPhMpWwbCS78xjoZOOeWik8vabUJmYy3uIpfLFMKzkO0MnnPKiaOrX+C+hMqmvhMoXw7CC70jOWj+IR4DAkx06lvhCeahN6yiGYQXfETrRKW9JWscbQsdO6bJngGGh16V0rf+BpPXkfICm6EIWwbCQ7wSd3Bz6JP9Q6Ng59yStpwiGBWrX+vQh9qXQsUsuS1pXNgwL+A7QSS25KWldN4SOXeKhs3rPAMNMLWv9O5LWtzSDnFO9lY5hBl/x34VOJofvWlpnziRqiofgqj0DDDO0rvVpMrS0fF7yncT1ZcFwga9068naaxLq9HKYjilR9UAFwwXenaETKPWehDrfGbJWxXsGGM7wrszSWj/XNQn1Xh2yHooeqGA4w1eYGq3xrYR6/TMdU6NozwDDCd6NoQZreSTwiVrOMriEf6OoD8dgCHySLQ84png+YPRei+w9AwxB7lr/JPlCqC8jGCZ6PeDYNw/Vi3sGGCZK1/q5vAEangssbYbWojXHCIaRnAccpTxtpf0AZyW7Sjk8ZHubPm1rB8NIzVp/jmdr3v+ntszvlW6uLLkr1Nb/MBz4ylGFLXLm6z6GyrbwbhO1NXsBatf6c94XaivmY6hsi8k9g2PBwH+PVFErb3xSe7E1fvMMH6gcC8RXqvfMLLgk1GbMx1DZVrhnMHoxuC5UQQ8e8qjNmI+hsj18LaP2Ri/krPQeilLxKjDl96hMLx5hRpOjuHHz9JEK9uYvTcbfEvfPJV+kbPG57Pq8+2Hwi1ChtfhbYEbvrWX0Zxh33tZYmZ00ox3puPPWa7vrJBt94SruvPXamzvJ3Mddn+POB0tfcDrN3LdRf0cvBi/Kvj6R98l9ct9G/R29SHja6v/j8VU7zdyHySk4hluC4ZZguCUYbgmGW4Lhdhwe/Adue62PMNaPmgAAAABJRU5ErkJggg==";
    private static string Base64Warning64D = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAQlSURBVHhe5dtPpB1XAMfxUEIpJYRSSukqhBBKKV2VUkLoKmQVQgkl1CuldFVKKaWU0lVWJTwtJYQQsgqhZBVCKKF0VUop7e9Xc64z531n5vyb6z2z+PDu7845Z87Me/f8mftOHf50sGkYbgmGW4LhlmC4R2fkbJLtFYYre0FuyG/y7+C5fCR0/KowXNFp+VFCx1N+j8qtBsMVfSLU8djHQmVXgeFK/Pf+h1CnY3/LG0J1dIfhSr4S6jC5JVRHdxiuwHfUd5Y6O+Vtobq6wnAFt4U6OeeBUF1dYdiZ7yR1MMcVoTq7wbCzR0Kdy/FMPHRSvV1g2JHvIHWshIdOqrsLDDt5SeLZXq0/xUMotdEMw05yJj25vhVqoxmGHbwqvnPUmcCTosPB0gTpHzkn1FYTDDv4TqgjwUN5RcLxXhE6o2ODnyVuowsMG10Q3zHqRECTnDeFjo29J2m5Jhg2uiN08sHvQuXMy2IqE3hI9XKaylbBsMH7Qice+1WorPk9KhP7UKhsFQwr+c48FjrpmIdGKm9PhMrEXN5DLJUvhmEl3xk64ZQXRVO/xn8JlUl9IVS+GIYVfEdy1vpBPAIEnuzQscQXykNtWkcxDCv4jtCJTrkoaR3nhY6d0mXPAMNCr0vpWv+ypPXkfICm6EIWwbCQ7wSd3Bz6JL8udOyc+5LWUwTDArVrffoQ+1zo2CUfSFpXNgwL+A7QSS35QdK6vhc6domHzuo9Awwztaz170pa39IMck71VjqGGXzFnwqdTA7ftbTOnEnUFA/BVXsGGGZoXevTZGhp+bzkG4nry4LhAl/p1pO11yTU6eUwHVOi6oEKhgu8O0MnUOodCXW+NWStivcMMJzhXZmltX6uaxLqvTpkPRQ9UMFwhq8wNVrjawn1+mc6pkbRngGGE7wbQw3W8kjgE7WcZXAJ/0ZRH47AEPgkWx5wTPF8wOi9Ftl7BhiC3LX+cfKZUF9GMEz0esCxbx6qF/cMMEyUrvVzeQM0PBdY2gytRWuOEQwjOQ84SnnaSvsBzkp2lXJ4yPY2fdrWDoaRmrX+HM/WvP9PbZnfK91cWXJPqK3/YTjwlaMKW+TM130MlW3h3SZqa/YC1K7157wr1FbMx1DZFpN7BkeCgf8eqaJW3vik9mJr/OYZPlA5EoivVO+ZWXBJqM2Yj6GyrXDPYPRicFOogh485FGbMR9DZXv4UkbtjV7Iy9J7KErFq8CU36MyvXiEGU2O4sbN00cq2Ju/NBl/S9w/l3yRssWnsuvz7ofBL0KF1uJvgRm9t5bRn2HceVtjZXbcjHak485br+2u42z0hau489Zrb+44cx93fY47Hyx9wekkc99G/R29GLwo+/pE3if3yX0b9Xf0IuFpq/+Px1ftJHMfJqfgGG4JhluC4ZZguCUYbgmG23Fw6j+lvS67J1TOmgAAAABJRU5ErkJggg==";
    private static string Base64Error64L = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAQBSURBVHhe5ZqxitVAFIa3EqwEKx/AdkutBEEQBN9AsBIEQRAEwQcQfABBsNrKShAE30CwEgRBEHwAQbASrAQ9H2TCJPknmTkzuZslxVfk7M455/9vbnIyuUcPrxzvGhncEzK4J2RwBc4ZN437xivjjfEp4sQg/tjgf24YrFG5miKDjbhk3DPeGb+Nf4WwhrXkIJeqUY0MVnLX4FNVomog5x1D1XQjg05uG58N1XxLqEEt1UMxMljINeOjoZpdE2pSW/WUjQxmct54YajmDgk90IvqcREZzOCy8cVQDZ0G9EJPqtdZZHCBq8ZPQzVymtATvamek8jgDNcNzy3tUNAbM4TqXSKDCXD3j6EKbwl6zD4TZFDA92uLp30Kes26JsjgCK6wW7rg5fLVWLw7yOAIZnRV4CxA70pTjwxGMHGpxDlQnFle/a0EcrwcxUqYnRplsIOnse+GSrpEcJ4cNSawNjwVes9ENCSfLGWwg0dTlXCJ8WnnNSEWH/Ca8MSI8/TIoEHhH4ZKNsdYfJyvxAQlPuAxAS0y3yTQwaaESjRHSnwg14Q58QGPCWia5JoEOkpve0viA0sm5IgPlJrAbXGSZxIwmKJUghS54gMpE0rEB0pNmEyIg4OO54ZanOKtUdr42ASP+JSRc6BtkGdw0OGZ+mpMOJR4QNsg1+DAuGiohTl4TTiU+AAa+3xxYmDrWi3KxfNpllArHtDY54yTAwODWlTCWia0EA+DoSguADUzd0xrE1qJBzT2ueMi0KoItDKhpXggV58/LgQfDLXIS60JrcUDGvsacTFY48WG14Q1xAMa+zpxQdi9Abv/CrQsVis+0NoEcvX540Kw5dvge0PVKmX2Nrj1QaiFCbOD0FkYhWtNmB2FLxh/DbVwCY94/t+zxmsC2tDY54sTBzy/7vCKZ513rccEtA1yDQ46nhlqcYoa8bU5Sk3I2hA5NtTiFIOragZj8QGPCaV3LbQNcgwOIkonwlwTUuIDJSaUip/sBsEk0OHZFl8yYUl8IMeEUvFQtC1OA54XIykTcsUH5kzwiEeLfFM8CUR4h6KxCaXiA8oEj3gYDD8xMthBce/L0WCCV3wgNsEr3v1yFHb9ejxQ+vZlSyy+tZLBEVw8eK+mCmyZZj+RgV3/SCqw65/JBXiU3LIJ9DZ43F1CBhfA3V+GauA0oafsTz4ggxnw/drShZFesr7zY2QwE66wNffnVtDD4tU+hQwWcsv4Zqjm1oSa1FY9ZSODDhg1HxmHuFVSg1rJ8bYEGayAU5HHzjWuD+Qkt/t0V8hgI9h9YXut5nUba8kx2clphQyuADuxPJQ8MJjP+TkNG5SB1wbxpwafMt/twe7tWsjgnpDBPSGD++H46D9HDL0WNLKwFgAAAABJRU5ErkJggg==";
    private static string Base64Error64D = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAQASURBVHhe5ZqxqtVAEIZvJVgJVj6Ara2VIAiC4BsIVoJgJQiCDyDYXhAEsbCyEgTBNxCsBEEQBO0FwUqwEnQ+yIZN8m+yO7s5N5cUX5G5d2fm/09OMtmco+8vjneNDO4JGdwTMrgCZ4zrxl3jufHa+Bjx0iD+wOB/rhmsUbmaIoONuGDcMd4av41/hbCGteQgl6pRjQxWctvgU1WiaiDnLUPVdCODTm4anwzVfEuoQS3VQzEyWMgV44Ohml0TalJb9ZSNDGZy1nhqqOYOCT3Qi+pxERnM4KLx2VANnQT0Qk+q11lkcIHLxk9DNXKS0BO9qZ6TyOAMVw3PLe1Q0BszhOpdIoMJcPePoQpvCXrMPhNkUMD3a4unfQp6zbomyOAIrrBbuuDl8sVYvDvI4AhmdFXgNEDvSlOPDEYwcanEOVCcWV79rQRyPBvFSpidGmWwg6exb4ZKukRwnhw1JrA2PBV6z0Q0JJ8sZbCDR1OVcInxaec1IRYf8Jrw0Ijz9MigQeEfhko2x1h8nK/EBCU+4DEBLTLfJNDBpoRKNEdKfCDXhDnxAY8JaJrkmgQ6Sm97S+IDSybkiA+UmsBtcZJnEjCYolSCFLniAykTSsQHSk2YTIiDg44nhlqc4o1R2vjYBI/4lJFzoG2QZ3DQ4Zn6akw4lHhA2yDX4MA4b6iFOXhNOJT4ABr7fHFiYOtaLcrF82mWUCse0NjnjJMDA4NaVMJaJrQQD4OhKC4ANTN3TGsTWokHNPa54yLQqgi0MqGleCBXnz8uBO8NtchLrQmtxQMa+xpxMVjjxYbXhDXEAxr7OnFB2L0Bu/8KtCxWKz7Q2gRy9fnjQrDl2+A7Q9UqZfY2uPVBqIUJs4PQaRiFa02YHYXPGX8NtXAJj3j+37PGawLa0NjnixMHPL/u8IpnnXetxwS0DXINDjoeG2pxihrxtTlKTcjaELlkqMUpBlfVDMbiAx4TSu9aaBvkGBxElE6EuSakxAdKTCgVP9kNgkmgw7MtvmTCkvhAjgml4qFoW5wGPC9GUibkig/MmeARjxb5pngSiPAORWMTSsUHlAke8TAYfmJksIPi3pejwQSv+EBsgle8++Uo7Pr1eKD07cuWWHxrJYMjuHjwXk0V2DLNfiIDu/6RVGDXP5ML8Ci5ZRPobfC4u4QMLoC7vwzVwElCT9mffEAGM+D7taULI71kfefHyGAmXGFr7s+toIfFq30KGSzkhvHVUM2tCTWprXrKRgYdMGreNw5xq6QGtZLjbQkyWAGnIo+da1wfyElu9+mukMFGsPvC9lrN6zbWkmOyk9MKGVwBdmJ5KLlnMJ/zcxo2KAOvDOKPDD5lvtuD3du1kME9IYN7Qgb3w/HRf7R7TkJxVkIEAAAAAElFTkSuQmCC";
    private static string Base64Critical64L = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAVXSURBVHhe5Zpf5G1FFMcvERERERERET8iPURETxEREREREZeIiIiIniIiInqKiJ4ioteIuERERFwul4iIiFqf6wzT+OzZs2bvc7qchw/Hd69Za82cPf/3pcuPXJw1Kp4TKp4TKh6BW4KHg5eDD4OPgu+C7w/wG41n2GBLGfO1KyruCBX5IPgt+CcJZSiLD/O9CyruwB3Bp4FVbAZ84dNibULFjTweXA2sIlvAJ74t5jQqbuCx4M/AKrAH+CaGxZ5CxUnuDq4HlvieEINYlkMaFSfZs8+vQSzLIY2KEzwUWKLHhJiWSwoVJ/gksCSPCTEtlxQqJmHBMjPPb4WYmxdLKiZharIET8ETgeU0jIpJ3ggsuVNAbMtpGBWTfBZYcqeA2JbTMComYSNjyZ0CYltOw6iY5BjL3lGIbTkNo2KS3wNL7hQQ23IaRsUkfweW3MfBUwHbWXgp+Ckw2xpssC3l8IEvsyW25TSMikkssbcCs70t6DUCz7Cxsvi0MmY7jIpJ2t0fr+WtgdnC80FtX8MzKwP4bLsbsc12GBWTtEn9EJhdobdvWFvf47u2vynGgHYLvNYAF0FtX8MzK1NoG4DYZjeMiknapH4JzK7A8rW2r1lb2uK7tl9r7FVUTPJ1UCcFvVOb3s6xt8PDZ2tPbLMdRsUkHGW3if0atI3Azu3NoLVtwabd5eELn60tsWu7NCom6Y3q14Jy9m/rhR8PtDq2pRw+2ueF3qwxhIpJbg9mzwOoQK8BexCT2JbTMCpOwG2OJdmDRQ+vOoysEFuIabmkUHGSF4LMqTCvN9dhwG+zMYhBLMshjYobYLX2dPBF8FdgFZgBX/jEd2+VmUbFnbDZYZbNo/0SKjZwJ8crd0+ljWAN8PmBVi8sPc82ALmS8+p9oooVDFDlxIfXkNva0ZH3vaCtCEvdtaWwPceXxWghN3Is3Y/c2zXFf1CxglZsk/k5GLmUeD1oy842AL4sRg05kVtb9sXA7G+g4gFazhwCLUxSvQHJKjLbAGgWA8iBXJYGXfYPi2+BigeeDMxhDctT5uOlhng1qBdJ2QagLD7MNzGJbUvkFupiProN8G5gzgzmZk5s7gzMF0dbzwQ8BxI3ynNsKWO+eE6szJqDupivbgOwAWkPO9bghIaFzYOB+dwCPvGd/f6AOizuTlWsKP/Wl0H2+PurYPPVVYAPfFmMJciVnMtbZX5voGIH5lc2L3YGsMSVgJE4s4LDljLtYUsPciK31HpFxUHuDd4PRrsJfZa9fu8f4Rk2o/2b2ORALuZvFRWTsNpipB4ZjaGMEw8ExQe/M/2bWMTc/OWYiht4LsjcFdJPwZ4Z3wbEsNhTqLgDjwZ73hrjC58WaxMq7kh2nKjZ3L9HUHFn6N8zX5BRph4njoKKO8GnM5n+vQQ+dv9CtKDiBpi/2UEy91tltoBPfGfWE6uoOAHzN9/r9I6wa/4I6N/3HeA3mtm2EINY3RXeKComuD/gtCYzf78W2PyNxrPR9QQNRmxyaH0No+IKvILs1jL9m7UBy9Tu6cwBbLDNrifIKd09VBTYTb0dfBNkpjTO9xZ3YgNQtneG2ML5ATmS61BcFQ9wvsanKZl9N9T92/zOgC/O+kbHiQK5U4fFc0wVD7wTmNMl2IJyNLV5fd4B38TIbs2pi/nrNsArgTlr4VZntH/vRRknRm+UqIv56TbAXcHSQSNwq/tsYGVPCTnYDXOBOlAXK7s6CNrlBtAfl77m+j8gF3KyXLuXKipWcLpSDzyMstzPme3NAG9DfQpN7t0TIhUb+FCRfTgnsccc4PaCHMmVnFf/LBXPCRXPCRXPh4tL/wKADwz243s4PAAAAABJRU5ErkJggg==";
    private static string Base64Critical64D = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAVXSURBVHhe5Zpf5G1FFMcvERERERERERE9RURPERERERHR0yUiIiJ6ih8RKXqKiJ4ioteIiEtEibhcLhEREbU+1xmm8dmzZ83e53Q5Dx+O716z1po5e/7vSz9/eHHWqHhOqHhOqHgEbgkeCV4O3g8+CL4NvjvAbzSeYYMtZczXrqi4I1TkveC34J8klKEsPsz3Lqi4A3cEnwRWsRnwhU+LtQkVN/J4cDWwimwBn/i2mNOouIHHgj8Dq8Ae4JsYFnsKFSe5O7geWOJ7QgxiWQ5pVJxkzz6/BrEshzQqTvBwYIkeE2JaLilUnODjwJI8JsS0XFKomIQFy8w8vxVibl4sqZiEqckSPAVPBJbTMComeT2w5E4BsS2nYVRM8mlgyZ0CYltOw6iYhI2MJXcKiG05DaNikmMse0chtuU0jIpJfg8suVNAbMtpGBWT/B1Ych8FTwVsZ+Gl4MfAbGuwwbaUwwe+zJbYltMwKiaxxN4MzPa2oNcIPMPGyuLTypjtMComaXd/vJa3BmYLzwe1fQ3PrAzgs+1uxDbbYVRM0iZ1JTC7Qm/fsLa+x3dtf1OMAe0WeK0BHgpq+xqeWZlC2wDENrthVEzSJvVLYHYFlq+1fc3a0hbftf1aY6+iYpKvgjop6J3a9HaOvR0ePlt7YpvtMCom4Si7TezXoG0Edm5vBK1tCzbtLg9f+GxtiV3bpVExSW9UvxaUs39bL/xwoNWxLeXw0T4v9GaNIVRMcnswex5ABXoN2IOYxLachlFxAm5zLMkeLHp41WFkhdhCTMslhYqTvBBkToV5vbkOA36bjUEMYlkOaVTcAKu1p4PPg78Cq8AM+MInvnurzDQq7oTNDrNsHu2XULGBOzleuXsqbQRrgM8OtHph6Xm2AciVnFfvE1WsYIAqJz68htzWjo687wZtRVjqri2F7Tm+LEYLuZFj6X7k3q4p/oOKFbRim8xPwcilxGtBW3a2AfBlMWrIidzasi8GZn8DFQ/QcuYQaGGS6g1IVpHZBkCzGEAO5LI06LJ/WHwLVDzwZGAOa1ieMh8vNcTloF4kZRuAsvgw38Qkti2RW6iL+eg2wDuBOTOYmzmxuTMwXxxtPRPwHEjcKM+xpYz54jmxMmsO6mK+ug3ABqQ97FiDExoWNg8G5nML+MR39vsD6rC4O1WxovxbXwTZ4+8vg81XVwE+8GUxliBXci5vlfm9gYodmF/ZvNgZwBLfB4zEmRUctpRpD1t6kBO5pdYrKg5yb3ARjHYT+ix7/d4/wjNsRvs3scmBXMzfKiomYbXFSD0yGkMZJx4Iig9+Z/o3sYi5+csxFTfwXJC5K6Sfgj0zvgmIYbGnUHEHHg32vDXGFz4t1iZU3JHsOFGzuX+PoOLO0L9nviCjTD1OHAUVd4JPZzL9ewl87P6FaEHFDTB/s4Nk7rfKbAGf+M6sJ1ZRcQLmb77X6R1h1/wR0L/vO8BvNLNtIQaxuiu8UVRMcH/AaU1m/n41sPkbjWej6wkajNjk0PoaRsUVeAXZrWX6N2sDlqnd05kD2GCbXU+QU7p7qCiwm3or+DrITGmc7y3uxAagbO8MsYXzA3Ik16G4Kh7gfI1PUzL7bqj7t/mdAV+c9Y2OEwVypw6L55gqHng7MKdLsAXlaGrz+rwDvomR3ZpTF/PXbYBXAnPWwq3OaP/eizJOjN4oURfz022Au4Klg0bgVvfZwMqeEnKwG+YCdaAuVnZ1ELTLDaA/Ln3N9X9ALuRkuXYvVVSs4HSlHngYZbmfM9ubAd6G+hSa3LsnRCo28KEi+3BOYo85wO0FOZIrOa/+WSqeEyqeEyqeDxeX/gXtfp4TA9KAoQAAAABJRU5ErkJggg==";

}
