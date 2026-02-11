using BuffaloApp.ViewModels;

namespace BuffaloApp.Views;

public partial class RulesPage : ContentPage
{
    public RulesPage(RulesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
