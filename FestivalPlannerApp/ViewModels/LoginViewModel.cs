using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;

namespace FestivalPlannerApp.ViewModels;

public partial class LoginViewModel (ISpotifyService spotifyService, IAlertService alertService) : BaseViewModel
{
    [RelayCommand]
    public async Task Appearing()
    {
        if (spotifyService.IsSignedIn())
        {
            await Shell.Current.GoToAsync($"//{nameof(MyFestivalsPage)}");
        }
    }
    [RelayCommand]
    public async Task Login ()
    {
        if (await spotifyService.Authenticate())
        {
            await Shell.Current.GoToAsync($"//{nameof(MyFestivalsPage)}");
        }
        else
        {
            await alertService.ShowAlert("Login failed", "Please try again");
        }
    }
}
