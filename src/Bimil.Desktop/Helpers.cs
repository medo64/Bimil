namespace Bimil.Desktop;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

internal static class Helpers {

    public static T GetControl<T>(Window window, string name) where T : Control {
        return window.FindControl<T>(name) ?? throw new ArgumentNullException(nameof(name), "Control not found.");
    }

    public static void FocusControl(Window window, string name) {
        var control = window.FindControl<Control>(name) ?? throw new ArgumentNullException(nameof(name), "Control not found.");
        if (control.IsAttachedToVisualTree()) {
            control.Focus(NavigationMethod.Tab);
        } else {
            control.AttachedToVisualTree += (sender, e) => { control.Focus(NavigationMethod.Tab); };
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

        private static T GetControlEtc<T>(Window window, string name, string propertyPath, out PropertyInfo propertyInfo) where T : Control {  // ignore possibility of the non-existent name
            var control = window.FindControl<T>(name)!;
            var type = Assembly.GetExecutingAssembly().GetType("Bimil.Desktop." + propertyPath[0..propertyPath.LastIndexOf('.')].Replace('.', '+'))!;
            propertyInfo = type.GetProperty(propertyPath[(propertyPath.LastIndexOf('.') + 1)..])!;
            return control;
        }

        public static void SetupCheckBox(Window window, string name, string propertyPath, Action? runOnUpdate = null) {  // ignore possibility of the non-existent name
            var control = GetControlEtc<CheckBox>(window, name, propertyPath, out var propertyInfo);
            control.IsChecked = (bool)propertyInfo.GetValue(null)!;
            control.IsCheckedChanged += (sender, e) => {
                propertyInfo.SetValue(null, control.IsChecked);
                runOnUpdate?.Invoke();
            };
        }

        public static void SetupTextBoxFromInt32(Window window, string name, string propertyPath, int minValue, int maxValue, Action? runOnUpdate = null) {  // ignore possibility of the non-existent name
            var control = GetControlEtc<TextBox>(window, name, propertyPath, out var propertyInfo);
            control.Text = ((int)propertyInfo.GetValue(null)!).ToString(CultureInfo.CurrentUICulture);
            control.TextChanged += (sender, e) => {
                if (int.TryParse(control.Text, NumberStyles.Integer, CultureInfo.CurrentUICulture, out var newValue) && (newValue >= minValue) && (newValue <= maxValue)) {
                    var oldValue = (int)propertyInfo.GetValue(null)!;
                    if (oldValue != newValue) {  // change only if different
                        propertyInfo.SetValue(null, newValue);
                        runOnUpdate?.Invoke();
                        control.Background = GetBrush("SystemAltMediumLowColor");  // doesn't change brush immediatelly :(
                        control.InvalidateMeasure();
                    }
                } else {
                    control.Background = GetRedBrush("SystemAltMediumLowColor");  // doesn't change brush immediatelly :(
                    control.InvalidateMeasure();
                }
            };
            control.LostFocus += (sender, e) => {
                control.Text = ((int)propertyInfo.GetValue(null)!).ToString(CultureInfo.CurrentUICulture);
            };
        }

    }
}
