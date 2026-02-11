using CommunityToolkit.Mvvm.ComponentModel;
using BuffaloApp.Data;
using BuffaloApp.Services;
using System.Collections.ObjectModel;

namespace BuffaloApp.ViewModels;

/// <summary>
/// ViewModel pour le classement
/// </summary>
public partial class LeaderboardViewModel : ObservableObject
{
    private readonly BuffaloService _buffaloService;
    private readonly BuffaloDatabase _database;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _myRank;

    [ObservableProperty]
    private int _myBuffaloGiven;

    public ObservableCollection<LeaderboardDisplayItem> Leaderboard { get; } = new();

    public LeaderboardViewModel(BuffaloDatabase database, BuffaloService buffaloService)
    {
        _database = database;
        _buffaloService = buffaloService;
    }

    public async Task InitializeAsync()
    {
        await RefreshAsync();
    }

    public async Task RefreshAsync()
    {
        IsLoading = true;

        try
        {
            var localPlayer = await _database.GetLocalPlayerAsync();
            var leaderboard = await _buffaloService.GetLeaderboardAsync();

            Leaderboard.Clear();

            foreach (var entry in leaderboard)
            {
                Leaderboard.Add(new LeaderboardDisplayItem
                {
                    Rank = entry.Rank,
                    Pseudo = entry.Player.Pseudo,
                    BuffaloGiven = entry.BuffaloGiven,
                    IsCurrentUser = entry.Player.Id == localPlayer?.Id,
                    RankEmoji = entry.Rank switch
                    {
                        1 => "🥇",
                        2 => "🥈",
                        3 => "🥉",
                        _ => $"#{entry.Rank}"
                    }
                });

                if (entry.Player.Id == localPlayer?.Id)
                {
                    MyRank = entry.Rank;
                    MyBuffaloGiven = entry.BuffaloGiven;
                }
            }

            // Ajoute le joueur local s'il n'est pas dans le classement
            if (localPlayer != null && !Leaderboard.Any(l => l.IsCurrentUser))
            {
                var stats = await _buffaloService.GetPlayerStatsAsync(localPlayer.Id);
                MyRank = Leaderboard.Count + 1;
                MyBuffaloGiven = stats.BuffaloGiven;

                Leaderboard.Add(new LeaderboardDisplayItem
                {
                    Rank = MyRank,
                    Pseudo = localPlayer.Pseudo + " (Toi)",
                    BuffaloGiven = stats.BuffaloGiven,
                    IsCurrentUser = true,
                    RankEmoji = $"#{MyRank}"
                });
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}

/// <summary>
/// Item d'affichage pour le classement
/// </summary>
public class LeaderboardDisplayItem
{
    public int Rank { get; set; }
    public string Pseudo { get; set; } = string.Empty;
    public int BuffaloGiven { get; set; }
    public bool IsCurrentUser { get; set; }
    public string RankEmoji { get; set; } = string.Empty;
}
