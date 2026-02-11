using BuffaloApp.Models;
using System.Collections.ObjectModel;

namespace BuffaloApp.Services;

/// <summary>
/// Implémentation du service Bluetooth pour la détection des joueurs Buffalo
/// Note: Cette implémentation utilise BLE (Bluetooth Low Energy) pour la détection à proximité
/// </summary>
public class BluetoothService : IBluetoothService
{
    private readonly List<NearbyPlayer> _nearbyPlayers = new();
    private bool _isScanning;
    private bool _isBroadcasting;
    private Player? _localPlayer;
    private CancellationTokenSource? _scanCancellationTokenSource;

    // UUID unique pour l'application Buffalo
    public static readonly Guid BuffaloServiceUuid = Guid.Parse("B0FFA100-CAFE-BEEF-DEAD-000000000001");
    public static readonly Guid BuffaloCharacteristicUuid = Guid.Parse("B0FFA100-CAFE-BEEF-DEAD-000000000002");

    public event EventHandler<NearbyPlayer>? PlayerDetected;
    public event EventHandler<Player>? PlayerLost;
    public event EventHandler<BuffaloEvent>? BuffaloReceived;

    public bool IsAvailable => true; // TODO: Vérifier la disponibilité réelle du Bluetooth

    public bool IsScanning => _isScanning;

    public bool IsBroadcasting => _isBroadcasting;

    public IReadOnlyList<NearbyPlayer> NearbyPlayers => _nearbyPlayers.AsReadOnly();

    public async Task<bool> RequestPermissionsAsync()
    {
        try
        {
            // Demande les permissions Bluetooth pour Android 12+
            var bluetoothStatus = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
            if (bluetoothStatus != PermissionStatus.Granted)
            {
                bluetoothStatus = await Permissions.RequestAsync<Permissions.Bluetooth>();
            }

            var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (locationStatus != PermissionStatus.Granted)
            {
                locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return bluetoothStatus == PermissionStatus.Granted && locationStatus == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors de la demande de permissions: {ex.Message}");
            return false;
        }
    }

    public async Task StartScanningAsync()
    {
        if (_isScanning)
            return;

        var hasPermissions = await RequestPermissionsAsync();
        if (!hasPermissions)
        {
            throw new InvalidOperationException("Les permissions Bluetooth sont requises");
        }

        _isScanning = true;
        _scanCancellationTokenSource = new CancellationTokenSource();

        // Démarre le scan en arrière-plan
        _ = ScanLoopAsync(_scanCancellationTokenSource.Token);
    }

    private async Task ScanLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _isScanning)
        {
            try
            {
                // TODO: Implémenter le vrai scan BLE avec Android.Bluetooth
                // Pour l'instant, on simule la détection

                // Nettoie les joueurs qui n'ont pas été vus depuis 30 secondes
                var timeout = DateTime.Now.AddSeconds(-30);
                var lostPlayers = _nearbyPlayers.Where(p => p.LastDetected < timeout).ToList();
                
                foreach (var lost in lostPlayers)
                {
                    _nearbyPlayers.Remove(lost);
                    PlayerLost?.Invoke(this, lost.Player);
                }

                await Task.Delay(1000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du scan: {ex.Message}");
            }
        }
    }

    public Task StopScanningAsync()
    {
        _isScanning = false;
        _scanCancellationTokenSource?.Cancel();
        _scanCancellationTokenSource?.Dispose();
        _scanCancellationTokenSource = null;
        return Task.CompletedTask;
    }

    public async Task StartBroadcastingAsync(Player localPlayer)
    {
        if (_isBroadcasting)
            return;

        var hasPermissions = await RequestPermissionsAsync();
        if (!hasPermissions)
        {
            throw new InvalidOperationException("Les permissions Bluetooth sont requises");
        }

        _localPlayer = localPlayer;
        _isBroadcasting = true;

        // TODO: Implémenter le vrai advertising BLE
        // - Créer un GATT server
        // - Ajouter le service Buffalo avec l'UUID
        // - Inclure les données du joueur dans les caractéristiques
    }

    public Task StopBroadcastingAsync()
    {
        _isBroadcasting = false;
        _localPlayer = null;
        return Task.CompletedTask;
    }

    public async Task SendBuffaloAsync(Player target)
    {
        if (_localPlayer == null)
            throw new InvalidOperationException("Le joueur local n'est pas défini");

        // TODO: Implémenter l'envoi réel via BLE
        // - Se connecter au GATT server du joueur cible
        // - Écrire dans la caractéristique Buffalo

        await Task.CompletedTask;
    }

    /// <summary>
    /// Méthode appelée quand un joueur est détecté via BLE
    /// </summary>
    internal void OnPlayerDiscovered(Player player, int rssi)
    {
        var existing = _nearbyPlayers.FirstOrDefault(p => p.Player.BluetoothId == player.BluetoothId);
        
        if (existing != null)
        {
            existing.LastDetected = DateTime.Now;
            existing.SignalStrength = rssi;
            existing.EstimatedDistance = CalculateDistance(rssi);
        }
        else
        {
            var nearbyPlayer = new NearbyPlayer
            {
                Player = player,
                SignalStrength = rssi,
                EstimatedDistance = CalculateDistance(rssi),
                LastDetected = DateTime.Now,
                IsActivelyPlaying = player.IsPlaying
            };
            
            _nearbyPlayers.Add(nearbyPlayer);
            PlayerDetected?.Invoke(this, nearbyPlayer);
        }
    }

    /// <summary>
    /// Méthode appelée quand un Buffalo est reçu via BLE
    /// </summary>
    internal void OnBuffaloReceived(BuffaloEvent buffaloEvent)
    {
        BuffaloReceived?.Invoke(this, buffaloEvent);
    }

    /// <summary>
    /// Calcule la distance approximative basée sur le RSSI
    /// </summary>
    private static double CalculateDistance(int rssi)
    {
        // Formule approximative basée sur le modèle de propagation
        // txPower est généralement -59 dBm à 1 mètre
        const int txPower = -59;
        
        if (rssi == 0)
            return -1;

        double ratio = rssi * 1.0 / txPower;
        
        if (ratio < 1.0)
            return Math.Pow(ratio, 10);
        else
            return 0.89976 * Math.Pow(ratio, 7.7095) + 0.111;
    }

    // Pour les tests/démo - simule la détection d'un joueur
    public void SimulatePlayerDetection(Player player, int rssi = -50)
    {
        OnPlayerDiscovered(player, rssi);
    }
}
