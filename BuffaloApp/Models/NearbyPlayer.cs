namespace BuffaloApp.Models;

/// <summary>
/// Représente un joueur détecté à proximité via Bluetooth
/// </summary>
public class NearbyPlayer
{
    /// <summary>
    /// Joueur détecté
    /// </summary>
    public Player Player { get; set; } = new();

    /// <summary>
    /// Force du signal Bluetooth (pour estimer la distance)
    /// </summary>
    public int SignalStrength { get; set; }

    /// <summary>
    /// Distance estimée en mètres
    /// </summary>
    public double EstimatedDistance { get; set; }

    /// <summary>
    /// Dernière détection
    /// </summary>
    public DateTime LastDetected { get; set; } = DateTime.Now;

    /// <summary>
    /// Nombre de Buffalo en ardoise avec ce joueur (que tu lui dois)
    /// </summary>
    public int SlateOwed { get; set; }

    /// <summary>
    /// Nombre de Buffalo en ardoise avec ce joueur (qu'il te doit)
    /// </summary>
    public int SlateOwedToYou { get; set; }

    /// <summary>
    /// Indique si ce joueur est actuellement en mode Buffalo actif
    /// </summary>
    public bool IsActivelyPlaying { get; set; }

    /// <summary>
    /// Description de la distance pour l'affichage
    /// </summary>
    public string DistanceDescription => EstimatedDistance switch
    {
        < 1 => "Très proche",
        < 3 => "Proche",
        < 10 => "À proximité",
        _ => "Dans le coin"
    };
}
