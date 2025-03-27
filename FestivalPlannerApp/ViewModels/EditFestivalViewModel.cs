using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;
using System;
using System.Collections.ObjectModel;

namespace FestivalPlannerApp.ViewModels;

[QueryProperty("FestivalId", "FestivalId")] // Property to receive the FestivalId from the navigation & key
public partial class EditFestivalViewModel(IDatabaseService databaseService) : BaseViewModel
{
    [ObservableProperty]
    public partial int FestivalId { get; set; }
    [ObservableProperty]
    public partial Festival Festival { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<Day> Days { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<Stage> Stages { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<Concert> Concerts { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<ScheduleItem> Schedule { get; set; } = new ObservableCollection<ScheduleItem>();

    [RelayCommand]
    public async Task NavigatedTo()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            Festival = await databaseService.GetFestivalByIdAsync(FestivalId);
            var days = await databaseService.GetDaysAsync(FestivalId);
            Days = new ObservableCollection<Day>(days);
            Stages = new ObservableCollection<Stage>(await databaseService.GetStagesAsync(FestivalId));
            Concerts = new ObservableCollection<Concert>(await databaseService.GetConcertsAsync(FestivalId));

            GenerateSchedule();
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
    [RelayCommand]
    public async Task EditConcert(string concertName)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"/{nameof(EditConcertPage)}", true,
                new Dictionary<string, object>
                {
                    { "ConcertName", concertName },
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

    //[RelayCommand]
    //private async Task LoadMoreItems()
    //{
    //    if(IsBusy)
    //        return;
    //    try
    //    {
    //        IsBusy = true;
    //        var moreDays = await databaseService.GetDaysAsync(FestivalId); // Load 5 at a time
    //        foreach (var day in moreDays)
    //        {
    //            Days.Add(day);
    //        }
    //        GenerateSchedule();
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}
    private void GenerateSchedule()
    {
        try
        {
            Schedule.Clear();

            foreach (var day in Days)
            {
                var dayItems = new List<ScheduleItem>();
                foreach (var time in day.TimeSlots)
                {
                    var item = new ScheduleItem
                    {
                        Date = day.StartDateTime,
                        StartTime = time,
                        EndTime = time.Add(TimeSpan.FromHours(1))
                    };

                    var relevantConcerts = Concerts
                        .Where(c => c.DayId == day.Id && c.StartTime <= time && c.EndTime > time)
                        .ToDictionary(c => c.StageId, c => c.ArtistName);

                    item.StageEvents = Stages.ToDictionary(
                        s => s.Id,
                        s => relevantConcerts.TryGetValue(s.Id, out var artist) ? artist : string.Empty);

                    dayItems.Add(item);
                    Schedule.Add(item);
                }
                // Notify all days that Schedule is updated
                day.InjectViewModel(this);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
