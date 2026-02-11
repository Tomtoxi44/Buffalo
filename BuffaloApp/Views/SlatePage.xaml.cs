using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class SlatePage : ContentPage
{
    private readonly SlateViewModel _viewModel;

    public SlatePage(SlateViewModel viewModel)
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
