using System.Globalization;

namespace BuffaloApp.Converters;

/// <summary>
/// Convertit un int en bool (true si > 0)
/// </summary>
public class IntToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue)
            return intValue > 0;
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convertit IsRightHanded en emoji de main
/// </summary>
public class BoolToHandEmojiConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isRightHanded)
            return isRightHanded ? "🤚" : "🖐️";
        return "🤚";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convertit IsPlaying en texte de recherche
/// </summary>
public class BoolToSearchTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isPlaying)
            return isPlaying 
                ? "Recherche de joueurs Buffalo...\nAssurez-vous d'être proche d'autres joueurs" 
                : "Activez le mode Buffalo\npour détecter les joueurs";
        return "Activez le mode Buffalo";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Convertit le rang et IsCurrentUser en couleur de fond
/// </summary>
public class LeaderboardBackgroundConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 2)
            return Color.FromArgb("#0f3460");

        var rank = values[0] as int? ?? 0;
        var isCurrentUser = values[1] as bool? ?? false;

        if (isCurrentUser)
            return Color.FromArgb("#1e5128"); // Vert foncé pour l'utilisateur actuel

        return rank switch
        {
            1 => Color.FromArgb("#5c4d1a"), // Or
            2 => Color.FromArgb("#4a4a4a"), // Argent
            3 => Color.FromArgb("#4a3728"), // Bronze
            _ => Color.FromArgb("#0f3460")  // Bleu par défaut
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
