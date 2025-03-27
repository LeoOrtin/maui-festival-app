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
    private readonly IDatabaseService _databaseService;
    [ObservableProperty]
    public partial User? CurrentUser { get; set; }
    public ProfileViewModel(ISpotifyService spotifyService, IAlertService alertService, IDatabaseService databaseService) 
    {
        _spotifyService = spotifyService;
        _alertService = alertService;
        _databaseService = databaseService;
        
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
        
    }
}
