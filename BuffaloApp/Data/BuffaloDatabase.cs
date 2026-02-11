using SQLite;
using BuffaloApp.Models;

namespace BuffaloApp.Data;

/// <summary>
/// Service de base de données SQLite pour l'application Buffalo
/// </summary>
public class BuffaloDatabase
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public BuffaloDatabase()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "buffalo.db3");
    }

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        
        await _database.CreateTableAsync<Player>();
        await _database.CreateTableAsync<BuffaloEvent>();
        await _database.CreateTableAsync<SlateEntry>();
    }

    #region Player Operations

    public async Task<Player?> GetLocalPlayerAsync()
    {
        await InitAsync();
        return await _database!.Table<Player>().FirstOrDefaultAsync(p => p.IsLocalPlayer);
    }

    public async Task<Player?> GetPlayerByIdAsync(int id)
    {
        await InitAsync();
        return await _database!.GetAsync<Player>(id);
    }

    public async Task<Player?> GetPlayerByBluetoothIdAsync(string bluetoothId)
    {
        await InitAsync();
        return await _database!.Table<Player>().FirstOrDefaultAsync(p => p.BluetoothId == bluetoothId);
    }

    public async Task<List<Player>> GetAllPlayersAsync()
    {
        await InitAsync();
        return await _database!.Table<Player>().Where(p => !p.IsLocalPlayer).ToListAsync();
    }

    public async Task<int> SavePlayerAsync(Player player)
    {
        await InitAsync();
        if (player.Id != 0)
        {
            await _database!.UpdateAsync(player);
            return player.Id;
        }
        else
        {
            await _database!.InsertAsync(player);
            return player.Id;
        }
    }

    public async Task DeletePlayerAsync(Player player)
    {
        await InitAsync();
        await _database!.DeleteAsync(player);
    }

    #endregion

    #region Buffalo Event Operations

    public async Task<List<BuffaloEvent>> GetAllBuffaloEventsAsync()
    {
        await InitAsync();
        return await _database!.Table<BuffaloEvent>().OrderByDescending(e => e.EventDate).ToListAsync();
    }

    public async Task<List<BuffaloEvent>> GetBuffaloEventsForPlayerAsync(int playerId)
    {
        await InitAsync();
        return await _database!.Table<BuffaloEvent>()
            .Where(e => e.GiverId == playerId || e.ReceiverId == playerId)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<int> SaveBuffaloEventAsync(BuffaloEvent buffaloEvent)
    {
        await InitAsync();
        if (buffaloEvent.Id != 0)
        {
            await _database!.UpdateAsync(buffaloEvent);
            return buffaloEvent.Id;
        }
        else
        {
            await _database!.InsertAsync(buffaloEvent);
            return buffaloEvent.Id;
        }
    }

    public async Task<int> GetBuffaloGivenCountAsync(int playerId)
    {
        await InitAsync();
        return await _database!.Table<BuffaloEvent>()
            .Where(e => e.GiverId == playerId && e.Status == BuffaloStatus.Accepted)
            .CountAsync();
    }

    public async Task<int> GetBuffaloReceivedCountAsync(int playerId)
    {
        await InitAsync();
        return await _database!.Table<BuffaloEvent>()
            .Where(e => e.ReceiverId == playerId && e.Status == BuffaloStatus.Accepted)
            .CountAsync();
    }

    #endregion

    #region Slate Operations

    public async Task<List<SlateEntry>> GetAllSlateEntriesAsync()
    {
        await InitAsync();
        return await _database!.Table<SlateEntry>().ToListAsync();
    }

    public async Task<List<SlateEntry>> GetPendingSlateEntriesAsync()
    {
        await InitAsync();
        return await _database!.Table<SlateEntry>().Where(s => !s.IsSettled).ToListAsync();
    }

    public async Task<List<SlateEntry>> GetSlateEntriesOwedByPlayerAsync(int debtorPlayerId)
    {
        await InitAsync();
        return await _database!.Table<SlateEntry>()
            .Where(s => s.DebtorPlayerId == debtorPlayerId && !s.IsSettled)
            .ToListAsync();
    }

    public async Task<List<SlateEntry>> GetSlateEntriesOwedToPlayerAsync(int creditorPlayerId)
    {
        await InitAsync();
        return await _database!.Table<SlateEntry>()
            .Where(s => s.CreditorPlayerId == creditorPlayerId && !s.IsSettled)
            .ToListAsync();
    }

    public async Task<int> SaveSlateEntryAsync(SlateEntry slateEntry)
    {
        await InitAsync();
        if (slateEntry.Id != 0)
        {
            await _database!.UpdateAsync(slateEntry);
            return slateEntry.Id;
        }
        else
        {
            await _database!.InsertAsync(slateEntry);
            return slateEntry.Id;
        }
    }

    public async Task SettleSlateEntryAsync(int slateEntryId)
    {
        await InitAsync();
        var entry = await _database!.GetAsync<SlateEntry>(slateEntryId);
        if (entry != null)
        {
            entry.IsSettled = true;
            entry.SettledDate = DateTime.Now;
            await _database.UpdateAsync(entry);
        }
    }

    public async Task<int> GetSlateCountOwedByPlayerAsync(int debtorPlayerId)
    {
        await InitAsync();
        return await _database!.Table<SlateEntry>()
            .Where(s => s.DebtorPlayerId == debtorPlayerId && !s.IsSettled)
            .CountAsync();
    }

    public async Task<int> GetSlateCountOwedToPlayerAsync(int creditorPlayerId)
    {
        await InitAsync();
        return await _database!.Table<SlateEntry>()
            .Where(s => s.CreditorPlayerId == creditorPlayerId && !s.IsSettled)
            .CountAsync();
    }

    #endregion

    #region Statistics

    public async Task<(int given, int received, int slateOwed, int slateOwedToYou)> GetPlayerStatsAsync(int playerId)
    {
        await InitAsync();
        
        var given = await GetBuffaloGivenCountAsync(playerId);
        var received = await GetBuffaloReceivedCountAsync(playerId);
        var slateOwed = await GetSlateCountOwedByPlayerAsync(playerId);
        var slateOwedToYou = await GetSlateCountOwedToPlayerAsync(playerId);
        
        return (given, received, slateOwed, slateOwedToYou);
    }

    public async Task<List<(Player player, int count)>> GetLeaderboardAsync()
    {
        await InitAsync();
        var players = await GetAllPlayersAsync();
        var leaderboard = new List<(Player player, int count)>();

        foreach (var player in players)
        {
            var given = await GetBuffaloGivenCountAsync(player.Id);
            leaderboard.Add((player, given));
        }

        return leaderboard.OrderByDescending(x => x.count).ToList();
    }

    #endregion
}
