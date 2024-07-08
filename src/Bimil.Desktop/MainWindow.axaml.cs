namespace Bimil.Desktop;

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;

internal partial class MainWindow : Window {

    public MainWindow() {
        InitializeComponent();
        ThemeToolbarImageResources.Setup(this);

        // ThemeVariant
        switch (Settings.Theme) {
            case Settings.ThemeVariant.Light: AppAvalonia.Current!.RequestedThemeVariant = ThemeVariant.Light; break;
            case Settings.ThemeVariant.Dark: AppAvalonia.Current!.RequestedThemeVariant = ThemeVariant.Dark; break;
            default: break;
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


    #region Menu

    public void OnMenuFileNewClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuFileOpenClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuFileSaveClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuFileSaveAsClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuFilePropertiesClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuFileChangePasswordClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuFileReadonlyClick(object sender, RoutedEventArgs e) {

    }


    public void OnMenuItemAddClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuItemEditClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuItemViewClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuItemRemoveClick(object sender, RoutedEventArgs e) {

    }


    public void OnMenuEditFindClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuEditPasswordGenerateClick(object sender, RoutedEventArgs e) {

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

    public void OnMenuAppOptionsClick(object sender, RoutedEventArgs e) {

    }

    public void OnMenuAppFeedbackClick(object sender, RoutedEventArgs e) {
        Medo.Avalonia.FeedbackBox.ShowDialog(this, new Uri("https://medo64.com/feedback/"));
    }

    public void OnMenuAppUpgradeClick(object sender, RoutedEventArgs e) {
        Medo.Avalonia.UpgradeBox.ShowDialog(this, new Uri("https://medo64.com/upgrade/"));
    }

    public void OnMenuAppAboutClick(object sender, RoutedEventArgs e) {
        Medo.Avalonia.AboutBox.ShowDialog(this, new Uri("https://medo64.com/bimil/"));
    }

    #endregion Menu

}
