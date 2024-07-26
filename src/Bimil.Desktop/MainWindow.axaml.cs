namespace Bimil.Desktop;

using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using Medo.Diagnostics;
using Bimil.Core;
using System.IO;
using Medo.Avalonia;
using Medo.Configuration;
using System.Threading.Tasks;

internal partial class MainWindow : Window {

    public MainWindow() {
        InitializeComponent();

        ThemeImageResources.Updated += (s, e) => {
            Helpers.FindChild<Image>(mnuFileNew)!.Source = ThemeImageResources.Enabled!.FileNew;
            Helpers.FindChild<Image>(mnuFileOpen)!.Source = ThemeImageResources.Enabled!.FileOpen;
            Helpers.FindChild<Image>(mnuFileSave)!.Source = mnuFileSave.IsEnabled ? ThemeImageResources.Enabled!.FileSave : ThemeImageResources.Disabled!.FileSave;
            Helpers.FindChild<Image>(mnuFileProperties)!.Source = mnuFileProperties.IsEnabled ? ThemeImageResources.Enabled!.FileProperties : ThemeImageResources.Disabled!.FileProperties;
            Helpers.FindChild<Image>(mnuItemAdd)!.Source = mnuItemAdd.IsEnabled ? ThemeImageResources.Enabled!.ItemAdd : ThemeImageResources.Disabled!.ItemAdd;
            Helpers.FindChild<Image>(mnuItemView)!.Source = mnuItemView.IsEnabled ? ThemeImageResources.Enabled!.ItemView : ThemeImageResources.Disabled!.ItemView;
            Helpers.FindChild<Image>(mnuItemEdit)!.Source = mnuItemEdit.IsEnabled ? ThemeImageResources.Enabled!.ItemEdit : ThemeImageResources.Disabled!.ItemEdit;
            Helpers.FindChild<Image>(mnuItemRemove)!.Source = mnuItemRemove.IsEnabled ? ThemeImageResources.Enabled!.ItemRemove : ThemeImageResources.Disabled!.ItemRemove;
            Helpers.FindChild<Image>(mnuFind)!.Source = mnuFind.IsEnabled ? ThemeImageResources.Enabled!.Find : ThemeImageResources.Disabled!.Find;
            Helpers.FindChild<Image>(mnuPasswordGenerator)!.Source = mnuPasswordGenerator.IsEnabled ? ThemeImageResources.Enabled!.PasswordGenerator : ThemeImageResources.Disabled!.PasswordGenerator;
            Helpers.FindChild<Image>(mnuApp)!.Source = mnuApp.IsEnabled ? ThemeImageResources.Enabled!.App : ThemeImageResources.Disabled!.App;
        };
        ThemeImageResources.Setup(this);

        // ThemeVariant
        switch (Settings.Theme) {
            case Settings.ThemeVariant.Light: AppAvalonia.Current!.RequestedThemeVariant = ThemeVariant.Light; break;
            case Settings.ThemeVariant.Dark: AppAvalonia.Current!.RequestedThemeVariant = ThemeVariant.Dark; break;
            default: break;
        }

        // Centralized log handler
        UnhandledCatch.Attach();
        UnhandledCatch.UnhandledException += (sender, e) => {
            if (e.Exception != null) {
                Trace.WriteLine(e.Exception.Message);
                Trace.WriteLine(e.Exception.StackTrace);
                Dispatcher.UIThread.Invoke(delegate {
                    Medo.Avalonia.FeedbackBox.ShowDialog(this, new Uri("https://medo64.com/feedback/"), e.Exception);
                });
            }
        };

        // State update
        State.StateChanged += (sender, e) => {
            var file = State.File;
            var lblFileName = Helpers.GetControl<Label>(this, "lblFileName");
            var lblLastSave = Helpers.GetControl<Label>(this, "lblLastSave");

            if (file != null) {
                Title = file.Name;
                lblFileName.Content = file.FullName;
                lblLastSave.Content = (file.LastWriteTime != DateTime.MinValue)
                                    ? file.LastWriteTime.ToShortDateString() + " " + file.LastWriteTime.ToLongTimeString()
                                    : "";
                RecentFiles.Add(file);
            } else {
                Title = "Bimil";
                lblFileName.Content = "";
                lblLastSave.Content = "";
            }

            var hasDocument = (State.Document != null);
            mnuFileSave.IsEnabled = hasDocument;
            mnuFileProperties.IsEnabled = hasDocument;
            mnuItemAdd.IsEnabled = hasDocument;
            mnuItemView.IsEnabled = false;  // will enable when item is selected
            mnuItemEdit.IsEnabled = false;
            mnuItemRemove.IsEnabled = false;
            mnuFind.IsEnabled = hasDocument;

            ThemeImageResources.Update();  // enable/disable buttons
        };
    }


    protected override async void OnOpened(EventArgs e) {
        base.OnOpened(e);

        while (!IsActive) { await Task.Delay(10); }  // wait for window to be fully initialized; otherwise it doesn't center right

        var files = RecentFiles.GetFiles();
        if (files.Count > 0) {
            if (Settings.ShowStart) {
                var frm = new StartWindow();
                await frm.ShowDialog(this);
                if (frm.SelectedFile != null) {
                    OpenFile(frm.SelectedFile, frm.SelectedReadonly);
                }
            } else if (Settings.LoadLast) {
                OpenFile(files[0], @readonly: false);
            }
        }
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        switch (e.Key) {
            case Key.Escape:
                if (Settings.CloseOnEscape) { Close(); }
                break;

            default: base.OnKeyDown(e); break;
        }
    }


    private async void OpenFile(FileInfo file, bool @readonly) {
        while (true) {  // repeat until successful or given up
            try {
                var frm = PasswordWindow.GetEnterPasswordWindow(file.Name);
                await frm.ShowDialog(this);
                if (frm.Password != null) {
                    State.OpenFile(file, frm.Password, @readonly);
                }
                break;
            } catch (Exception ex) {
                MessageBox.ShowErrorDialog(this, "Error opening file", ex.Message);
            }
        }
    }


    #region Menu

    public async void nnuFileNew_Click(object sender, RoutedEventArgs e) {
        var frm = PasswordWindow.GetNewPasswordWindow("Select password");
        await frm.ShowDialog(this);
        if (frm.Password != null) {
            State.NewFile(frm.Password);
        }
    }

    public void mnuFileOpen_Opened(object sender, RoutedEventArgs e) {
        var root = (Menu)sender!;
        var menu = (MenuItem)root.Items[0]!;
        for (var i = menu.Items.Count - 1; i > 1; i--) {
            menu.Items.RemoveAt(i);
        }

        var files = RecentFiles.GetFiles();
        var separatorMenuItem = (MenuItem)menu.Items[1]!;
        separatorMenuItem.IsVisible = (files.Count > 0);

        foreach (var file in files) {
            var menuItem = new MenuItem { Header = file.Name, Tag = file };
            menuItem.Click += (s, e) => {
                OpenFile((FileInfo)((MenuItem)s!).Tag!, @readonly: false);
            };
            menu.Items.Add(menuItem);
        }
    }

    public async void mnuFileOpen_Click(object sender, RoutedEventArgs e) {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
            Title = "Open Text File",
            FileTypeFilter = new FilePickerFileType[] {
                new("Bimil and PasswordSafe") { Patterns = [ "*.bimil", "*.passwordsafe" ], MimeTypes = [ "/*" ] },
                new("Bimil") { Patterns = [ "*.bimil" ], MimeTypes = [ "/*" ] },
                new("PasswordSafe") { Patterns = [ "*.passwordsafe" ], MimeTypes = [ "/*" ] },
                FilePickerFileTypes.All
            },
            AllowMultiple = false
        });
        if (files.Count > 0) {
            var fileInfo = new FileInfo(Uri.UnescapeDataString(files[0].Path.AbsolutePath));
            OpenFile(fileInfo, @readonly: false);
        }
    }

    public void mnuFileSave_Click(object sender, RoutedEventArgs e) {

    }

    public void mnuFileSaveAs_Click(object sender, RoutedEventArgs e) {

    }

    public void mnuFileProperties_Click(object sender, RoutedEventArgs e) {

    }

    public void mnuFilePropertiesPassword_Click(object sender, RoutedEventArgs e) {

    }

    public void mnuFilePropertiesReadonly_Click(object sender, RoutedEventArgs e) {

    }


    public void mnuItemAdd_Click(object sender, RoutedEventArgs e) {

    }

    public void mnuItemEdit_Click(object sender, RoutedEventArgs e) {

    }

    public void mnuItemView_Click(object sender, RoutedEventArgs e) {

    }

    public void mnuItemRemove_Click(object sender, RoutedEventArgs e) {

    }


    public void mnuFind_Click(object sender, RoutedEventArgs e) {

    }

    public async void mnuPasswordGenerator_Click(object sender, RoutedEventArgs e) {
        var frm = new PasswordGeneratorWindow();
        await frm.ShowDialog(this);
    }


    public void OnMenuAppOpened(object sender, RoutedEventArgs e) {
        var app = AppAvalonia.Current!;
        var menuItemDark = this.FindControl<MenuItem>("MenuAppDarkMode") ?? throw new InvalidOperationException("Cannot find menu item.");
        var menuItemLight = this.FindControl<MenuItem>("MenuAppLightMode") ?? throw new InvalidOperationException("Cannot find menu item.");

        if ((app.ActualThemeVariant ?? ThemeVariant.Light) == ThemeVariant.Light) {
            menuItemDark.IsVisible = true;
            menuItemLight.IsVisible = false;
        } else {
            menuItemDark.IsVisible = false;
            menuItemLight.IsVisible = true;
        }
    }

    public void OnMenuAppDarkModeClick(object sender, RoutedEventArgs e) {
        AppAvalonia.Current!.RequestedThemeVariant = ThemeVariant.Dark;
        Settings.Theme = Settings.ThemeVariant.Dark;
    }

    public void OnMenuAppLightModeClick(object sender, RoutedEventArgs e) {
        AppAvalonia.Current!.RequestedThemeVariant = ThemeVariant.Light;
        Settings.Theme = Settings.ThemeVariant.Light;
    }

    public async void mnuAppOptions_Click(object sender, RoutedEventArgs e) {
        var frm = new OptionsWindow();
        await frm.ShowDialog(this);
    }

    public void mnuAppFeedback_Click(object sender, RoutedEventArgs e) {
        Medo.Avalonia.FeedbackBox.ShowDialog(this, new Uri("https://medo64.com/feedback/"));
    }

    public void mnuAppUpgrade_Click(object sender, RoutedEventArgs e) {
        Medo.Avalonia.UpgradeBox.ShowDialog(this, new Uri("https://medo64.com/upgrade/"));
    }

    public void mnuAppAbout_Click(object sender, RoutedEventArgs e) {
        Medo.Avalonia.AboutBox.ShowDialog(this, new Uri("https://medo64.com/bimil/"));
    }

    #endregion Menu

}
