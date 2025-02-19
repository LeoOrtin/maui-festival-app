using FestivalPlannerApp.Services;
using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly ISpotifyService _spotifyService;
    public LoginPage(LoginViewModel vm, ISpotifyService spotifyService)
	{
		InitializeComponent();
        BindingContext = vm;
        _spotifyService = spotifyService;
    }
}