namespace Bimil;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using Medo;
using Medo.Avalonia;
using Medo.Diagnostics;
using Medo.Security.Cryptography.PasswordSafe;

internal partial class MainWindow : Window {

    public MainWindow() {
        InitializeComponent();

        ThemeImageResources.Updated += (s, e) => {
            ThemeImageResources.SetImage(mnuFileNew, "FileNew");
            ThemeImageResources.SetImage(mnuFileOpen, "FileOpen");
            ThemeImageResources.SetImage(mnuFileSave, "FileSave");
            ThemeImageResources.SetImage(mnuFileProperties, "FileEdit");
            ThemeImageResources.SetImage(mnuItemAdd, "ItemAdd");
            ThemeImageResources.SetImage(mnuItemView, "ItemView");
            ThemeImageResources.SetImage(mnuItemEdit, "ItemEdit");
            ThemeImageResources.SetImage(mnuItemRemove, "ItemRemove");
            ThemeImageResources.SetImage(mnuFind, "Find");
            ThemeImageResources.SetImage(mnuPasswordGenerator, "PasswordGenerate");
            ThemeImageResources.SetImage(mnuApp, "App");
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
        UnhandledCatch.UnhandledException += (_, e) => {
            if (e.Exception != null) {
                Trace.WriteLine(e.Exception.Message);
                Trace.WriteLine(e.Exception.StackTrace);
                Dispatcher.UIThread.Invoke(delegate {
                    Medo.Avalonia.FeedbackBox.ShowDialog(this, new Uri("https://medo64.com/feedback/"), e.Exception);
                });
            }
        };

        // State update
        AvaloniaHelpers.DisableTab(mnu);
        State.DocumentChanged += (_, _) => { ReplenishDocument(); };
        State.GroupsChanged += (_, _) => { Replenishment.FillGroups(State, cmbGroups, includeAnyGroup: true); };
        State.ItemsChanged += (_, _) => { ReplenishEntries(); };
        ReplenishDocument();

        // attach events
        txtFilter.TextChanged += (_, _) => { ReplenishEntries(); };
        cmbGroups.SelectionChanged += (_, _) => { ReplenishEntries(); };
    }

    public readonly State State = new();


    protected override async void OnOpened(EventArgs e) {
        base.OnOpened(e);

        while (!IsActive) { await Task.Delay(10); }  // wait for window to be fully initialized; otherwise it doesn't center right

        var files = Config.Recent.Files;
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

    protected override void OnKeyDown(KeyEventArgs e) {
        Debug.WriteLine($"[MainWindow] KeyDown: {e.Key} (Modifiers: {e.KeyModifiers})");

        switch ((e.Key, e.KeyModifiers)) {
            case (Key.Escape, KeyModifiers.None):
                if (Settings.CloseOnEscape) { Close(); }
                break;

            case (Key.Enter, KeyModifiers.None):
                lsbEntries_DoubleTapped(null!, null!);
                break;

            case (Key.Down, KeyModifiers.None):
                if (lsbEntries.SelectedIndex < lsbEntries.Items.Count - 1) {
                    lsbEntries.SelectedIndex += 1;
                }
                break;

            case (Key.Up, KeyModifiers.None):
                if (lsbEntries.SelectedIndex > 0) {
                    lsbEntries.SelectedIndex -= 1;
                }
                break;

            case (Key.PageDown, KeyModifiers.None):
                if (cmbGroups.SelectedIndex < cmbGroups.Items.Count - 1) {
                    cmbGroups.SelectedIndex += 1;
                }
                break;

            case (Key.C, KeyModifiers.Control):
            case (Key.Insert, KeyModifiers.Control):
                mnuEntryCopy_Click(null!, null!);
                break;

            case (Key.N, KeyModifiers.Control):
            case (Key.N, KeyModifiers.Alt):
                mnuFileNew_Click(null!, null!);
                break;

            case (Key.O, KeyModifiers.Control):
                mnuFileOpen_Click(null!, null!);
                break;

            case (Key.O, KeyModifiers.Alt):
                mnuFileOpenDropDown.IsSubMenuOpen = true;
                break;

            case (Key.S, KeyModifiers.Control):
                mnuFileSave_Click(null!, null!);
                break;

            case (Key.S, KeyModifiers.Control | KeyModifiers.Shift):
                mnuFileSaveAs_Click(null!, null!);
                break;

            case (Key.S, KeyModifiers.Alt):
                mnuFileSaveDropDown.IsSubMenuOpen = true;
                break;

            case (Key.V, KeyModifiers.Control):
            case (Key.Insert, KeyModifiers.Shift):
                mnuEntryPaste_Click(null!, null!);
                break;

            case (Key.X, KeyModifiers.Control):
            case (Key.Delete, KeyModifiers.Shift):
                mnuEntryCut_Click(null!, null!);
                break;


            case (Key.Enter, KeyModifiers.Alt):
                mnuFileProperties_Click(null!, null!);
                break;

            case (Key.A, KeyModifiers.Alt):
            case (Key.OemPlus, KeyModifiers.None):
            case (Key.Add, KeyModifiers.None):
            case (Key.Insert, KeyModifiers.None):
                mnuItemAdd_Click(null!, null!);
                break;

            case (Key.E, KeyModifiers.Alt):
            case (Key.OemPipe, KeyModifiers.None):
            case (Key.Multiply, KeyModifiers.None):
                mnuItemEdit_Click(null!, null!);
                break;

            case (Key.R, KeyModifiers.Alt):
            case (Key.OemMinus, KeyModifiers.None):
            case (Key.Subtract, KeyModifiers.None):
            case (Key.Delete, KeyModifiers.None):
                mnuItemRemove_Click(null!, null!);
                break;

            case (Key.F, KeyModifiers.Control):
                mnuFind_Click(null!, null!);
                break;

            case (Key.F1, KeyModifiers.None):
                mnuAppDropDown.IsSubMenuOpen = true;
                break;

            case (Key.F5, KeyModifiers.None):
                ReplenishEntries();
                break;

            default: base.OnKeyDown(e); return;
        }

        e.Handled = true;
    }


    private async void OpenFile(FileInfo file, bool @readonly) {
        while (true) {  // repeat until successful or given up
            var frm = PasswordWindow.GetEnterPasswordWindow($"Enter password ({file.Name})");
            await frm.ShowDialog(this);
            try {
                if (frm.ExistingPassword != null) {
                    State.OpenFile(file, frm.ExistingPassword, @readonly);
                }
                break;
            } catch (Exception ex) {
                await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);  // allow UI to update
                MessageBox.ShowErrorDialog(this, "Error opening file", ex.Message);
            }
        }
        AvaloniaHelpers.FocusControl(txtFilter);
    }


    #region Menu

    public async void mnuFileNew_Click(object sender, RoutedEventArgs e) {
        if (mnuFileNew.IsEnabled == false) { return; }

        var frm = PasswordWindow.GetNewPasswordWindow("Select password");
        await frm.ShowDialog(this);
        if (frm.NewPassword != null) {
            State.NewFile(frm.NewPassword);
        }
    }

    public void mnuFileOpen_SubmenuOpened(object sender, RoutedEventArgs e) {
        var root = sender as Menu ?? (sender as MenuItem)?.Parent as Menu;
        if (root == null) { return; }

        var menu = (MenuItem)root.Items[0]!;
        for (var i = menu.Items.Count - 1; i > 1; i--) {
            menu.Items.RemoveAt(i);
        }

        var files = Config.Recent.Files;
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
        if (mnuFileOpen.IsEnabled == false) { return; }

        var filePickerOptions = new FilePickerOpenOptions {
            Title = "Open File",
            FileTypeFilter = new FilePickerFileType[] {
                new("Bimil and PasswordSafe") { Patterns = [ "*.bimil", "*.passwordsafe" ], MimeTypes = [ "/*" ] },
                new("Bimil") { Patterns = [ "*.bimil" ], MimeTypes = [ "/*" ] },
                new("PasswordSafe") { Patterns = [ "*.passwordsafe" ], MimeTypes = [ "/*" ] },
                FilePickerFileTypes.All
            },
            AllowMultiple = false,
        };
        if (Config.Recent.Files.TryGet(0, out var recentFile) && (recentFile.DirectoryName != null)) {
            filePickerOptions.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(recentFile.DirectoryName);
        }

        var files = await StorageProvider.OpenFilePickerAsync(filePickerOptions);
        if (files.Count > 0) {
            var fileInfo = new FileInfo(Uri.UnescapeDataString(files[0].Path.AbsolutePath));
            OpenFile(fileInfo, @readonly: false);
        }
    }

    public void mnuFileSave_Click(object sender, RoutedEventArgs e) {
        if (mnuFileSave.IsEnabled == false) { return; }

        if (State.File != null) {
            try {
                State.SaveFile();
            } catch (Exception ex) {
                MessageBox.ShowErrorDialog(this, "Save file", ex.Message);
            }
        } else {
            mnuFileSaveAs_Click(sender, e);
        }
    }

    public async void mnuFileSaveAs_Click(object sender, RoutedEventArgs e) {
        if (mnuFileSave.IsEnabled == false) { return; }

        var filePickerOptions = new FilePickerSaveOptions {
            Title = "Save File",
            FileTypeChoices = new FilePickerFileType[] {
                new("Bimil") { Patterns = [ "*.bimil" ], MimeTypes = [ "/*" ] },
                new("PasswordSafe") { Patterns = [ "*.passwordsafe" ], MimeTypes = [ "/*" ] },
            },
            DefaultExtension = ".bimil",
            ShowOverwritePrompt = false,
        };
        if (State.File != null) {
            filePickerOptions.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(State.File.DirectoryName!);
            filePickerOptions.SuggestedFileName = State.File.Name;
        } else if (Config.Recent.Files.TryGet(0, out var recentFile) && (recentFile.DirectoryName != null)) {
            filePickerOptions.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(recentFile.DirectoryName);
        }

        var file = await StorageProvider.SaveFilePickerAsync(filePickerOptions);
        if (file != null) {
            var fileInfo = new FileInfo(Uri.UnescapeDataString(file.Path.AbsolutePath));
            try {
                State.SaveFile(fileInfo);
            } catch (Exception ex) {
                MessageBox.ShowErrorDialog(this, "Save file", ex.Message);
            }
        }
    }

    public void mnuFileProperties_SubmenuOpened(object sender, RoutedEventArgs e) {
        mnuFilePropertiesReadonly.Header = (State.Document?.IsReadOnly ?? false)
                                         ? "Make read/write"
                                         : "Make read-only";
    }

    public async void mnuFileProperties_Click(object sender, RoutedEventArgs e) {
        if (mnuFileProperties.IsEnabled == false) { return; }

        if (State.Document != null) {
            var frm = new PropertiesWindow(State);
            await frm.ShowDialog(this);
        }
    }

    public async void mnuFilePropertiesPassword_Click(object sender, RoutedEventArgs e) {
        if (State.Document != null) {
            var title = (State.File != null) ? $"Change password ({State.File.Name})" : $"Change password";
            while (true) {  // repeat until successful or given up
                var frm = PasswordWindow.GetChangePasswordWindow(title, hasFile: true);
                await frm.ShowDialog(this);
                if ((frm.ExistingPassword != null) && (frm.NewPassword != null)) {
                    try {
                        if (State.Document!.ValidatePassphrase(frm.ExistingPassword) == false) { throw new InvalidOperationException("Cannot verify existing password."); }
                        State.Document!.ChangePassphrase(frm.NewPassword);
                        break;
                    } catch (Exception ex) {
                        MessageBox.ShowErrorDialog(this, "Error changing password", ex.Message);
                    }
                } else {
                    break;
                }
            }
        }
    }

    public void mnuFilePropertiesReadonly_Click(object sender, RoutedEventArgs e) {
        if (State.Document != null) {
            State.Document.IsReadOnly = !State.Document.IsReadOnly;
            State.RaiseDocumentChange();  // state doesn't get automatically updated for this one
        }
    }


    public async void mnuItemAdd_Click(object sender, RoutedEventArgs e) {
        if (mnuItemAdd.IsEnabled == false) { return; }

        var frm = new EntryWindow(State);
        await frm.ShowDialog(this);
    }

    public async void mnuItemEdit_Click(object sender, RoutedEventArgs e) {
        if (mnuItemEdit.IsEnabled == false) { return; }
        if (mnuItemEdit.IsVisible == false) {  // all "view" actions should call the view handler
            mnuItemView_Click(sender, e);
            return;
        }

        if (lsbEntries.SelectedItem is ListBoxItem { Tag: Entry selectedEntry }) {
            var frm = new EntryWindow(State, selectedEntry);
            await frm.ShowDialog(this);
        }
    }

    public async void mnuItemView_Click(object sender, RoutedEventArgs e) {
        if (mnuItemView.IsEnabled == false) { return; }

        if (lsbEntries.SelectedItem is ListBoxItem { Tag: Entry selectedEntry }) {
            var frm = new EntryWindow(State, selectedEntry, readOnly: true);
            await frm.ShowDialog(this);
        }
    }

    public void mnuItemRemove_Click(object sender, RoutedEventArgs e) {
        if (mnuItemRemove.IsEnabled == false) { return; }

        if (lsbEntries.SelectedItem is ListBoxItem { Tag: Entry selectedEntry }) {
            if (MessageBox.ShowQuestionDialog(this, "Remove entry", $"Do you really want to remove entry '{selectedEntry.Title}'?", "Yes", "No") == 0) {
                lsbEntries.Items.RemoveAt(lsbEntries.SelectedIndex);
            }
        }
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


    public void mnuEntry_Opening(object? sender, CancelEventArgs e) {
        var canPaste = Entry.TryImportFromJson(PTClipboard.GetText(), out _);
        var canCopy = (lsbEntries.SelectedItem as ListBoxItem)?.Tag is Entry;
        mnuEntryCut.IsEnabled = canCopy;
        mnuEntryCopy.IsEnabled = canCopy;
        mnuEntryPaste.IsEnabled = canPaste;
    }

    public void mnuEntryCut_Click(object? sender, RoutedEventArgs e) {
        if ((lsbEntries.SelectedItem as ListBoxItem)?.Tag is not Entry entry) { return; }
        var json = entry.ExportToJson();
        PTClipboard.SetText(json);
        lsbEntries.Items.Remove(lsbEntries.SelectedItem);
        State?.Document?.Entries.Remove(entry);
    }

    public void mnuEntryCopy_Click(object? sender, RoutedEventArgs e) {
        if ((lsbEntries.SelectedItem as ListBoxItem)?.Tag is not Entry entry) { return; }
        var json = entry.ExportToJson();
        PTClipboard.SetText(json);
    }

    public void mnuEntryPaste_Click(object? sender, RoutedEventArgs e) {
        var text = PTClipboard.GetText();
        if (Entry.TryImportFromJson(text, out var entry)) {
            State?.Document?.Entries.Add(entry);
            ReplenishEntries(entry);
        }
    }

    #endregion Menu

    #region Events

    public void lsbEntries_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        var selectedEntry = (lsbEntries.SelectedItem as ListBoxItem)?.Tag as Entry;
        mnuItemView.IsEnabled = (selectedEntry != null);
        mnuItemEdit.IsEnabled = (selectedEntry != null);
        mnuItemRemove.IsEnabled = (selectedEntry != null);
    }

    public void lsbEntries_DoubleTapped(object sender, TappedEventArgs e) {
        mnuItemEdit_Click(sender, e);
    }

    #endregion

    private void ReplenishDocument() {
        var file = State.File;
        if (file != null) {
            Title = file.Name;
            Config.Recent.Files.Add(file);
        } else {
            Title = "Bimil";
        }

        var hasDocument = (State.Document != null);
        var isReadWrite = !(State.Document?.IsReadOnly ?? true);
        mnuFileSave.IsEnabled = hasDocument;
        mnuFileProperties.IsEnabled = hasDocument;
        mnuFilePropertiesPassword.IsEnabled = isReadWrite;
        mnuItemAdd.IsEnabled = hasDocument && isReadWrite;
        mnuItemView.IsEnabled = false;  // will enable when item is selected
        mnuItemEdit.IsEnabled = false;
        mnuItemRemove.IsEnabled = false;
        mnuFind.IsEnabled = hasDocument;

        txtFilter.IsVisible = hasDocument;
        cmbGroups.IsVisible = hasDocument;
        lsbEntries.IsVisible = hasDocument;

        ThemeImageResources.Update();  // enable/disable buttons

        txtFilter.Text = "";
        cmbGroups.SelectedItem = null;

        Replenishment.FillGroups(State, cmbGroups, includeAnyGroup: true);
        ReplenishEntries();
    }

    private void ReplenishEntries(Entry? selectedEntry = null) {
        if (selectedEntry == null) {
            if (lsbEntries.SelectedItem is ListBoxItem { Tag: Entry entry }) {
                selectedEntry = entry;
            }
        }

        lsbEntries.Items.Clear();
        if (State.Document != null) {
            var filter = txtFilter.Text ?? "";
            var group = (cmbGroups.SelectedItem as ComboBoxItem)?.Tag as string;
            var items = State.GetEntries(filter, group);
            foreach (var item in items) {
                var titleBlock = new TextBlock() {
                    Text = item.Title,
                    FontSize = FontSize * 1.25,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                var groupBlock = new TextBlock() {
                    Text = !string.IsNullOrEmpty(item.Group) ? item.Group : "(no group)",
                    TextAlignment = TextAlignment.Right,
                    FontSize = FontSize * 0.75,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                var dock = new DockPanel();
                dock.Children.Add(titleBlock);
                dock.Children.Add(groupBlock);
                DockPanel.SetDock(titleBlock, Dock.Left);
                DockPanel.SetDock(groupBlock, Dock.Right);

                var newLBItem = new ListBoxItem { Content = dock, Tag = item };
                lsbEntries.Items.Add(newLBItem);
                if (item == selectedEntry) {
                    lsbEntries.SelectedItem = newLBItem;
                }
            }
        }
        if ((lsbEntries.SelectedIndex < 0) && (lsbEntries.Items.Count > 0)) {
            lsbEntries.SelectedIndex = 0;
        }
    }

}
