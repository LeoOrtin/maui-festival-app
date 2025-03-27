using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;
using System.Collections.ObjectModel;

namespace FestivalPlannerApp.ViewModels;

[QueryProperty("FestivalId", "FestivalId")]
public partial class FestivalInfoViewModel(IDatabaseService databaseService, ISpotifyService spotifyService) : BaseViewModel
{
    [ObservableProperty]
    public partial int FestivalId { get; set; }
    [ObservableProperty]
    public partial Festival Festival { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<ConcertGroup> RecommendedConcertsGroups { get; set; } = [];
    [RelayCommand]
    public async Task NavigatedTo()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var days = new List<Day>(await databaseService.GetDaysAsync(FestivalId));
            var concerts = await CalculateRecommendations();

            RecommendedConcertsGroups.Clear();

            foreach (var day in days)
            {
                RecommendedConcertsGroups.Add(new ConcertGroup(day, concerts));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    async Task EditFestival()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"/{nameof(EditFestivalPage)}", true,
                new Dictionary<string, object>
                {
                    { "FestivalId", FestivalId }
                });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    public async Task BackButton()
    {
        await Shell.Current.GoToAsync($"//{nameof(MyFestivalsPage)}");
    }
    private async Task<List<Concert>> CalculateRecommendations()
    {
        var recommendedConcerts = new List<Concert>();
        try
        {
            var days = new List<Day>(await databaseService.GetDaysAsync(FestivalId));
            var concerts = new List<Concert>(await databaseService.GetConcertsAsync(FestivalId));
            var topArtists = new List<Artist>(await spotifyService.GetTopArtists());
            
            var topArtistsScores = topArtists.ToDictionary(a => a.Id, a => topArtists.Count - topArtists.IndexOf(a));

            foreach (var day in days)
            {
                var concertsByDay = concerts.Where(c => c.DayId == day.Id).ToList();
                var startTimes = concertsByDay.Select(c => c.StartTime).Distinct().ToList();
                foreach (var time in startTimes)
                {
                    // Get all concerts that start at this time
                    var concertsAtTime = concertsByDay.Where(c => c.StartTime == time).ToList();
                    foreach (var concert in concertsAtTime)
                    {
                        if (concert.ArtistId != null && topArtistsScores.TryGetValue(concert.ArtistId, out var score))
                        {
                            concert.Score = score;
                        }
                    }
                    // Check if all the scores are 0
                    if (concertsAtTime.All(c => c.Score == 0))
                    {
                        // if all scores are 0, set all scores to artist popularity
                        foreach (var concert in concertsAtTime)
                        {
                            var artist = await spotifyService.GetArtist(concert.ArtistId ?? string.Empty);
                            concert.Score = artist.Popularity;
                        }
                    }
                    // Get concert with highest score
                    var recommended = concertsAtTime.OrderByDescending(c => c.Score).FirstOrDefault() ?? new();
                    recommendedConcerts.Add(recommended);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return recommendedConcerts;
    }
}
