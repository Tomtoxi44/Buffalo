using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class SlatePage : ContentPage
{
    private SlateViewModel? _viewModel;

    public SlatePage()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        if (Handler?.MauiContext?.Services != null && _viewModel == null)
        {
            _viewModel = Handler.MauiContext.Services.GetRequiredService<SlateViewModel>();
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
}
