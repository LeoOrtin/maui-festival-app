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
    public partial ObservableCollection<Day> Days { get; set; } = new ObservableCollection<Day>();
    [ObservableProperty]
    public partial ObservableCollection<StageSchedule> Schedule { get; set; } = new ObservableCollection<StageSchedule>();
    [ObservableProperty]
    public partial Day SelectedDay { get; set; }

    private ObservableCollection<Stage> stages = new();
    private ObservableCollection<Concert> concerts = new();

    [RelayCommand]
    public async Task NavigatedTo()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            Festival = await databaseService.GetFestivalByIdAsync(FestivalId);
            Days = new ObservableCollection<Day>(await databaseService.GetDaysAsync(FestivalId));
            stages = new ObservableCollection<Stage>(await databaseService.GetStagesAsync(FestivalId));
            concerts = new ObservableCollection<Concert>(await databaseService.GetConcertsAsync(FestivalId));

            SelectedDay = Days[0];

            Schedule.Clear();
            foreach (var stage in stages)
            {
                Schedule.Add(new StageSchedule(stage, SelectedDay, concerts));
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
    public async Task DaySelection()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Task.Delay(10);

            Schedule.Clear();
            foreach (var stage in stages)
            {
                Schedule.Add(new StageSchedule(stage, SelectedDay, concerts));
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
    public async Task BackButton()
    {
        await Shell.Current.GoToAsync($"//{nameof(MyFestivalsPage)}");
    }
    [RelayCommand]
    public async Task ViewRecommendations()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"/{nameof(FestivalInfoPage)}", true,
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
    public async Task EditConcert(ArtistTimeSlot concert)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"/{nameof(EditConcertPage)}", true,
                new Dictionary<string, object>
                {
                    { "ConcertName", concert.ArtistName },
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
}
