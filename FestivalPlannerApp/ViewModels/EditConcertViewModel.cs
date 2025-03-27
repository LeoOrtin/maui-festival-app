using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;
using System.Collections.ObjectModel;

namespace FestivalPlannerApp.ViewModels;

[QueryProperty("ConcertName", "ConcertName")]
[QueryProperty("FestivalId", "FestivalId")]
public partial class EditConcertViewModel(IDatabaseService databaseService, ISpotifyService spotifyService, IAlertService alertService) : BaseViewModel
{
    private List<Day> days = [];
    private System.Timers.Timer? _searchTimer;
    private bool startPicker = false;
    private bool endPicker = false;
    private string? artistId;
    [ObservableProperty]
    public partial string? SearchText { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<Artist> ArtistsResult { get; set; } = [];
    [ObservableProperty]
    public partial string ConcertName { get; set; }
    [ObservableProperty]
    public partial int FestivalId { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<Stage> Stages { get; set; }
    [ObservableProperty]
    public partial Stage SelectedStage { get; set; } = new();
    [ObservableProperty]
    public partial Day SelectedDay { get; set; }
    [ObservableProperty]
    public partial TimeSpan SelectedStartTime { get; set; }
    [ObservableProperty]
    public partial TimeSpan SelectedEndTime { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<TimeSpan> StartTimeSlots { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<TimeSpan> EndTimeSlots { get; set; }
    [ObservableProperty]
    public partial Festival CurrentFestival { get; set; }
    [ObservableProperty]
    public partial Concert CurrentConcert { get; set; } = new();
    [RelayCommand]
    public async Task NavigatedTo()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            CurrentFestival = await databaseService.GetFestivalByIdAsync(FestivalId);
            Stages = new ObservableCollection<Stage>(await databaseService.GetStagesAsync(FestivalId));
            days = await databaseService.GetDaysAsync(FestivalId);
            CurrentConcert = await databaseService.GetConcertByNameAsync(ConcertName, FestivalId) ?? new();
            SelectedStage = Stages.FirstOrDefault(s => s.Id == CurrentConcert.StageId) ?? new();
            SelectedStartTime = CurrentConcert.StartTime;
            SelectedEndTime = CurrentConcert.EndTime;
            SearchText = CurrentConcert.ArtistName;
            var selectedDay = await databaseService.GetDayByIdAsync(CurrentConcert.DayId) ?? days[0];
            StartTimeSlots = new ObservableCollection<TimeSpan>(selectedDay.TimeSlots);
            SelectedDay = new Day
            {
                Id = selectedDay.Id,
                StartDateTime = selectedDay.StartDateTime
            };
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
    partial void OnSearchTextChanged(string? value)
    {
        RestartSearchTimer();
    }
    private void RestartSearchTimer()
    {
        _searchTimer?.Stop();
        _searchTimer = new System.Timers.Timer(300); // 300ms debounce delay
        _searchTimer.Elapsed += async (s, e) =>
        {
            _searchTimer.Stop();
            await Search();
        };
        _searchTimer.Start();
    }
    private async Task Search()
    {
        try
        {
            ArtistsResult.Clear();
            if (SearchText != CurrentConcert.ArtistName)
            {
                var result = new ObservableCollection<Artist>(await spotifyService.SearchArtists(SearchText ?? string.Empty));
                ArtistsResult = result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    [RelayCommand]
    public void ArtistSelection(Artist selectedArtist)
    {
        SearchText = selectedArtist.Name;
        artistId = selectedArtist.Id;
        _searchTimer?.Stop();
        ArtistsResult.Clear();
    }
    [RelayCommand]
    public void DateSelection()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var selectedDay = days.FirstOrDefault(d => d.StartDateTime.Date == SelectedDay.StartDateTime.Date) ?? new();
            StartTimeSlots.Clear();
            StartTimeSlots = selectedDay.TimeSlots;
            SelectedDay = new Day
            {
                Id = selectedDay.Id,
                StartDateTime = selectedDay.StartDateTime
            };

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
    public void StartTimeSelection()
    {
        startPicker = true;
        EndTimeSlots?.Clear();
        EndTimeSlots = new ObservableCollection<TimeSpan>
            (StartTimeSlots.SkipWhile(x => x != SelectedStartTime.Add(TimeSpan.FromHours(1))));
    }
    [RelayCommand]
    public void EndTimeSelection()
    {
        endPicker = true;
    }
    [RelayCommand]
    public async Task Save()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            if(string.IsNullOrEmpty(SearchText) 
                || SelectedStage == null
                || !startPicker || !endPicker)
            {
                await alertService.ShowAlert("Please fill in all fields", "");
                return;
            }

            var concerts = await databaseService.GetConcertsAsync(FestivalId);

            if(concerts.Any(c => c.ArtistName == SearchText && c.Id != CurrentConcert.Id))
            {
                await alertService.ShowAlert("Artist already exists", "Please select another artist");
                return;
            }

            // Validate if the concert's timeslot overlaps with any existing concert on the same stage and day
            bool hasOverlap = concerts.Any(c =>
                c.Id != CurrentConcert.Id && // Ignore the current concert if editing
                c.StageId == SelectedStage.Id && // Same stage
                c.DayId == SelectedDay.Id && // Same day
                SelectedStartTime < c.EndTime && SelectedEndTime > c.StartTime); // Overlapping time range check
            if (hasOverlap)
            {
                await alertService.ShowAlert("Time Slot Conflict", "The selected time overlaps with an existing concert. Please choose another time or stage.");
                return;
            }
            if (CurrentConcert.Id == 0)
            {
                // Add new concert
                await databaseService.AddConcertAsync(
                    new Concert
                    {
                        FestivalId = FestivalId,
                        StageId = SelectedStage.Id,
                        StageName = SelectedStage.Name,
                        DayId = SelectedDay.Id,
                        ArtistId = artistId,
                        ArtistName = SearchText,
                        StartTime = SelectedStartTime,
                        EndTime = SelectedEndTime
                    });
            }
            else
            {
                // Update existing concert
                CurrentConcert.FestivalId = FestivalId;
                CurrentConcert.StageId = SelectedStage.Id;
                CurrentConcert.StageName = SelectedStage.Name;
                CurrentConcert.DayId = SelectedDay.Id;
                CurrentConcert.ArtistName = SearchText;
                CurrentConcert.StartTime = SelectedStartTime;
                CurrentConcert.EndTime = SelectedEndTime;
                await databaseService.UpdateConcertAsync(CurrentConcert);
            }

                await Shell.Current.GoToAsync("..", true,
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
    public async Task Delete()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            if(await alertService.ShowConfirmation("Delete Concert", "Are you sure you want to delete this concert?"))
            {
                await databaseService.DeleteConcertAsync(CurrentConcert);
                await Shell.Current.GoToAsync($"../{nameof(EditFestivalPage)}", true,
                    new Dictionary<string, object>
                    {
                        { "FestivalId", FestivalId }
                    });
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
}
