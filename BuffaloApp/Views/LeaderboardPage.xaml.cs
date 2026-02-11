using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class LeaderboardPage : ContentPage
{
    private readonly LeaderboardViewModel _viewModel;

    public LeaderboardPage(LeaderboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
