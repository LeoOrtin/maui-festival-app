using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;

namespace FestivalPlannerApp.ViewModels
{
    public partial class LoginViewModel (ISpotifyService spotifyService, IAlertService alertService) : ObservableObject
    {
        private readonly ISpotifyService _spotifyService = spotifyService;
        private readonly IAlertService _alertService = alertService;
        [RelayCommand]
        public async Task Appearing()
        {
            if (_spotifyService.IsSignedIn())
            {
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
        }
        [RelayCommand]
        public async Task Login ()
        {
            if (await _spotifyService.Authenticate())
            {
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
            else
            {
                await _alertService.ShowAlert("Login failed", "Please try again");
            }
        }
    }
}
