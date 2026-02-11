using BuffaloApp.Data;
using BuffaloApp.Models;

namespace BuffaloApp.Services;

/// <summary>
/// Service de gestion des Buffalo et des ardoises
/// </summary>
public class BuffaloService
{
    private readonly BuffaloDatabase _database;
    private readonly IBluetoothService _bluetoothService;

    public BuffaloService(BuffaloDatabase database, IBluetoothService bluetoothService)
    {
        _database = database;
        _bluetoothService = bluetoothService;
    }

    /// <summary>
    /// Donne un Buffalo à un joueur
    /// </summary>
    public async Task<BuffaloEvent> GiveBuffaloAsync(Player giver, Player receiver, string? location = null)
    {
        var buffaloEvent = new BuffaloEvent
        {
            GiverId = giver.Id,
            ReceiverId = receiver.Id,
            EventDate = DateTime.Now,
            Status = BuffaloStatus.Pending,
            Location = location
        };

        await _database.SaveBuffaloEventAsync(buffaloEvent);

        // Notifie le joueur via Bluetooth si possible
        try
        {
            await _bluetoothService.SendBuffaloAsync(receiver);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors de l'envoi du Buffalo: {ex.Message}");
        }

        return buffaloEvent;
    }

    /// <summary>
    /// Accepte un Buffalo (le joueur boit cul-sec)
    /// </summary>
    public async Task AcceptBuffaloAsync(BuffaloEvent buffaloEvent)
    {
        buffaloEvent.Status = BuffaloStatus.Accepted;
        await _database.SaveBuffaloEventAsync(buffaloEvent);

        // Met à jour les compteurs
        var giver = await _database.GetPlayerByIdAsync(buffaloEvent.GiverId);
        var receiver = await _database.GetPlayerByIdAsync(buffaloEvent.ReceiverId);

        if (giver != null)
        {
            giver.BuffaloGiven++;
            await _database.SavePlayerAsync(giver);
        }

        if (receiver != null)
        {
            receiver.BuffaloReceived++;
            await _database.SavePlayerAsync(receiver);
        }
    }

    /// <summary>
    /// Refuse un Buffalo et le met sur l'ardoise
    /// </summary>
    public async Task<SlateEntry> RefuseBuffaloAsync(BuffaloEvent buffaloEvent, string? note = null)
    {
        buffaloEvent.Status = BuffaloStatus.OnSlate;
        await _database.SaveBuffaloEventAsync(buffaloEvent);

        var slateEntry = new SlateEntry
        {
            OriginalBuffaloEventId = buffaloEvent.Id,
            CreditorPlayerId = buffaloEvent.GiverId,
            DebtorPlayerId = buffaloEvent.ReceiverId,
            CreatedDate = DateTime.Now,
            Location = buffaloEvent.Location,
            Note = note,
            IsSettled = false
        };

        await _database.SaveSlateEntryAsync(slateEntry);
        return slateEntry;
    }

    /// <summary>
    /// Règle une ardoise (le créancier redonne son Buffalo)
    /// </summary>
    public async Task<BuffaloEvent> SettleSlateAsync(SlateEntry slateEntry, string? location = null)
    {
        // Crée un nouvel événement Buffalo pour le règlement
        var buffaloEvent = new BuffaloEvent
        {
            GiverId = slateEntry.CreditorPlayerId,
            ReceiverId = slateEntry.DebtorPlayerId,
            EventDate = DateTime.Now,
            Status = BuffaloStatus.SlateSettled,
            Location = location,
            Comment = $"Ardoise du {slateEntry.CreatedDate:dd/MM/yyyy} réglée"
        };

        await _database.SaveBuffaloEventAsync(buffaloEvent);

        // Marque l'ardoise comme réglée
        await _database.SettleSlateEntryAsync(slateEntry.Id);

        // Met à jour l'événement original
        var originalEvent = await GetBuffaloEventByIdAsync(slateEntry.OriginalBuffaloEventId);
        if (originalEvent != null)
        {
            originalEvent.Status = BuffaloStatus.SlateSettled;
            await _database.SaveBuffaloEventAsync(originalEvent);
        }

        // Met à jour les compteurs
        var giver = await _database.GetPlayerByIdAsync(slateEntry.CreditorPlayerId);
        var receiver = await _database.GetPlayerByIdAsync(slateEntry.DebtorPlayerId);

        if (giver != null)
        {
            giver.BuffaloGiven++;
            await _database.SavePlayerAsync(giver);
        }

        if (receiver != null)
        {
            receiver.BuffaloReceived++;
            await _database.SavePlayerAsync(receiver);
        }

        return buffaloEvent;
    }

    /// <summary>
    /// Récupère les ardoises en attente pour un joueur (ce qu'il doit)
    /// </summary>
    public async Task<List<SlateEntry>> GetPendingSlatesOwedByAsync(int playerId)
    {
        return await _database.GetSlateEntriesOwedByPlayerAsync(playerId);
    }

    /// <summary>
    /// Récupère les ardoises en attente qu'un joueur peut réclamer
    /// </summary>
    public async Task<List<SlateEntry>> GetPendingSlatesOwedToAsync(int playerId)
    {
        return await _database.GetSlateEntriesOwedToPlayerAsync(playerId);
    }

    /// <summary>
    /// Récupère un événement Buffalo par son ID
    /// </summary>
    private async Task<BuffaloEvent?> GetBuffaloEventByIdAsync(int id)
    {
        var events = await _database.GetAllBuffaloEventsAsync();
        return events.FirstOrDefault(e => e.Id == id);
    }

    /// <summary>
    /// Récupère les statistiques d'un joueur
    /// </summary>
    public async Task<PlayerStats> GetPlayerStatsAsync(int playerId)
    {
        var (given, received, slateOwed, slateOwedToYou) = await _database.GetPlayerStatsAsync(playerId);
        
        return new PlayerStats
        {
            BuffaloGiven = given,
            BuffaloReceived = received,
            SlateOwed = slateOwed,
            SlateOwedToYou = slateOwedToYou
        };
    }

    /// <summary>
    /// Récupère le classement des joueurs
    /// </summary>
    public async Task<List<LeaderboardEntry>> GetLeaderboardAsync()
    {
        var leaderboard = await _database.GetLeaderboardAsync();
        
        return leaderboard.Select((x, index) => new LeaderboardEntry
        {
            Rank = index + 1,
            Player = x.player,
            BuffaloGiven = x.count
        }).ToList();
    }
}

/// <summary>
/// Statistiques d'un joueur
/// </summary>
public class PlayerStats
{
    public int BuffaloGiven { get; set; }
    public int BuffaloReceived { get; set; }
    public int SlateOwed { get; set; }
    public int SlateOwedToYou { get; set; }
    
    public int Balance => BuffaloGiven - BuffaloReceived;
}

/// <summary>
/// Entrée du classement
/// </summary>
public class LeaderboardEntry
{
    public int Rank { get; set; }
    public Player Player { get; set; } = new();
    public int BuffaloGiven { get; set; }
}
