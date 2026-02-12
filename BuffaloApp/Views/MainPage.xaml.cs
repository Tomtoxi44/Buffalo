using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class MainPage : ContentPage
{
    private MainViewModel? _viewModel;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        if (Handler?.MauiContext?.Services != null && _viewModel == null)
        {
            _viewModel = Handler.MauiContext.Services.GetRequiredService<MainViewModel>();
            BindingContext = _viewModel;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel != null)
        {
            await _viewModel.InitializeAsync();
        }
    }

    private async void OnBuffaloModeToggled(object? sender, ToggledEventArgs e)
    {
        if (_viewModel?.TogglePlayingCommand.CanExecute(null) == true)
        {
            await _viewModel.TogglePlayingCommand.ExecuteAsync(null);
        }
    }
}
