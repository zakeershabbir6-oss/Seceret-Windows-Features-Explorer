using System;
using System.Collections.Generic;
using System.Linq;
using HiddenFeaturesExplorer.Models;

namespace HiddenFeaturesExplorer.Services;

public class SearchService
{
    private readonly List<WindowsFeature> _features;
    private readonly Dictionary<string, List<string>> _intentMap;

    public SearchService()
    {
        _features = FeatureDatabase.GetAllFeatures();
        _intentMap = BuildIntentMap();
    }

    private Dictionary<string, List<string>> BuildIntentMap()
    {
        return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["make windows faster"] = new() { "startup-apps", "power-options", "disk-cleanup", "fast-startup", "power-toys" },
            ["find large files"] = new() { "disk-cleanup", "resource-monitor" },
            ["stop apps from starting automatically"] = new() { "startup-apps" },
            ["see what is using my internet"] = new() { "resource-monitor", "data-usage" },
            ["quickly rename many files"] = new() { "power-rename" },
            ["find hidden settings"] = new() { "god-mode", "group-policy", "registry-editor" },
            ["fix common windows problems"] = new() { "troubleshoot", "system-file-checker", "safe-mode" },
            ["customize windows"] = new() { "personalization", "dark-mode", "accent-colors", "transparency-effects", "power-toys" },
            ["free up storage"] = new() { "disk-cleanup", "compress-folders", "file-history" },
            ["protect my computer"] = new() { "windows-defender", "bitlocker", "ransomware-protection", "find-my-device" },
            ["manage my passwords"] = new() { "credential-manager" },
            ["speed up startup"] = new() { "startup-apps", "fast-startup" },
            ["organize my desktop"] = new() { "virtual-desktops", "snap-layouts" },
            ["take screenshots"] = new() { "dictation-shortcut" },
            ["backup my files"] = new() { "file-history", "recovery-drive" },
            ["run linux on windows"] = new() { "wsl" },
            ["control my pc with voice"] = new() { "voice-typing", "voice-access" },
            ["see system logs"] = new() { "event-viewer" },
            ["test my memory"] = new() { "memory-diagnostics" },
            ["diagnose performance"] = new() { "resource-monitor", "performance-monitor", "task-manager" },
            ["manage network"] = new() { "network-status", "mobile-hotspot", "data-usage" },
            ["access all settings"] = new() { "god-mode", "control-panel" },
            ["customize terminal"] = new() { "windows-terminal", "windows-terminal-profiles" },
            ["run virtual machines"] = new() { "hyper-v" },
            ["run android apps"] = new() { "windows-subsystem-android" },
            ["emoji keyboard"] = new() { "emoji-shortcut" },
            ["dark mode"] = new() { "dark-mode" },
            ["accessibility tools"] = new() { "magnifier", "narrator", "color-filter", "voice-access", "eye-control" },
        };
    }

    public List<WindowsFeature> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return _features.OrderByDescending(f => f.UsefulnessScore).ToList();

        var queryLower = query.ToLower().Trim();
        var scored = new List<(WindowsFeature Feature, double Score)>();

        // Check intent matches first
        foreach (var intent in _intentMap)
        {
            if (intent.Key.Contains(queryLower) || queryLower.Contains(intent.Key))
            {
                foreach (var featureId in intent.Value)
                {
                    var feature = _features.FirstOrDefault(f => f.Id == featureId);
                    if (feature != null)
                    {
                        scored.Add((feature, 100.0));
                    }
                }
            }
        }

        foreach (var feature in _features)
        {
            double score = 0;

            // Name match
            if (feature.Name.ToLower().Contains(queryLower))
                score += 50;

            // Simple explanation match
            if (feature.SimpleExplanation.ToLower().Contains(queryLower))
                score += 30;

            // Detailed explanation match
            if (feature.DetailedExplanation.ToLower().Contains(queryLower))
                score += 20;

            // Search intents match
            foreach (var intent in feature.SearchIntents)
            {
                if (intent.ToLower().Contains(queryLower))
                    score += 40;
            }

            // Category match
            if (feature.Category.ToString().ToLower().Contains(queryLower))
                score += 15;

            // Tags match
            foreach (var tag in feature.Tags)
            {
                if (tag.Label.ToLower().Contains(queryLower))
                    score += 25;
            }

            // What it does match
            if (feature.WhatItDoes.ToLower().Contains(queryLower))
                score += 25;

            // Why useful match
            if (feature.WhyUseful.ToLower().Contains(queryLower))
                score += 20;

            if (score > 0)
            {
                scored.Add((feature, score));
            }
        }

        return scored
            .GroupBy(x => x.Feature.Id)
            .Select(g => new { Feature = g.Key, Score = g.Max(x => x.Score) })
            .OrderByDescending(x => x.Score)
            .Select(x => _features.First(f => f.Id == x.Feature))
            .ToList();
    }

    public List<WindowsFeature> GetByCategory(FeatureCategory category)
    {
        return _features
            .Where(f => f.Category == category)
            .OrderByDescending(f => f.UsefulnessScore)
            .ToList();
    }

    public List<WindowsFeature> GetFeatured()
    {
        return _features
            .OrderByDescending(f => f.UsefulnessScore)
            .Take(8)
            .ToList();
    }

    public List<WindowsFeature> GetRecommended(WindowsSystemInfo systemInfo, UserPreferences preferences)
    {
        return _features
            .Where(f => systemInfo.IsFeatureAvailable(f))
            .Where(f => preferences.InterestedCategories.Count == 0 || preferences.InterestedCategories.Contains(f.Category))
            .OrderByDescending(f => f.UsefulnessScore)
            .ThenByDescending(f => f.DiscoveryDifficulty)
            .Take(12)
            .ToList();
    }

    public List<string> GetSuggestions()
    {
        return new List<string>
        {
            "Make Windows faster",
            "Find hidden settings",
            "Organize my desktop",
            "Protect my computer",
            "Customize Windows",
            "Fix common problems",
            "Free up storage",
            "Speed up startup",
            "Take screenshots",
            "Manage my passwords",
            "See what is using my internet",
            "Run linux on windows",
            "Emoji keyboard",
            "Dark mode",
        };
    }
}
