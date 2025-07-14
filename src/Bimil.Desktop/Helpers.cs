namespace Bimil.Desktop;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Bimil.Core;
using HarfBuzzSharp;
using Medo.Security.Cryptography.PasswordSafe;

internal static class Helpers {

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
            var type = Assembly.GetExecutingAssembly().GetType("Bimil.Desktop." + propertyPath[0..propertyPath.LastIndexOf('.')].Replace('.', '+'))!;
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

    public static string? GetRecordCaption(RecordType recordType) {
        switch (recordType) {
            case RecordType.UserName: return "User name";
            case RecordType.Password: return "Password";
            case RecordType.Url: return "URL";
            case RecordType.EmailAddress: return "E-mail";
            case RecordType.Notes: return "Notes";

            case RecordType.TwoFactorKey: return "Two-factor key";
            case RecordType.CreditCardNumber: return "Card number";
            case RecordType.CreditCardExpiration: return "Card expiration";
            case RecordType.CreditCardVerificationValue: return "Card security code";
            case RecordType.CreditCardPin: return "Card PIN";

            case RecordType.QRCode: return "QR Code";

            case RecordType.RunCommand: return "Run command";

            default: return null; //all other fields are not really supported
        }
    }

    public static void ReplenishGroups(ComboBox cmbGroups, bool includeAnyGroup = false) {
        var previousGroup = (cmbGroups.SelectedItem as ComboBoxItem)?.Tag as string;
        cmbGroups.Items.Clear();

        if (includeAnyGroup) {
            cmbGroups.Items.Add(new ComboBoxItem { Content = "(any group)", Tag = null });
        }

        var groups = State.GetGroups();
        if (groups.Count > 0) {
            foreach (var group in groups) {
                var groupText = (group.Length > 0) ? group : "(no group)";
                cmbGroups.Items.Add(new ComboBoxItem { Content = groupText, Tag = group });
            }
        }

        foreach (var item in cmbGroups.Items) {
            if ((item is ComboBoxItem comboBoxItem) && ((comboBoxItem.Tag as string) == previousGroup)) {
                cmbGroups.SelectedItem = comboBoxItem;
                break;
            }
        }
    }

    public static void SelectGroup(ComboBox cmbGroups, string group) {
        foreach (var item in cmbGroups.Items) {
            if ((item is ComboBoxItem comboBoxItem) && (comboBoxItem.Tag is string currGroup)) {
                if (group.Equals(currGroup, StringComparison.CurrentCultureIgnoreCase)) {
                    cmbGroups.SelectedItem = item;
                    return;
                }
            }
        }
    }

}
