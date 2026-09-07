namespace Bimil;

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

internal static class Replenishment {

    public static void FillGroups(Document document, ComboBox cmbGroups, bool includeAnyGroup = false) {
        var previousGroup = (cmbGroups.SelectedItem as ComboBoxItem)?.Tag as string;
        cmbGroups.Items.Clear();

        if (includeAnyGroup) {
            cmbGroups.Items.Add(new ComboBoxItem { Content = "(any group)", Tag = null });
        }

        var groups = document.GetGroupList();
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

    public static void FillGroups(Document document, AutoCompleteBox cmbGroups) {
        cmbGroups.ItemsSource = document.GetGroupList();
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
