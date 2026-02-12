using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class ProfilePage : ContentPage
{
    private ProfileViewModel? _viewModel;

    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        if (Handler?.MauiContext?.Services != null && _viewModel == null)
        {
            _viewModel = Handler.MauiContext.Services.GetRequiredService<ProfileViewModel>();
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

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        _viewModel?.ToggleThemeCommand.Execute(null);
    }
}
