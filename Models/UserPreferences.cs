using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HiddenFeaturesExplorer.Models;

public class UserPreferences
{
    public bool IsDarkTheme { get; set; } = true;
    public bool ExplainSimplyMode { get; set; } = true;
    public DifficultyLevel PreferredDifficulty { get; set; } = DifficultyLevel.Intermediate;
    public List<FeatureCategory> InterestedCategories { get; set; } = new();
    public List<string> FavoriteFeatureIds { get; set; } = new();
    public List<string> RecentlyViewedFeatureIds { get; set; } = new();
    public string? WindowsEdition { get; set; }
    public string? WindowsBuild { get; set; }

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "HiddenFeaturesExplorer",
        "preferences.json");

    public void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(SettingsPath, json);
        }
        catch { }
    }

    public static UserPreferences Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<UserPreferences>(json) ?? new UserPreferences();
            }
        }
        catch { }
        return new UserPreferences();
    }
}
