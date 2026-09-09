using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HiddenFeaturesExplorer.Models;
using HiddenFeaturesExplorer.Services;

namespace HiddenFeaturesExplorer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly SearchService _searchService;
    private readonly WindowsSystemInfo _systemInfo;
    private readonly UserPreferences _preferences;

    private string _searchQuery = string.Empty;
    private WindowsFeature? _selectedFeature;
    private FeatureCategory? _selectedCategory;
    private string _selectedNavIndex = "home";
    private bool _isDarkTheme = true;
    private bool _explainSimply = true;
    private bool _showDetailPanel;
    private string _statusMessage = string.Empty;
    private string _searchSuggestion = string.Empty;

    public ObservableCollection<WindowsFeature> Features { get; } = new();
    public ObservableCollection<WindowsFeature> FeaturedFeatures { get; } = new();
    public ObservableCollection<WindowsFeature> RecommendedFeatures { get; } = new();
    public ObservableCollection<WindowsFeature> FavoriteFeatures { get; } = new();
    public ObservableCollection<WindowsFeature> RecentlyDiscoveredFeatures { get; } = new();
    public ObservableCollection<WindowsFeature> SearchResults { get; } = new();
    public ObservableCollection<CategoryInfo> Categories { get; } = new();
    public ObservableCollection<string> SearchSuggestions { get; } = new();
    public ObservableCollection<string> RecentSearches { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                PerformSearch();
            }
        }
    }

    public WindowsFeature? SelectedFeature
    {
        get => _selectedFeature;
        set
        {
            if (SetProperty(ref _selectedFeature, value))
            {
                ShowDetailPanel = value != null;
            }
        }
    }

    public FeatureCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                FilterByCategory();
            }
        }
    }

    public string SelectedNavIndex
    {
        get => _selectedNavIndex;
        set
        {
            if (SetProperty(ref _selectedNavIndex, value))
            {
                NavigationChanged();
            }
        }
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                _preferences.IsDarkTheme = value;
                _preferences.Save();
                ApplyTheme();
            }
        }
    }

    public bool ExplainSimply
    {
        get => _explainSimply;
        set
        {
            if (SetProperty(ref _explainSimply, value))
            {
                _preferences.ExplainSimplyMode = value;
                _preferences.Save();
            }
        }
    }

    public bool ShowDetailPanel
    {
        get => _showDetailPanel;
        set => SetProperty(ref _showDetailPanel, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string SearchSuggestion
    {
        get => _searchSuggestion;
        set => SetProperty(ref _searchSuggestion, value);
    }

    public string SystemInfoText => $"{_systemInfo.ProductName} (Build {_systemInfo.BuildNumber})";
    public int TotalFeatures => Features.Count;
    public int TotalFavorites => FavoriteFeatures.Count;

    public ICommand SearchCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand LaunchFeatureCommand { get; }
    public ICommand SelectFeatureCommand { get; }
    public ICommand CloseDetailCommand { get; }
    public ICommand ToggleThemeCommand { get; }
    public ICommand UseSuggestionCommand { get; }
    public ICommand NavigateCommand { get; }
    public ICommand SelectCategoryCommand { get; }

    public MainViewModel()
    {
        _searchService = new SearchService();
        _systemInfo = WindowsSystemInfo.Detect();
        _preferences = UserPreferences.Load();

        SearchCommand = new RelayCommand(ExecuteSearch);
        ToggleFavoriteCommand = new RelayCommand(ExecuteToggleFavorite);
        LaunchFeatureCommand = new RelayCommand(ExecuteLaunchFeature);
        SelectFeatureCommand = new RelayCommand(ExecuteSelectFeature);
        CloseDetailCommand = new RelayCommand(() => { SelectedFeature = null; });
        ToggleThemeCommand = new RelayCommand(() => IsDarkTheme = !IsDarkTheme);
        UseSuggestionCommand = new RelayCommand(ExecuteUseSuggestion);
        NavigateCommand = new RelayCommand(ExecuteNavigate);
        SelectCategoryCommand = new RelayCommand(ExecuteSelectCategory);

        Initialize();
    }

    private void Initialize()
    {
        _isDarkTheme = _preferences.IsDarkTheme;
        _explainSimply = _preferences.ExplainSimplyMode;

        var allFeatures = _searchService.Search("");
        foreach (var feature in allFeatures)
        {
            if (_systemInfo.IsFeatureAvailable(feature))
            {
                Features.Add(feature);
            }
        }

        foreach (var category in FeatureDatabase.GetCategories())
        {
            category.FeatureCount = Features.Count(f => f.Category == category.Category);
            Categories.Add(category);
        }

        RefreshFeatured();
        RefreshRecommended();
        RefreshFavorites();
        RefreshRecentlyDiscovered();

        foreach (var suggestion in _searchService.GetSuggestions().Take(6))
        {
            SearchSuggestions.Add(suggestion);
        }

        StatusMessage = $"Discovered {Features.Count} hidden features on {_systemInfo.ProductName}";
        SearchSuggestion = SearchSuggestions.FirstOrDefault() ?? "What do you want to do?";
    }

    private void RefreshFeatured()
    {
        FeaturedFeatures.Clear();
        foreach (var feature in _searchService.GetFeatured().Where(f => _systemInfo.IsFeatureAvailable(f)))
        {
            FeaturedFeatures.Add(feature);
        }
    }

    private void RefreshRecommended()
    {
        RecommendedFeatures.Clear();
        foreach (var feature in _searchService.GetRecommended(_systemInfo, _preferences))
        {
            if (!RecommendedFeatures.Any(f => f.Id == feature.Id))
                RecommendedFeatures.Add(feature);
        }
    }

    private void RefreshFavorites()
    {
        FavoriteFeatures.Clear();
        foreach (var id in _preferences.FavoriteFeatureIds)
        {
            var feature = Features.FirstOrDefault(f => f.Id == id);
            if (feature != null)
                FavoriteFeatures.Add(feature);
        }
    }

    private void RefreshRecentlyDiscovered()
    {
        RecentlyDiscoveredFeatures.Clear();
        foreach (var id in _preferences.RecentlyViewedFeatureIds.Take(10))
        {
            var feature = Features.FirstOrDefault(f => f.Id == id);
            if (feature != null)
                RecentlyDiscoveredFeatures.Add(feature);
        }
    }

    private void PerformSearch()
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(_searchQuery))
        {
            StatusMessage = $"Showing all {Features.Count} features";
            return;
        }

        var results = _searchService.Search(_searchQuery)
            .Where(f => _systemInfo.IsFeatureAvailable(f))
            .ToList();

        foreach (var feature in results)
        {
            SearchResults.Add(feature);
        }

        StatusMessage = $"Found {results.Count} features matching \"{_searchQuery}\"";

        if (!string.IsNullOrWhiteSpace(_searchQuery) && !RecentSearches.Contains(_searchQuery))
        {
            RecentSearches.Insert(0, _searchQuery);
            if (RecentSearches.Count > 5)
                RecentSearches.RemoveAt(5);
        }
    }

    private void FilterByCategory()
    {
        SearchResults.Clear();

        if (_selectedCategory == null)
        {
            StatusMessage = $"Showing all {Features.Count} features";
            return;
        }

        var results = _searchService.GetByCategory(_selectedCategory.Value)
            .Where(f => _systemInfo.IsFeatureAvailable(f))
            .ToList();

        foreach (var feature in results)
        {
            SearchResults.Add(feature);
        }

        var catInfo = Categories.FirstOrDefault(c => c.Category == _selectedCategory);
        StatusMessage = $"Showing {results.Count} features in {catInfo?.DisplayName ?? _selectedCategory.ToString()}";
    }

    private void NavigationChanged()
    {
        SearchResults.Clear();
        SelectedCategory = null;
        SearchQuery = string.Empty;

        switch (_selectedNavIndex)
        {
            case "home":
                StatusMessage = $"Discovered {Features.Count} hidden features";
                break;
            case "favorites":
                foreach (var f in FavoriteFeatures) SearchResults.Add(f);
                StatusMessage = $"{FavoriteFeatures.Count} favorite features";
                break;
            case "recent":
                foreach (var f in RecentlyDiscoveredFeatures) SearchResults.Add(f);
                StatusMessage = $"{RecentlyDiscoveredFeatures.Count} recently discovered features";
                break;
            case "settings":
                StatusMessage = "Settings";
                break;
        }
    }

    private void ExecuteSearch()
    {
        PerformSearch();
    }

    private void ExecuteToggleFavorite(object? parameter)
    {
        if (parameter is WindowsFeature feature)
        {
            feature.IsFavorite = !feature.IsFavorite;

            if (feature.IsFavorite)
            {
                if (!_preferences.FavoriteFeatureIds.Contains(feature.Id))
                    _preferences.FavoriteFeatureIds.Add(feature.Id);
                StatusMessage = $"Added \"{feature.Name}\" to favorites";
            }
            else
            {
                _preferences.FavoriteFeatureIds.Remove(feature.Id);
                StatusMessage = $"Removed \"{feature.Name}\" from favorites";
            }

            _preferences.Save();
            RefreshFavorites();
        }
    }

    private void ExecuteLaunchFeature(object? parameter)
    {
        if (parameter is WindowsFeature feature)
        {
            if (feature.Safety == SafetyLevel.SystemImpact || feature.Safety == SafetyLevel.Administrative)
            {
                var result = MessageBox.Show(
                    $"⚠️ Warning: This feature may modify system settings.\n\n" +
                    $"Feature: {feature.Name}\n" +
                    $"Safety Level: {feature.Safety}\n\n" +
                    $"{feature.WarningMessage ?? "Are you sure you want to continue?"}\n\n" +
                    $"Click OK to proceed, Cancel to abort.",
                    "Safety Warning",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.OK)
                    return;
            }

            if (feature.Id == "god-mode")
            {
                LaunchService.CreateGodModeFolder();
                StatusMessage = "Created God Mode folder on Desktop!";
            }
            else if (!string.IsNullOrEmpty(feature.SettingsUri))
            {
                LaunchService.TryOpenSettings(feature.SettingsUri);
                StatusMessage = $"Opened settings for {feature.Name}";
            }
            else
            {
                LaunchService.TryLaunch(feature);
                StatusMessage = $"Launched {feature.Name}";
            }

            AddToRecentlyViewed(feature);
        }
    }

    private void ExecuteSelectFeature(object? parameter)
    {
        if (parameter is WindowsFeature feature)
        {
            SelectedFeature = feature;
            AddToRecentlyViewed(feature);
        }
    }

    private void ExecuteUseSuggestion(object? parameter)
    {
        if (parameter is string suggestion)
        {
            SearchQuery = suggestion;
        }
    }

    private void ExecuteSelectCategory(object? parameter)
    {
        if (parameter is FeatureCategory cat)
        {
            SelectedCategory = cat;
        }
    }

    private void ExecuteNavigate(object? parameter)
    {
        if (parameter is string nav)
        {
            SelectedNavIndex = nav;
        }
    }

    private void AddToRecentlyViewed(WindowsFeature feature)
    {
        _preferences.RecentlyViewedFeatureIds.Remove(feature.Id);
        _preferences.RecentlyViewedFeatureIds.Insert(0, feature.Id);
        if (_preferences.RecentlyViewedFeatureIds.Count > 20)
            _preferences.RecentlyViewedFeatureIds.RemoveRange(20, _preferences.RecentlyViewedFeatureIds.Count - 20);
        _preferences.Save();
        RefreshRecentlyDiscovered();
    }

    private void ApplyTheme()
    {
        var app = Application.Current;
        if (app == null) return;

        var dict = app.Resources;
        if (_isDarkTheme)
        {
            dict["WindowBackground"] = "#1E1E1E";
            dict["SurfaceBackground"] = "#252525";
            dict["CardBackground"] = "#2D2D2D";
            dict["PrimaryText"] = "#FFFFFF";
            dict["SecondaryText"] = "#B0B0B0";
            dict["BorderBrush"] = "#3D3D3D";
            dict["AccentColor"] = "#0078D4";
            dict["HoverBackground"] = "#353535";
            dict["SearchBackground"] = "#2D2D2D";
        }
        else
        {
            dict["WindowBackground"] = "#F3F3F3";
            dict["SurfaceBackground"] = "#FFFFFF";
            dict["CardBackground"] = "#FFFFFF";
            dict["PrimaryText"] = "#1A1A1A";
            dict["SecondaryText"] = "#666666";
            dict["BorderBrush"] = "#E0E0E0";
            dict["AccentColor"] = "#0078D4";
            dict["HoverBackground"] = "#F0F0F0";
            dict["SearchBackground"] = "#FFFFFF";
        }
    }
}
