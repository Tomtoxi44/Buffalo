using SQLite;

namespace BuffaloApp.Models;

/// <summary>
/// Représente un joueur de Buffalo
/// </summary>
public class Player
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Identifiant unique Bluetooth du joueur
    /// </summary>
    public string BluetoothId { get; set; } = string.Empty;

    /// <summary>
    /// Pseudo du joueur
    /// </summary>
    public string Pseudo { get; set; } = string.Empty;

    /// <summary>
    /// Indique si c'est le profil de l'utilisateur local
    /// </summary>
    public bool IsLocalPlayer { get; set; }

    /// <summary>
    /// Nombre de Buffalo donnés (réussis)
    /// </summary>
    public int BuffaloGiven { get; set; }

    /// <summary>
    /// Nombre de Buffalo reçus (acceptés)
    /// </summary>
    public int BuffaloReceived { get; set; }

    /// <summary>
    /// Main dominante (true = droitier, false = gaucher)
    /// </summary>
    public bool IsRightHanded { get; set; } = true;

    /// <summary>
    /// Date de première rencontre avec ce joueur
    /// </summary>
    public DateTime FirstSeen { get; set; } = DateTime.Now;

    /// <summary>
    /// Dernière fois que ce joueur a été vu
    /// </summary>
    public DateTime LastSeen { get; set; } = DateTime.Now;

    /// <summary>
    /// Indique si le mode Buffalo est actif pour ce joueur
    /// </summary>
    public bool IsPlaying { get; set; }
}
