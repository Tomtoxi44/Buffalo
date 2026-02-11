using BuffaloApp.Models;

namespace BuffaloApp.Services;

/// <summary>
/// Interface pour le service Bluetooth
/// </summary>
public interface IBluetoothService
{
    /// <summary>
    /// Événement déclenché quand un joueur est détecté
    /// </summary>
    event EventHandler<NearbyPlayer>? PlayerDetected;

    /// <summary>
    /// Événement déclenché quand un joueur n'est plus à proximité
    /// </summary>
    event EventHandler<Player>? PlayerLost;

    /// <summary>
    /// Événement déclenché quand on reçoit un Buffalo
    /// </summary>
    event EventHandler<BuffaloEvent>? BuffaloReceived;

    /// <summary>
    /// Indique si le Bluetooth est disponible
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Indique si le scan est en cours
    /// </summary>
    bool IsScanning { get; }

    /// <summary>
    /// Indique si l'émission est active
    /// </summary>
    bool IsBroadcasting { get; }

    /// <summary>
    /// Liste des joueurs actuellement à proximité
    /// </summary>
    IReadOnlyList<NearbyPlayer> NearbyPlayers { get; }

    /// <summary>
    /// Démarre le scan des joueurs à proximité
    /// </summary>
    Task StartScanningAsync();

    /// <summary>
    /// Arrête le scan
    /// </summary>
    Task StopScanningAsync();

    /// <summary>
    /// Démarre l'émission du signal Buffalo
    /// </summary>
    Task StartBroadcastingAsync(Player localPlayer);

    /// <summary>
    /// Arrête l'émission
    /// </summary>
    Task StopBroadcastingAsync();

    /// <summary>
    /// Envoie un Buffalo à un joueur
    /// </summary>
    Task SendBuffaloAsync(Player target);

    /// <summary>
    /// Vérifie et demande les permissions Bluetooth
    /// </summary>
    Task<bool> RequestPermissionsAsync();
}
