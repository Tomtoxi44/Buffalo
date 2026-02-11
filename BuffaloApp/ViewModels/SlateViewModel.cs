using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BuffaloApp.Data;
using BuffaloApp.Models;
using BuffaloApp.Services;
using System.Collections.ObjectModel;

namespace BuffaloApp.ViewModels;

/// <summary>
/// ViewModel pour la gestion de l'ardoise
/// </summary>
public partial class SlateViewModel : ObservableObject
{
    private readonly BuffaloDatabase _database;
    private readonly BuffaloService _buffaloService;

    [ObservableProperty]
    private int _totalOwed;

    [ObservableProperty]
    private int _totalOwedToYou;

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<SlateDisplayItem> SlatesOwed { get; } = new();
    public ObservableCollection<SlateDisplayItem> SlatesOwedToYou { get; } = new();

    public SlateViewModel(BuffaloDatabase database, BuffaloService buffaloService)
    {
        _database = database;
        _buffaloService = buffaloService;
    }

    public async Task InitializeAsync()
    {
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsLoading = true;

        try
        {
            var localPlayer = await _database.GetLocalPlayerAsync();
            if (localPlayer == null) return;

            // Ardoises que je dois
            var slatesOwed = await _buffaloService.GetPendingSlatesOwedByAsync(localPlayer.Id);
            SlatesOwed.Clear();
            
            foreach (var slate in slatesOwed)
            {
                var creditor = await _database.GetPlayerByIdAsync(slate.CreditorPlayerId);
                SlatesOwed.Add(new SlateDisplayItem
                {
                    SlateEntry = slate,
                    OtherPlayerName = creditor?.Pseudo ?? "Inconnu",
                    DateText = $"Depuis le {slate.CreatedDate:dd/MM/yyyy}",
                    LocationText = slate.Location ?? "Lieu inconnu",
                    IsOwedByMe = true
                });
            }
            TotalOwed = slatesOwed.Count;

            // Ardoises qu'on me doit
            var slatesOwedToYou = await _buffaloService.GetPendingSlatesOwedToAsync(localPlayer.Id);
            SlatesOwedToYou.Clear();
            
            foreach (var slate in slatesOwedToYou)
            {
                var debtor = await _database.GetPlayerByIdAsync(slate.DebtorPlayerId);
                SlatesOwedToYou.Add(new SlateDisplayItem
                {
                    SlateEntry = slate,
                    OtherPlayerName = debtor?.Pseudo ?? "Inconnu",
                    DateText = $"Depuis le {slate.CreatedDate:dd/MM/yyyy}",
                    LocationText = slate.Location ?? "Lieu inconnu",
                    IsOwedByMe = false
                });
            }
            TotalOwedToYou = slatesOwedToYou.Count;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SettleSlateAsync(SlateDisplayItem item)
    {
        if (item.SlateEntry == null) return;

        bool confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Régler l'ardoise",
            $"Confirmer que {item.OtherPlayerName} a bu son Buffalo ?",
            "Oui, réglé !",
            "Annuler"
        );

        if (confirm)
        {
            await _buffaloService.SettleSlateAsync(item.SlateEntry);
            await RefreshAsync();
        }
    }
}

/// <summary>
/// Item d'affichage pour l'ardoise
/// </summary>
public class SlateDisplayItem
{
    public SlateEntry? SlateEntry { get; set; }
    public string OtherPlayerName { get; set; } = string.Empty;
    public string DateText { get; set; } = string.Empty;
    public string LocationText { get; set; } = string.Empty;
    public bool IsOwedByMe { get; set; }
}
