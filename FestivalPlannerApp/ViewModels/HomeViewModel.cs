using CommunityToolkit.Mvvm.ComponentModel;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;

namespace FestivalPlannerApp.ViewModels
{
    public partial class HomeViewModel(ISpotifyService spotifyService, IAlertService alertService) : ObservableObject
    {
        private readonly ISpotifyService _spotifyService = spotifyService;
        private readonly IAlertService _alertService = alertService;
        [ObservableProperty]
        public partial List<Artist>? TopArtists { get; set; }
        public async void LoadData()
        {
            TopArtists = await _spotifyService.GetTopArtists();
        }
    }
}
