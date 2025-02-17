using FestivalPlannerApp.Services;
using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class HomePage : ContentPage
{
    private readonly ISpotifyService _spotifyService;
    private readonly IAlertService _alertService;
    public HomePage(HomeViewModel vm, ISpotifyService spotifyService, IAlertService alertService)
	{
		InitializeComponent();
        BindingContext = vm;
        _spotifyService = spotifyService;
        _alertService = alertService;
    }
    override protected async void OnAppearing()
    {
        base.OnAppearing();
        if (!_spotifyService.IsSignedIn())
        {
            while (!await _spotifyService.Authenticate())
            {
                await _alertService.ShowAlert("Error", "Authorization failed");
            }
            await _alertService.ShowAlert("Success", "Logged in!");
        }
        HomeViewModel homeViewModel = (HomeViewModel)BindingContext;
        homeViewModel.LoadData();
    }
}