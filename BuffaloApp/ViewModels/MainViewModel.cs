using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BuffaloApp.Data;
using BuffaloApp.Models;
using BuffaloApp.Services;
using System.Collections.ObjectModel;

namespace BuffaloApp.ViewModels;

/// <summary>
/// ViewModel principal - Page d'accueil avec détection des joueurs
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly BuffaloDatabase _database;
    private readonly IBluetoothService _bluetoothService;
    private readonly BuffaloService _buffaloService;

    [ObservableProperty]
    private Player? _localPlayer;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private int _nearbyPlayersCount;

    [ObservableProperty]
    private int _buffaloGiven;

    [ObservableProperty]
    private int _buffaloReceived;

    [ObservableProperty]
    private int _slateOwed;

    [ObservableProperty]
    private int _slateOwedToYou;

    [ObservableProperty]
    private string _statusMessage = "Prêt à jouer !";

    public ObservableCollection<NearbyPlayer> NearbyPlayers { get; } = new();

    public MainViewModel(BuffaloDatabase database, IBluetoothService bluetoothService, BuffaloService buffaloService)
    {
        _database = database;
        _bluetoothService = bluetoothService;
        _buffaloService = buffaloService;

        _bluetoothService.PlayerDetected += OnPlayerDetected;
        _bluetoothService.PlayerLost += OnPlayerLost;
        _bluetoothService.BuffaloReceived += OnBuffaloReceived;
    }

    public async Task InitializeAsync()
    {
        LocalPlayer = await _database.GetLocalPlayerAsync();
        
        if (LocalPlayer == null)
        {
            // Premier lancement - créer le profil
            LocalPlayer = new Player
            {
                Pseudo = "Nouveau joueur",
                BluetoothId = Guid.NewGuid().ToString(),
                IsLocalPlayer = true,
                IsRightHanded = true
            };
            await _database.SavePlayerAsync(LocalPlayer);
        }

        IsPlaying = LocalPlayer.IsPlaying;
        await RefreshStatsAsync();
    }

    public async Task RefreshStatsAsync()
    {
        if (LocalPlayer == null) return;

        var stats = await _buffaloService.GetPlayerStatsAsync(LocalPlayer.Id);
        BuffaloGiven = stats.BuffaloGiven;
        BuffaloReceived = stats.BuffaloReceived;
        SlateOwed = stats.SlateOwed;
        SlateOwedToYou = stats.SlateOwedToYou;
    }

    [RelayCommand]
    private async Task TogglePlayingAsync()
    {
        if (LocalPlayer == null) return;

        IsPlaying = !IsPlaying;
        LocalPlayer.IsPlaying = IsPlaying;
        await _database.SavePlayerAsync(LocalPlayer);

        if (IsPlaying)
        {
            var success = await StartDetectionAsync();
            if (success)
            {
                StatusMessage = "Mode Buffalo ACTIVÉ !";
            }
            else
            {
                // Revert the toggle if activation failed
                IsPlaying = false;
                LocalPlayer.IsPlaying = false;
                await _database.SavePlayerAsync(LocalPlayer);
            }
        }
        else
        {
            await StopDetectionAsync();
            StatusMessage = "Mode Buffalo désactivé";
        }
    }

    [RelayCommand]
    private async Task<bool> StartDetectionAsync()
    {
        try
        {
            // Ask user for connection method preference
            var connectionMethod = await Application.Current?.MainPage?.DisplayActionSheet(
                "Choisir la méthode de connexion",
                "Annuler",
                null,
                "Bluetooth",
                "Autre (à venir)"
            );

            if (connectionMethod == "Annuler" || connectionMethod == null)
            {
                StatusMessage = "Activation annulée";
                return false;
            }

            if (connectionMethod == "Autre (à venir)")
            {
                StatusMessage = "Cette option sera disponible prochainement";
                return false;
            }

            // Proceed with Bluetooth
            var hasPermissions = await _bluetoothService.RequestPermissionsAsync();
            if (!hasPermissions)
            {
                StatusMessage = "Permissions Bluetooth requises";
                return false;
            }

            await _bluetoothService.StartScanningAsync();
            
            if (LocalPlayer != null)
            {
                await _bluetoothService.StartBroadcastingAsync(LocalPlayer);
            }

            IsScanning = true;
            StatusMessage = "Recherche de joueurs...";
            return true;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erreur: {ex.Message}";
            return false;
        }
    }

    [RelayCommand]
    private async Task StopDetectionAsync()
    {
        await _bluetoothService.StopScanningAsync();
        await _bluetoothService.StopBroadcastingAsync();
        IsScanning = false;
        StatusMessage = "Détection arrêtée";
    }

    [RelayCommand]
    private async Task GiveBuffaloAsync(NearbyPlayer nearbyPlayer)
    {
        if (LocalPlayer == null) return;

        var buffaloEvent = await _buffaloService.GiveBuffaloAsync(
            LocalPlayer, 
            nearbyPlayer.Player,
            null // TODO: Ajouter la géolocalisation pour le nom du bar
        );

        StatusMessage = $"BUFFALO envoyé à {nearbyPlayer.Player.Pseudo} !";
        await RefreshStatsAsync();

        // Affiche une notification
        // TODO: Implémenter les notifications
    }

    [RelayCommand]
    private async Task SettleSlateAsync(NearbyPlayer nearbyPlayer)
    {
        if (LocalPlayer == null) return;

        // Récupère les ardoises que ce joueur nous doit
        var slates = await _buffaloService.GetPendingSlatesOwedToAsync(LocalPlayer.Id);
        var slateWithPlayer = slates.FirstOrDefault(s => s.DebtorPlayerId == nearbyPlayer.Player.Id);

        if (slateWithPlayer != null)
        {
            await _buffaloService.SettleSlateAsync(slateWithPlayer);
            StatusMessage = $"Ardoise réglée avec {nearbyPlayer.Player.Pseudo} !";
            
            // Met à jour le compteur d'ardoise du joueur proche
            nearbyPlayer.SlateOwedToYou--;
            
            await RefreshStatsAsync();
        }
        else
        {
            StatusMessage = $"Pas d'ardoise en attente avec {nearbyPlayer.Player.Pseudo}";
        }
    }

    private void OnPlayerDetected(object? sender, NearbyPlayer nearbyPlayer)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var existing = NearbyPlayers.FirstOrDefault(p => p.Player.BluetoothId == nearbyPlayer.Player.BluetoothId);
            if (existing == null)
            {
                // Charge les infos d'ardoise
                if (LocalPlayer != null)
                {
                    var slatesOwed = await _buffaloService.GetPendingSlatesOwedByAsync(LocalPlayer.Id);
                    var slatesOwedToYou = await _buffaloService.GetPendingSlatesOwedToAsync(LocalPlayer.Id);

                    nearbyPlayer.SlateOwed = slatesOwed.Count(s => s.CreditorPlayerId == nearbyPlayer.Player.Id);
                    nearbyPlayer.SlateOwedToYou = slatesOwedToYou.Count(s => s.DebtorPlayerId == nearbyPlayer.Player.Id);
                }

                NearbyPlayers.Add(nearbyPlayer);
                NearbyPlayersCount = NearbyPlayers.Count;
                StatusMessage = $"{nearbyPlayer.Player.Pseudo} détecté !";
            }
        });
    }

    private void OnPlayerLost(object? sender, Player player)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var toRemove = NearbyPlayers.FirstOrDefault(p => p.Player.BluetoothId == player.BluetoothId);
            if (toRemove != null)
            {
                NearbyPlayers.Remove(toRemove);
                NearbyPlayersCount = NearbyPlayers.Count;
            }
        });
    }

    private async void OnBuffaloReceived(object? sender, BuffaloEvent buffaloEvent)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var giver = await _database.GetPlayerByIdAsync(buffaloEvent.GiverId);
            StatusMessage = $"BUFFALO reçu de {giver?.Pseudo ?? "quelqu'un"} !";

            // Affiche une alerte pour accepter ou refuser
            // Ceci sera géré par la View
        });
    }
}
