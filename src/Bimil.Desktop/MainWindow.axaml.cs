using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Bimil.Desktop;

internal partial class MainWindow : Window {

    public MainWindow() {
        InitializeComponent();
        //Avalonia.Themes.Fluent.FluentTheme.ModeProperty =
    }

    #region Menu

    public void OnMenuFileNewClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuFileOpenClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuFileSaveClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuFileSaveAsClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuFilePropertiesClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuFileChangePasswordClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuFileReadonlyClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }


    public void OnMenuItemAddClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuItemEditClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuItemViewClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuItemRemoveClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }


    public void OnMenuEditFindClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuEditPasswordGenerateClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }


    public void OnMenuAppOptionsClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuAppFeedbackClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuAppUpgradeClick(object sender, RoutedEventArgs e) {
        Title = DateTime.Now.ToString();
    }

    public void OnMenuAppAboutClick(object sender, RoutedEventArgs e) {
        Medo.Avalonia.AboutBox.ShowDialog(this, new Uri("https://medo64.com/bimil/"));
    }

    #endregion Menu

}
