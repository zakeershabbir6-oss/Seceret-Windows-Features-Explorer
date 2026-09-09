using System;
using System.Windows;
using System.Windows.Input;
using HiddenFeaturesExplorer.Models;
using HiddenFeaturesExplorer.ViewModels;

namespace HiddenFeaturesExplorer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            ShowDefaultView();
        }
    }

    private void ShowDefaultView()
    {
        SearchResultsHeader.Visibility = Visibility.Collapsed;
        FavoritesView.Visibility = Visibility.Collapsed;
        RecentView.Visibility = Visibility.Collapsed;
        SettingsView.Visibility = Visibility.Collapsed;
    }

    private MainViewModel? GetVm() => DataContext as MainViewModel;

    // Navigation handlers
    private void NavHome_Click(object sender, MouseButtonEventArgs e)
    {
        GetVm()?.NavigateCommand.Execute("home");
        ShowDefaultView();
        SearchResultsHeader.Visibility = Visibility.Collapsed;
    }

    private void NavFav_Click(object sender, MouseButtonEventArgs e)
    {
        GetVm()?.NavigateCommand.Execute("favorites");
        HideAllViews();
        FavoritesView.Visibility = Visibility.Visible;
    }

    private void NavRecent_Click(object sender, MouseButtonEventArgs e)
    {
        GetVm()?.NavigateCommand.Execute("recent");
        HideAllViews();
        RecentView.Visibility = Visibility.Visible;
    }

    private void NavSettings_Click(object sender, MouseButtonEventArgs e)
    {
        GetVm()?.NavigateCommand.Execute("settings");
        HideAllViews();
        SettingsView.Visibility = Visibility.Visible;
    }

    private void HideAllViews()
    {
        FavoritesView.Visibility = Visibility.Collapsed;
        RecentView.Visibility = Visibility.Collapsed;
        SettingsView.Visibility = Visibility.Collapsed;
    }

    // Category click
    private void Category_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is System.Windows.Controls.Border border && border.Tag is FeatureCategory cat)
        {
            GetVm()?.SelectCategoryCommand.Execute(cat);
            HideAllViews();
            SearchResultsHeader.Visibility = Visibility.Visible;
            SearchResultsHeader.Text = $"📂 {cat} Features";
        }
    }

    // Feature card click
    private void FeatureCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is System.Windows.Controls.Border border && border.Tag is WindowsFeature feature)
        {
            GetVm()?.SelectFeatureCommand.Execute(feature);
        }
    }

    // Suggestion click
    private void Suggestion_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is System.Windows.Controls.Border border && border.Tag is string suggestion)
        {
            GetVm()?.SearchCommand.Execute(suggestion);
            SearchBox.Text = suggestion;
        }
    }
}
