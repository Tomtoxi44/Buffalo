using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BuffaloApp.Data;
using BuffaloApp.Models;

namespace BuffaloApp.ViewModels;

/// <summary>
/// ViewModel pour le profil du joueur
/// </summary>
public partial class ProfileViewModel : ObservableObject
{
    private readonly BuffaloDatabase _database;

    [ObservableProperty]
    private Player? _player;

    [ObservableProperty]
    private string _pseudo = string.Empty;

    [ObservableProperty]
    private bool _isRightHanded = true;

    [ObservableProperty]
    private int _buffaloGiven;

    [ObservableProperty]
    private int _buffaloReceived;

    [ObservableProperty]
    private string _dominantHand = "Droitier";

    [ObservableProperty]
    private string _memberSince = string.Empty;

    [ObservableProperty]
    private bool _isDarkMode = true;

    public ProfileViewModel(BuffaloDatabase database)
    {
        _database = database;
        // Initialize with current theme
        _isDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
    }

    public async Task InitializeAsync()
    {
        Player = await _database.GetLocalPlayerAsync();
        
        if (Player != null)
        {
            Pseudo = Player.Pseudo;
            IsRightHanded = Player.IsRightHanded;
            BuffaloGiven = Player.BuffaloGiven;
            BuffaloReceived = Player.BuffaloReceived;
            DominantHand = Player.IsRightHanded ? "Droitier" : "Gaucher";
            MemberSince = $"Membre depuis le {Player.FirstSeen:dd/MM/yyyy}";
        }
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        if (Player == null) return;

        Player.Pseudo = Pseudo;
        Player.IsRightHanded = IsRightHanded;
        
        await _database.SavePlayerAsync(Player);
        DominantHand = IsRightHanded ? "Droitier" : "Gaucher";
    }

    [RelayCommand]
    private void ToggleHand()
    {
        IsRightHanded = !IsRightHanded;
        DominantHand = IsRightHanded ? "Droitier" : "Gaucher";
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
        }
    }

    partial void OnPseudoChanged(string value)
    {
        if (Player != null && !string.IsNullOrWhiteSpace(value))
        {
            Player.Pseudo = value;
        }
    }
}
