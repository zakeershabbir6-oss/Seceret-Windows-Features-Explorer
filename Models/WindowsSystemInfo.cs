using System;

namespace HiddenFeaturesExplorer.Models;

public class WindowsSystemInfo
{
    public string ProductName { get; set; } = string.Empty;
    public string Edition { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string BuildNumber { get; set; } = string.Empty;
    public string DisplayVersion { get; set; } = string.Empty;
    public bool IsWindows11 { get; set; }
    public bool IsWindows10 { get; set; }

    public WindowsVersionRequirement CurrentVersionRequirement
    {
        get
        {
            if (IsWindows11)
            {
                if (int.TryParse(BuildNumber, out int build))
                {
                    if (build >= 22631) return WindowsVersionRequirement.Windows11Version23H2;
                    if (build >= 22000) return WindowsVersionRequirement.Windows11Version21H2;
                }
                return WindowsVersionRequirement.Windows11;
            }
            if (IsWindows10)
            {
                if (int.TryParse(BuildNumber, out int build))
                {
                    if (build >= 19041) return WindowsVersionRequirement.Windows10Version2004;
                    if (build >= 18362) return WindowsVersionRequirement.Windows10Version1903;
                }
                return WindowsVersionRequirement.Windows10;
            }
            return WindowsVersionRequirement.Any;
        }
    }

    public bool IsFeatureAvailable(WindowsFeature feature)
    {
        if (feature.MinVersion == WindowsVersionRequirement.Any) return true;
        return feature.MinVersion <= CurrentVersionRequirement;
    }

    public static WindowsSystemInfo Detect()
    {
        var info = new WindowsSystemInfo();
        try
        {
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (key != null)
            {
                info.ProductName = key.GetValue("ProductName")?.ToString() ?? "Unknown";
                info.Edition = key.GetValue("EditionID")?.ToString() ?? "Unknown";
                info.Version = key.GetValue("ReleaseId")?.ToString() ?? "Unknown";
                info.BuildNumber = key.GetValue("CurrentBuildNumber")?.ToString() ?? "0";
                info.DisplayVersion = key.GetValue("DisplayVersion")?.ToString() ?? "";

                info.IsWindows11 = info.ProductName.Contains("Windows 11") || (int.TryParse(info.BuildNumber, out int b) && b >= 22000);
                info.IsWindows10 = info.ProductName.Contains("Windows 10") || (!info.IsWindows11 && int.TryParse(info.BuildNumber, out _));
            }
        }
        catch
        {
            info.ProductName = "Windows (Unknown Version)";
            info.BuildNumber = "0";
        }
        return info;
    }
}
