using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using HiddenFeaturesExplorer.Models;

namespace HiddenFeaturesExplorer.Services;

public static class LaunchService
{
    public static bool TryLaunch(WindowsFeature feature)
    {
        if (!feature.CanLaunch || string.IsNullOrEmpty(feature.LaunchCommand))
            return false;

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = feature.LaunchCommand,
                UseShellExecute = true,
                Verb = feature.RequiresAdmin ? "runas" : ""
            };
            Process.Start(psi);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool TryOpenSettings(string? settingsUri)
    {
        if (string.IsNullOrEmpty(settingsUri))
            return false;

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = settingsUri,
                UseShellExecute = true
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void CreateGodModeFolder()
    {
        try
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var godModePath = Path.Combine(desktopPath, "GodMode.{ED7BA470-8E54-465E-825C-99712043E01C}");
            Directory.CreateDirectory(godModePath);
            Process.Start("explorer.exe", godModePath);
        }
        catch { }
    }
}
