using System;
using System.Collections.Generic;

namespace HiddenFeaturesExplorer.Models;

public enum FeatureCategory
{
    Productivity,
    FileManagement,
    SystemTools,
    PrivacySecurity,
    Performance,
    Customization,
    KeyboardShortcuts,
    PowerUserTools,
    Accessibility,
    Networking,
    Troubleshooting,
    DeveloperFeatures
}

public enum DifficultyLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}

public enum SafetyLevel
{
    Safe,
    Informational,
    ChangesSettings,
    Administrative,
    SystemImpact
}

public enum WindowsVersionRequirement
{
    Any,
    Windows10,
    Windows11,
    Windows10Version1903,
    Windows10Version2004,
    Windows11Version21H2,
    Windows11Version22H2,
    Windows11Version23H2
}

public class FeatureTag
{
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class WindowsFeature
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string SimpleExplanation { get; set; } = string.Empty;
    public string DetailedExplanation { get; set; } = string.Empty;
    public string WhatItDoes { get; set; } = string.Empty;
    public string WhyUseful { get; set; } = string.Empty;
    public string HowToAccess { get; set; } = string.Empty;
    public string? KeyboardShortcut { get; set; }
    public string? CommandLine { get; set; }
    public string? NavigationPath { get; set; }
    public string? SettingsUri { get; set; }
    public FeatureCategory Category { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public SafetyLevel Safety { get; set; }
    public WindowsVersionRequirement MinVersion { get; set; } = WindowsVersionRequirement.Any;
    public bool RequiresAdmin { get; set; }
    public bool CanLaunch { get; set; }
    public string? LaunchCommand { get; set; }
    public List<FeatureTag> Tags { get; set; } = new();
    public List<string> SearchIntents { get; set; } = new();
    public List<string> RelatedFeatureIds { get; set; } = new();
    public double UsefulnessScore { get; set; } = 5.0;
    public double DiscoveryDifficulty { get; set; } = 5.0;
    public DateTime? LastUsed { get; set; }
    public int UseCount { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsRecentlyDiscovered { get; set; }
    public string? WarningMessage { get; set; }
    public string? IconGlyph { get; set; }
    public string AccentColor { get; set; } = "#0078D4";
}

public class CategoryInfo
{
    public FeatureCategory Category { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconGlyph { get; set; } = string.Empty;
    public string Color { get; set; } = "#0078D4";
    public int FeatureCount { get; set; }
}
