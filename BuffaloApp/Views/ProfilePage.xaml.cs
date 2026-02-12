using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    public ProfilePage(ProfileViewModel viewModel)
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

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        _viewModel.ToggleThemeCommand.Execute(null);
    }
}
