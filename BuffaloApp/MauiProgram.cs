using Microsoft.Extensions.Logging;
using BuffaloApp.Data;
using BuffaloApp.Services;
using BuffaloApp.ViewModels;
using BuffaloApp.Views;

namespace BuffaloApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Services
		builder.Services.AddSingleton<BuffaloDatabase>();
		builder.Services.AddSingleton<IBluetoothService, BluetoothService>();
		builder.Services.AddSingleton<BuffaloService>();

		// ViewModels
		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<ProfileViewModel>();
		builder.Services.AddSingleton<SlateViewModel>();
		builder.Services.AddSingleton<LeaderboardViewModel>();
		builder.Services.AddSingleton<RulesViewModel>();

		// Views
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<ProfilePage>();
		builder.Services.AddSingleton<SlatePage>();
		builder.Services.AddSingleton<LeaderboardPage>();
		builder.Services.AddSingleton<RulesPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
