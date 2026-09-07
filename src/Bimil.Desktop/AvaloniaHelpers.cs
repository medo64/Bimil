namespace Bimil;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Medo;

internal static class AvaloniaHelpers {

    private static readonly FontFamily AlternateFontFamily = new("avares://Bimil/Assets/Fonts/SeriousShanns-Light.otf#Serious Shanns Light");

    public static void RegisterForAlternateFont(params TextBox?[] textBoxes) {
        if (textBoxes == null) { return; }
        foreach (var textBox in textBoxes) {
            if (textBox != null) {
                var originalFontFamily = textBox.FontFamily;
                textBox.KeyDown += (sender, e) => {
                    if (e.Key == Key.F1) {
                        var isPass = textBox.PasswordChar != '\0';
                        if (textBox.FontFamily != AlternateFontFamily) {
                            textBox.FontFamily = AlternateFontFamily;
                            textBox.FontSize *= 2;
                            if (isPass) { textBox.RevealPassword = true; }
                        } else {
                            textBox.FontFamily = originalFontFamily;
                            textBox.FontSize /= 2;
                            if (isPass) { textBox.RevealPassword = false; }
                        }
                    }
                };
            }
        }
    }

    public static void RegisterForTwoFactorView(TextBox textBox, TextBox textBoxCode, BinaryField field) {
        textBoxCode.FontFamily = AlternateFontFamily;
        textBoxCode.FontSize *= 2;

        void keyDownHandler(object? sender, KeyEventArgs e) {
            if (e.Key == Key.F1) {
                var panel = (Panel)textBox.Parent!;
                var timer = textBoxCode.Tag as Timer;
                if (textBox.IsVisible) {
                    textBox.IsVisible = false;
                    panel.Children.Remove(textBoxCode);
                    panel.Children.Add(textBoxCode);
                    textBoxCode.IsVisible = true;
                    if (timer == null) {
                        textBoxCode.Tag = new Timer(_ => {  // TODO: find clearer way of cleaning this up
                            TimeBasedOtp otp;
                            byte[] secret = field?.Data.GetBytes() ?? [];
                            try {
                                otp = new TimeBasedOtp(secret);
                                var code = otp.GetCodeAsText(CodeOutputFormat.None);
                                Dispatcher.UIThread.Post(() => {
                                    if (textBoxCode.Text != code) {
                                        textBoxCode.Text = code;
                                        textBoxCode.SelectAll();
                                    }
                                });
                            } finally {
                                Array.Clear(secret, 0, secret.Length);
                            }
                        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                    } else {
                        timer?.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
                    }
                    FocusControl(textBoxCode);
                } else {
                    textBoxCode.IsVisible = false;
                    panel.Children.Remove(textBox);
                    panel.Children.Add(textBox);
                    textBox.IsVisible = true;
                    timer?.Change(TimeSpan.Zero, Timeout.InfiniteTimeSpan);
                    FocusControl(textBox);
                }
            }
        }
        textBox.KeyDown += keyDownHandler;
        textBoxCode.KeyDown += keyDownHandler;
    }

    public static void SetupDialog(Window window) {
        void CloseWindow() {
            Dispatcher.UIThread.InvokeAsync(() => {
                window.WindowState = WindowState.Normal;
                window.Close();
            });
        }
        window.PropertyChanged += (sender, e) => {  // close on minimize
            if ((e.Property == Window.WindowStateProperty) && (window.WindowState == WindowState.Minimized)) {
                CloseWindow();
            }
        };
        window.KeyDown += (sender, e) => {
            if ((e.Key, e.KeyModifiers) == (Key.Escape, KeyModifiers.None)) {
                CloseWindow();
            }
        };
    }

    public static void FocusControl(Control control) {
        if (control.IsAttachedToVisualTree()) {
            control.Focus(NavigationMethod.Tab);
        } else {
            control.AttachedToVisualTree += (sender, e) => { control.Focus(NavigationMethod.Tab); };
        }
    }

    public static T? FindChild<T>(ContentControl control) {
        if (control.Content is T childT) {
            return childT;
        } else if (control.Content is Panel panel) {
            foreach (var childControl in panel.Children) {
                if (childControl is T childControlT) {
                    return childControlT;
                }
            }
        }
        return default;
    }

    public static void DisableTab(Control control) {
        control.IsTabStop = false;
        if (control is Panel panel) {
            foreach (var child in panel.Children) {
                DisableTab(child);
            }
        } else if (control is ContentControl content) {
            if (content.Content is Control contentControl) {
                DisableTab(contentControl);
            }
        } else if (control is Menu menu) {
            foreach (var child in menu.Items) {
                if (child is MenuItem childItem) {
                    DisableTab(childItem);
                }
            }
        }
    }


    private static readonly Dictionary<string, ISolidColorBrush> BrushCache = [];
    public static ISolidColorBrush GetBrush(string name) {
        if (!BrushCache.TryGetValue(name, out var brush)) {
            var variant = Application.Current?.ActualThemeVariant ?? ThemeVariant.Light;
            if (Application.Current?.Styles[0] is IResourceProvider provider && provider.TryGetResource(name, variant, out var resource)) {
                if (resource is Color color) {
                    brush = new SolidColorBrush(color);
                    BrushCache.Add(name, brush);
                }
            }
        }
        return brush ?? throw new ArgumentOutOfRangeException(nameof(name), "Brush not found");
    }

    private static readonly Dictionary<string, ISolidColorBrush> RedBrushCache = [];
    public static ISolidColorBrush GetRedBrush(string name) {
        if (!RedBrushCache.TryGetValue(name, out var redBrush)) {
            var brush = GetBrush(name);
            var hslColor = brush.Color.ToHsl();
            if (hslColor.L < 0.4) {
                redBrush = new SolidColorBrush(HslColor.FromHsl(0, 1, 0.25).ToRgb());
            } else if (hslColor.L > 0.6) {
                redBrush = new SolidColorBrush(HslColor.FromHsl(0, 1, 0.75).ToRgb());
            } else {
                redBrush = new SolidColorBrush(HslColor.FromHsl(0, 1, 0.50).ToRgb());
            }
            RedBrushCache.Add(name, redBrush);
        }
        return redBrush ?? throw new ArgumentOutOfRangeException(nameof(name), "Brush not found");
    }


    internal static class ControlSetup {

        private static PropertyInfo GetControlPropertyInfo(Control control, string propertyPath) {  // ignore possibility of the non-existent name
            var type = Assembly.GetExecutingAssembly().GetType("Bimil." + propertyPath[0..propertyPath.LastIndexOf('.')].Replace('.', '+'))!;
            return type.GetProperty(propertyPath[(propertyPath.LastIndexOf('.') + 1)..])!;
        }

        public static void SetupCheckBox(CheckBox control, string propertyPath, Action? runOnUpdate = null) {  // ignore possibility of the non-existent name
            var propertyInfo = GetControlPropertyInfo(control, propertyPath);
            control.IsChecked = (bool)propertyInfo.GetValue(null)!;
            control.IsCheckedChanged += (sender, e) => {
                propertyInfo.SetValue(null, control.IsChecked);
                runOnUpdate?.Invoke();
            };
        }

        public static void SetupTextBoxFromInt32(TextBox control, string propertyPath, int minValue, int maxValue, Action? runOnUpdate = null) {  // ignore possibility of the non-existent name
            var propertyInfo = GetControlPropertyInfo(control, propertyPath);
            control.Text = ((int)propertyInfo.GetValue(null)!).ToString(CultureInfo.CurrentUICulture);
            control.TextChanged += (sender, e) => {
                if (int.TryParse(control.Text, NumberStyles.Integer, CultureInfo.CurrentUICulture, out var newValue) && (newValue >= minValue) && (newValue <= maxValue)) {
                    var oldValue = (int)propertyInfo.GetValue(null)!;
                    if (oldValue != newValue) {  // change only if different
                        propertyInfo.SetValue(null, newValue);
                        runOnUpdate?.Invoke();
                        control.Background = GetBrush("SystemAltMediumLowColor");  // doesn't change brush immediately :(
                        control.InvalidateMeasure();
                    }
                } else {
                    control.Background = GetRedBrush("SystemAltMediumLowColor");  // doesn't change brush immediately :(
                    control.InvalidateMeasure();
                }
            };
            control.LostFocus += (sender, e) => {
                control.Text = ((int)propertyInfo.GetValue(null)!).ToString(CultureInfo.CurrentUICulture);
            };
        }

    }

}
