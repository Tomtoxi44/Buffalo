using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class RulesPage : ContentPage
{
    public RulesPage()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        if (Handler?.MauiContext?.Services != null && BindingContext == null)
        {
            var viewModel = Handler.MauiContext.Services.GetRequiredService<RulesViewModel>();
            BindingContext = viewModel;
        }
    }
}
