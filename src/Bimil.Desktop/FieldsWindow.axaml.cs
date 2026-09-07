namespace Bimil;

using System;
using System.Collections;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Medo;
using Medo.Avalonia;

internal partial class FieldsWindow : Window {

    public FieldsWindow(Field[] fields) {
        InitializeComponent();
        AvaloniaHelpers.SetupDialog(this);
        InputElement.KeyDownEvent.AddClassHandler<TopLevel>((sender, e) => {
            if ((e.KeyModifiers == KeyModifiers.None) && (e.Key == Key.Enter)) {
                OnKeyDown(e);
            }
        }, handledEventsToo: true);

        foreach (var record in fields) {
            var captionBlock = new TextBlock() { Text = record.Caption, Tag = record };
            lsbFields.Items.Add(captionBlock);
        }
    }

    public void btnClose_Click(object sender, RoutedEventArgs e) {
        Close();
    }

}
