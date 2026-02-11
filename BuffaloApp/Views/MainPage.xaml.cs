using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
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

    private async void OnBuffaloModeToggled(object? sender, ToggledEventArgs e)
    {
        if (_viewModel.TogglePlayingCommand.CanExecute(null))
        {
            await _viewModel.TogglePlayingCommand.ExecuteAsync(null);
        }
    }
}
