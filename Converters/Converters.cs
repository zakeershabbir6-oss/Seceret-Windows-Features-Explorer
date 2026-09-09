using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using HiddenFeaturesExplorer.Models;
using HiddenFeaturesExplorer.Services;

namespace HiddenFeaturesExplorer.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = value is bool b && b;
        if (parameter is string s && s == "Invert")
            flag = !flag;
        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is Visibility v && v == Visibility.Visible;
}

public class DifficultyToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            DifficultyLevel.Beginner => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")),
            DifficultyLevel.Intermediate => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800")),
            DifficultyLevel.Advanced => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5722")),
            DifficultyLevel.Expert => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F")),
            _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E9E9E")),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class DifficultyToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            DifficultyLevel.Beginner => "🟢 Beginner",
            DifficultyLevel.Intermediate => "🟡 Intermediate",
            DifficultyLevel.Advanced => "🟠 Advanced",
            DifficultyLevel.Expert => "🔴 Expert",
            _ => "Unknown",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class SafetyToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            SafetyLevel.Safe => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")),
            SafetyLevel.Informational => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3")),
            SafetyLevel.ChangesSettings => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800")),
            SafetyLevel.Administrative => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5722")),
            SafetyLevel.SystemImpact => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F")),
            _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E9E9E")),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class SafetyToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            SafetyLevel.Safe => "✅ Safe",
            SafetyLevel.Informational => "ℹ️ Informational",
            SafetyLevel.ChangesSettings => "⚠️ Changes Settings",
            SafetyLevel.Administrative => "🔐 Requires Admin",
            SafetyLevel.SystemImpact => "🔴 System Impact",
            _ => "Unknown",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class CategoryToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FeatureCategory cat)
        {
            var categories = FeatureDatabase.GetCategories();
            var info = categories.FirstOrDefault(c => c.Category == cat);
            if (info != null)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.Color));
        }
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class ScoreToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double score)
            return (score / 10.0) * 100;
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class BoolToFavoriteIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && b ? "\u2605" : "\u2606";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class BoolToFavoriteColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && b
            ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700"))
            : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class NavToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string nav && parameter is string expected)
        {
            return nav == expected
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4")) { Opacity = 0.15 }
                : new SolidColorBrush(Colors.Transparent);
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class NavToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string nav && parameter is string expected)
        {
            return nav == expected
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
        }
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
