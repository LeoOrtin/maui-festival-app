using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;

namespace FestivalPlannerApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly ISpotifyService _spotifyService;
    private readonly IAlertService _alertService;
    [ObservableProperty]
    public partial User? CurrentUser { get; set; }
    public ProfileViewModel(ISpotifyService spotifyService, IAlertService alertService) 
    {
        _spotifyService = spotifyService;
        _alertService = alertService;
        LoadUser();
    }
    public async void LoadUser()
    {
        CurrentUser = await _spotifyService.GetUser();
    }
    [RelayCommand]
    public async Task SignOut()
    {
        if(await _alertService.ShowConfirmation("Sign Out", "Are you sure you want to sign out"))
        {
            _spotifyService.SignOut();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
    [RelayCommand]
    public async Task Test()
    {
        var artistsMedium = await _spotifyService.GetTopArtists("medium_term", 50);
        var artistsLong = await _spotifyService.GetTopArtists("long_term", 50);
        for(int i = 0;  i < artistsMedium.Count; i++)
        {
            Console.WriteLine($"({i+1}) {artistsMedium[i].Name} // {artistsLong[i].Name}");
        }
    }
}
