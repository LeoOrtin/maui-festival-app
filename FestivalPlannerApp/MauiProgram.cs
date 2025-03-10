using CommunityToolkit.Maui;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.ViewModels;
using FestivalPlannerApp.Views;
using Microsoft.Extensions.Logging;

namespace FestivalPlannerApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
			// Register ViewModels and Views
			.RegisterViewModels()
			.RegisterViews()
			// Register App Services
			.RegisterAppServices()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
	public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
	{
        builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MyFestivalsPage>();
        builder.Services.AddTransient<NewFestivalPage>();
        builder.Services.AddTransient<EditFestivalPage>();
        builder.Services.AddTransient<FestivalInfoPage>();
		builder.Services.AddTransient<EditConcertPage>();

        return builder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<MyFestivalsViewModel>();
		builder.Services.AddTransient<NewFestivalViewModel>();
        builder.Services.AddTransient<EditFestivalViewModel>();
		builder.Services.AddTransient<FestivalInfoViewModel>();
		builder.Services.AddTransient<EditConcertViewModel>();

        return builder;
	}
    public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
    {
		builder.Services.AddSingleton<ISpotifyService, SpotifyService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IAlertService, AlertService>();

        return builder;
    }
}
