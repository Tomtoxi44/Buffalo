using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class RulesPage : ContentPage
{
    private RulesViewModel? _viewModel;

    public RulesPage()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        if (Handler?.MauiContext?.Services != null && _viewModel == null)
        {
            _viewModel = Handler.MauiContext.Services.GetRequiredService<RulesViewModel>();
            BindingContext = _viewModel;
        }
    }
}
