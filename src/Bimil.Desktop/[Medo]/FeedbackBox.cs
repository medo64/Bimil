/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2024-07-07: Initial Avalonia version

namespace Medo.Avalonia;

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Interactivity;
using global::Avalonia.Layout;
using global::Avalonia.Media;
using global::Avalonia.Styling;
using global::Avalonia.Threading;

/// <summary>
/// Simple about dialog.
/// </summary>
public static class FeedbackBox {

    /// <summary>
    /// Opens a window and gives opportunity to send feedback.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="feedbackUri">Address of form which will receive data.</param>
    public static void ShowDialog(Window owner, Uri feedbackUri) {
        ShowDialogInternal(owner, feedbackUri, exception: null);
    }

    /// <summary>
    /// Opens a window and gives opportunity to send exception.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="feedbackUri">Address of form which will receive data.</param>
    /// <param name="exception">Exception to send.</param>
    public static void ShowDialog(Window owner, Uri feedbackUri, Exception exception) {
        ShowDialogInternal(owner, feedbackUri, exception);
    }

    /// <summary>
    /// Opens a window and gives opportunity to show exception.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="exception">Exception to show.</param>
    public static void ShowDialog(Window owner, Exception exception) {
        ShowDialogInternal(owner, feedbackUri: null, exception);
    }

    /// <summary>
    /// Opens a window and gives opportunity to send feedback.
    /// </summary>
    /// <param name="owner">Window that owns this window.</param>
    /// <param name="feedbackUri">Address of form which will receive data.</param>
    private static void ShowDialogInternal(Window owner, Uri? feedbackUri, Exception? exception) {
        var window = new Window() { MinWidth = 800, MaxWidth = 1200 };
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
        window.Title = (exception == null) ? "Feedback" : "Error Report";

        var privacyTextBlock = new TextBlock() {
            Foreground = GetBrush("SystemAccentColor", Brushes.DarkBlue, Brushes.LightBlue),
            Margin = new Thickness(0, 0, 0, 11),
            Text = "This report will not contain any personal data not provided by you.",
        };

        var messageTextBlock = new TextBlock() {
            Margin = new Thickness(0, 7, 0, 0),
            Text = "What do you wish to report?",
        };
        var messageTextBox = new TextBox() {
            AcceptsReturn = true,
            Height = 90,
            Margin = new Thickness(0, 7, 0, 0),
            TextWrapping = TextWrapping.Wrap,
        };

        var gridContact = new Grid();
        gridContact.RowDefinitions.Add(new RowDefinition());
        gridContact.RowDefinitions.Add(new RowDefinition());
        gridContact.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
        gridContact.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var emailTextBlock = new TextBlock() {
            Margin = new Thickness(0, 7, 7, 0),
            Text = "Email (optional):",
            VerticalAlignment = VerticalAlignment.Center,
        };
        var emailTextBox = new TextBox() {
            Margin = new Thickness(0, 7, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
        };
        gridContact.Children.Add(emailTextBlock);
        Grid.SetRow(emailTextBlock, 0);
        Grid.SetColumn(emailTextBlock, 0);
        gridContact.Children.Add(emailTextBox);
        Grid.SetRow(emailTextBox, 0);
        Grid.SetColumn(emailTextBox, 1);

        var displayNameTextBlock = new TextBlock() {
            Margin = new Thickness(0, 7, 7, 0),
            Text = "Name (optional):",
            VerticalAlignment = VerticalAlignment.Center,
        };
        var displayNameTextBox = new TextBox() {
            Margin = new Thickness(0, 7, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
        };
        gridContact.Children.Add(displayNameTextBlock);
        Grid.SetRow(displayNameTextBlock, 1);
        Grid.SetColumn(displayNameTextBlock, 0);
        gridContact.Children.Add(displayNameTextBox);
        Grid.SetRow(displayNameTextBox, 1);
        Grid.SetColumn(displayNameTextBox, 1);

        var detailsTextBlock = new TextBlock() {
            Margin = new Thickness(0, 11, 0, 0),
            Text = "Additional data that will be sent:",
        };
        var detailsTextBox = new TextBox() {
            FontFamily = new FontFamily("Courier New"),
            Height = 180,
            Margin = new Thickness(0, 7, 0, 0),
            IsReadOnly = true,
            Text = GetAdditionalDetails(exception),
        };

        var mainStack = new StackPanel() { Margin = new Thickness(11) };
        if (feedbackUri != null) { mainStack.Children.Add(privacyTextBlock); }
        mainStack.Children.Add(messageTextBlock);
        mainStack.Children.Add(messageTextBox);
        mainStack.Children.Add(gridContact);
        mainStack.Children.Add(detailsTextBlock);
        mainStack.Children.Add(detailsTextBox);

        var statusTextBlock = new TextBlock() {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Text = ""
        };
        var buttonPanelLeft = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 11, 0) };
        var sendButton = new Button() {
            Content = (exception == null) ? "Send Feedback" : "Send Error Report",
            Margin = new Thickness(0, 0, 7, 0),
        };
        sendButton.Click += SendClick;
        buttonPanelLeft.Children.Add(sendButton);
        if (feedbackUri == null) { sendButton.IsVisible = false; }
        var closeButton = new Button() {
            Content = "Close",
            IsDefault = true,
            IsCancel = true,
            HorizontalAlignment = HorizontalAlignment.Right,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(21, 0, 0, 0),
        };
        closeButton.Click += CloseClick;
        var buttonDockPanel = new DockPanel() { Margin = new Thickness(11) };
        buttonDockPanel.Children.Add(buttonPanelLeft);
        buttonDockPanel.Children.Add(statusTextBlock);
        buttonDockPanel.Children.Add(closeButton);

        var windowStack = new StackPanel();
        windowStack.Children.Add(new TextBlock {
            Background = GetBrush("SystemBaseHighColor", Brushes.DarkGray, Brushes.LightGray),
            Foreground = GetBrush("SystemAltHighColor", Brushes.White, Brushes.Black),
            FontSize = window.FontSize * 1.25,
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 0, 0, 0),
            Padding = new Thickness(11),
            Text = window.Title,
        });
        windowStack.Children.Add(new Border() { Child = mainStack });
        windowStack.Children.Add(buttonDockPanel);

        var windowBorder = new Border {
            BorderThickness = new Thickness(1),
            BorderBrush = GetBrush("SystemChromeHighColor", Brushes.Black, Brushes.White),
            Child = windowStack
        };

        var bag = new Bag(feedbackUri!,  // send not visible when null
                          messageTextBox,
                          displayNameTextBox,
                          emailTextBox,
                          detailsTextBox,
                          window,
                          statusTextBlock);
        closeButton.Tag = bag;
        sendButton.Tag = bag;

        window.Opened += (s, e) => { messageTextBox.Focus(); };

        window.Content = windowBorder;
        if (owner != null) {
            window.ShowDialog(owner);
        } else {
            window.Show();
        }
    }

    private static void SendClick(object? sender, RoutedEventArgs e) {
        var button = (Button)sender!;
        button.IsEnabled = false;

        var bag = (Bag)button.Tag!;
        ThreadPool.QueueUserWorkItem(delegate {
            Dispatcher.UIThread.Invoke(delegate { bag.StatusBlock.Text = "Sending…"; });

            // prepare data
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var allFormParameters = new NameValueCollection {
                { "Product", GetAppProduct(assembly) },
                { "Version", GetAppVersion(assembly) },
                { "Message", (bag.Message + "\r\n\r\n" + bag.Details).Trim() },
            };
            if (!string.IsNullOrEmpty(bag.Email)) { allFormParameters.Add("Email", bag.Email); }
            if (!string.IsNullOrEmpty(bag.DisplayName)) { allFormParameters.Add("DisplayName", bag.DisplayName); }

            var sbPostData = new StringBuilder();
            for (var i = 0; i < allFormParameters.Count; ++i) {
                if (sbPostData.Length > 0) { sbPostData.Append('&'); }
                sbPostData.Append(UrlEncode(allFormParameters.GetKey(i)!) + "=" + UrlEncode(allFormParameters[i]!));
            }

            // send it
            try {
                using var request = new HttpRequestMessage(HttpMethod.Post, bag.FeedbackUri);
                request.Content = new StringContent(sbPostData.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

                var responseTask = HttpClient.SendAsync(request);
                responseTask.Wait();
                var response = responseTask.Result;

                if (response.IsSuccessStatusCode) {
                    var responseContentTask = response.Content.ReadAsStringAsync();
                    responseContentTask.Wait();
                    var responseContent = responseContentTask.Result;
                    if (string.IsNullOrEmpty(responseContent)) {
                        Dispatcher.UIThread.Invoke(delegate { bag.StatusBlock.Text = "Sent!"; });
                        Thread.Sleep(500);
                        Dispatcher.UIThread.Invoke(delegate { bag.Window.Close(); });
                        return;
                    } else {
                        Dispatcher.UIThread.Invoke(delegate { bag.StatusBlock.Text = "Error response from server"; });
                    }
                } else {
                    Dispatcher.UIThread.Invoke(delegate { bag.StatusBlock.Text = "Error response from server: " + ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture) + " " + response.ReasonPhrase; });
                }
            } catch (AggregateException aggEx) {
                if (aggEx.InnerException != null) {
                    Dispatcher.UIThread.Invoke(delegate { bag.StatusBlock.Text = "Error sending request: " + aggEx.InnerException.Message; });
                } else {
                    Dispatcher.UIThread.Invoke(delegate { bag.StatusBlock.Text = "Error sending request"; });
                }
            }

            Dispatcher.UIThread.Invoke(delegate { button.IsEnabled = true; });
        });
    }

    private static void CloseClick(object? sender, RoutedEventArgs e) {
        var bag = (Bag)((Button)sender!).Tag!;
        bag.CancelSource.Cancel();
        bag.Window.Close();
    }


    private static readonly HttpClient HttpClient = new();

    private static string UrlEncode(string text) {
        var source = System.Text.UTF8Encoding.UTF8.GetBytes(text);
        var sb = new StringBuilder();
        for (var i = 0; i < source.Length; ++i) {
            if (((source[i] >= 48) && (source[i] <= 57)) || ((source[i] >= 65) && (source[i] <= 90)) || ((source[i] >= 97) && (source[i] <= 122)) || (source[i] == 45) || (source[i] == 46) || (source[i] == 95) || (source[i] == 126)) { //A-Z a-z - . _ ~
                sb.Append(System.Convert.ToChar(source[i]));
            } else {
                sb.Append("%" + source[i].ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
            }
        }
        return sb.ToString();
    }

    private static string GetAdditionalDetails(Exception? exception) {
        var logBuilder = new StringBuilder();
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        AppendLine("Environment:", logBuilder);
        AppendLine(GetAppProduct(assembly) + " " + GetAppVersion(assembly), logBuilder, 0, true);
        AppendLine($".NET Framework {Environment.Version}", logBuilder, 0, true);
        var osPrettyName = GetOSPrettyName();
        if (osPrettyName != null) {
            AppendLine(System.Environment.OSVersion.ToString() + " (" + osPrettyName + ")", logBuilder, 0, true);
        } else {
            AppendLine(System.Environment.OSVersion.ToString(), logBuilder, 0, true);
        }
        AppendLine("Local time is " + DateTime.Now.ToString(@"yyyy\-MM\-dd\THH\:mm\:ssK", System.Globalization.CultureInfo.InvariantCulture), logBuilder, 0, true); //it will fail in OutOfMemory situation

        if (exception != null) {
            var ex = exception;
            var exLevel = 0;
            while (ex != null) {
                AppendLine("", logBuilder);

                if (exLevel == 0) {
                    AppendLine(ex.GetType().Name + ":", logBuilder);
                } else {
                    AppendLine(ex.GetType().Name + " (" + exLevel.ToString(CultureInfo.InvariantCulture) + "):", logBuilder);
                }
                AppendLine(ex.Message, logBuilder, 0, true);
                if (!string.IsNullOrEmpty(ex.StackTrace)) {
                    AppendLine(ex.StackTrace, logBuilder, 1, false);
                }

                ex = ex.InnerException;
                exLevel += 1;
                if (exLevel > 3) { break; }  // even this is too much
            }

            AppendLine("", logBuilder);
            AppendLine("Referenced assemblies:", logBuilder);
            if (assembly.FullName != null) { AppendLine(assembly.FullName, logBuilder, 0, true); }
            foreach (var iRefAss in assembly.GetReferencedAssemblies()) {
                AppendLine(iRefAss.ToString(), logBuilder, 0, true);
            }
        }

        return logBuilder.ToString();
    }

    private const int LineLength = 72;

    private static void AppendLine(string input, StringBuilder output) {
        AppendLine(input, output, 0, false);
    }

    private static void AppendLine(string input, StringBuilder output, int indentLevel, bool tickO) {
        if (input == null) { return; }
        input = input.Trim();
        if (input.Length == 0) {
            output.AppendLine();
            return;
        }

        if (tickO) {
            indentLevel += 1;
        }


        var maxWidth = LineLength - indentLevel * 2;
        var end = input.Length - 1;

        var firstChar = 0;

        int lastChar;
        int nextChar;
        do {
            if ((end - firstChar) < maxWidth) {
                lastChar = end;
                nextChar = end + 1;
            } else {
                var nextCrBreak = input.IndexOf('\r', firstChar, maxWidth);
                var nextLfBreak = input.IndexOf('\n', firstChar, maxWidth);
                int nextCrLfBreak;
                if (nextCrBreak == -1) {
                    nextCrLfBreak = nextLfBreak;
                } else if (nextLfBreak == -1) {
                    nextCrLfBreak = nextCrBreak;
                } else {
                    nextCrLfBreak = System.Math.Min(nextCrBreak, nextLfBreak);
                }
                if ((nextCrLfBreak != -1) && ((nextCrLfBreak - firstChar) <= maxWidth)) {
                    lastChar = nextCrLfBreak - 1;
                    nextChar = lastChar + 2;
                    if (nextChar <= end) {
                        if ((input[nextChar] == '\n') || (input[nextChar] == '\r')) {
                            nextChar += 1;
                        }
                    }
                } else {
                    var nextSpaceBreak = input.LastIndexOf(' ', firstChar + maxWidth, maxWidth);
                    if ((nextSpaceBreak != -1) && ((nextSpaceBreak - firstChar) <= maxWidth)) {
                        lastChar = nextSpaceBreak;
                        nextChar = lastChar + 1;
                    } else {
                        var nextOtherBreak1 = input.LastIndexOf('-', firstChar + maxWidth, maxWidth);
                        var nextOtherBreak2 = input.LastIndexOf(':', firstChar + maxWidth, maxWidth);
                        var nextOtherBreak3 = input.LastIndexOf('(', firstChar + maxWidth, maxWidth);
                        var nextOtherBreak4 = input.LastIndexOf(',', firstChar + maxWidth, maxWidth);
                        var nextOtherBreak = System.Math.Max(nextOtherBreak1, System.Math.Max(nextOtherBreak2, System.Math.Max(nextOtherBreak3, nextOtherBreak4)));
                        if ((nextOtherBreak != -1) && ((nextOtherBreak - firstChar) <= maxWidth)) {
                            lastChar = nextOtherBreak;
                            nextChar = lastChar + 1;
                        } else {
                            lastChar = firstChar + maxWidth;
                            if (lastChar > end) { lastChar = end; }
                            nextChar = lastChar;
                        }
                    }
                }
            }

            if (tickO) {
                for (var i = 0; i < indentLevel - 1; ++i) { output.Append("  "); }
                output.Append("◦ ");
                tickO = false;
            } else {
                for (var i = 0; i < indentLevel; ++i) { output.Append("  "); }
            }
            for (var i = firstChar; i <= lastChar; ++i) {
                output.Append(input[i]);
            }
            output.AppendLine();

            firstChar = nextChar;
        } while (nextChar <= end);
    }

    private static string GetAppProduct(Assembly assembly) {
        var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
        if ((productAttributes != null) && (productAttributes.Length >= 1)) {
            return ((AssemblyProductAttribute)productAttributes[^1]).Product;
        } else {
            return GetAppTitle(assembly);
        }
    }

    private static string GetAppTitle(Assembly assembly) {
        var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
        if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
            return ((AssemblyTitleAttribute)titleAttributes[^1]).Title;
        } else {
            return assembly.GetName().Name ?? "";
        }
    }

    private static string GetAppVersion(Assembly assembly, bool minorMajorOnly = false) {
        var version = assembly.GetName().Version;
        if (version != null) {
            if (minorMajorOnly) {
                return $"{version.Major}.{version.Minor}";
            } else {
                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
        }
        return "";
    }

    private static readonly string[] NewLineArray = ["\n\r", "\n", "\r"];

    private static string? GetOSPrettyName() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            try {
                var osReleaseLines = File.ReadAllText("/etc/os-release").Split(NewLineArray, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in osReleaseLines) {
                    if (line.StartsWith("PRETTY_NAME=", StringComparison.OrdinalIgnoreCase)) {
                        var text = line[12..].Trim();
                        if (text.StartsWith('"') && text.EndsWith('"')) {
                            return text[1..^1];
                        }
                    }
                }
            } catch (SystemException) { }
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            switch (Environment.OSVersion.Version.Major) {
                case 5: return "Windows XP";
                case 6:
                    switch (Environment.OSVersion.Version.Minor) {
                        case 0: return "Windows Vista";
                        case 1: return "Windows 7";
                        case 2: return "Windows 8";
                        case 3: return "Windows 8.1";
                    };
                    break;
                case 10: return (Environment.OSVersion.Version.Build > 22000) ? "Windows 11" : "Windows 10";
            }
        }
        return null;
    }


    private static ISolidColorBrush GetBrush(string name, ISolidColorBrush lightDefault, ISolidColorBrush darkDefault) {
        var variant = Application.Current?.ActualThemeVariant ?? ThemeVariant.Light;
        if (Application.Current?.Styles[0] is IResourceProvider provider && provider.TryGetResource(name, variant, out var resource)) {
            if (resource is Color color) {
                return new SolidColorBrush(color);
            }
        }
        Debug.WriteLine("[FeedbackBox] Cannot find brush " + name);
        return (variant == ThemeVariant.Light) ? lightDefault : darkDefault;
    }

    private record Bag {
        public Bag(Uri feedbackUri, TextBox messageBox, TextBox displayNameBox, TextBox emailBox, TextBox detailsBox, Window window, TextBlock statusBlock) {
            FeedbackUri = feedbackUri;
            MessageBox = messageBox;
            DisplayNameBox = displayNameBox;
            EmailBox = emailBox;
            DetailsBox = detailsBox;
            Window = window;
            StatusBlock = statusBlock;
            CancelSource = new CancellationTokenSource();
        }

        private readonly TextBox MessageBox;
        private readonly TextBox DisplayNameBox;
        private readonly TextBox EmailBox;
        private readonly TextBox DetailsBox;

        public Uri FeedbackUri { get; }
        public string Message => GetText(MessageBox).Trim();
        public string DisplayName => GetText(DisplayNameBox).Trim();
        public string Email => GetText(EmailBox).Trim();
        public string Details => GetText(DetailsBox).Trim();
        public Window Window { get; }
        public TextBlock StatusBlock { get; }
        public CancellationTokenSource CancelSource { get; }

        private static string GetText(TextBox box) {
            return Dispatcher.UIThread.Invoke<string>(delegate {
                return box.Text ?? "";
            });
        }
    }
}
