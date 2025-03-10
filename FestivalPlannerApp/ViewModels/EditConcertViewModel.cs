using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;
using System.Collections.ObjectModel;

namespace FestivalPlannerApp.ViewModels
{
    [QueryProperty("ConcertName", "ConcertName")]
    [QueryProperty("FestivalId", "FestivalId")]
    public partial class EditConcertViewModel(IDatabaseService databaseService) : BaseViewModel
    {
        private List<Day> days = [];
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
            EndTimeSlots?.Clear();
            EndTimeSlots = new ObservableCollection<TimeSpan>
                (StartTimeSlots.SkipWhile(x => x != CurrentConcert.StartTime.Add(TimeSpan.FromHours(1))));
        }
        [RelayCommand]
        public async Task Save()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await databaseService.AddConcertAsync(
                    new Concert
                    {
                        FestivalId = FestivalId,
                        StageId = SelectedStage.Id,
                        DayId = SelectedDay.Id,
                        ArtistName = CurrentConcert.ArtistName,
                        StartTime = CurrentConcert.StartTime,
                        EndTime = CurrentConcert.EndTime
                    });
                var concerts = await databaseService.GetConcertsAsync(FestivalId);

                await Shell.Current.GoToAsync($"../{nameof(EditFestivalPage)}", true,
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
        public async Task Cancel()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync("..");
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
}
