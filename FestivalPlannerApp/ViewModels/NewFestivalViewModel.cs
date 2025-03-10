using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;
using System.Collections.ObjectModel;

namespace FestivalPlannerApp.ViewModels
{
    public partial class NewFestivalViewModel(IAlertService alertService, IDatabaseService databaseService) : BaseViewModel
    {
        [ObservableProperty]
        public partial int StagesStepperValue { get; set; }
        [ObservableProperty]
        public partial int DaysStepperValue { get; set; }
        [ObservableProperty]
        public partial ObservableCollection<Stage> Stages { get; set; } = new ObservableCollection<Stage> { new Stage() };
        [ObservableProperty]
        public partial Day NewDay { get; set; } = new Day();
        [ObservableProperty]
        public partial ObservableCollection<Day> Days { get; set; } = new ObservableCollection<Day>();
        [ObservableProperty]
        public partial Festival NewFestival { get; set; } = new Festival();

        [RelayCommand]
        public void StagesStepper()
        {
            if (StagesStepperValue < Stages.Count)
            {
                Stages.RemoveAt(Stages.Count - 1);
            }
            else
            {
                Stages.Add(new Stage());
            }
        }
        [RelayCommand]
        public void DateSelection()
        {
            NewDay.StartDateTime = NewDay.StartDateTime.Date + NewDay.StartTime;
        }
        [RelayCommand]
        public void StartTimeSelection()
        {
            NewDay.StartDateTime = NewDay.StartDateTime.Date + NewDay.StartTime;
        }
        [RelayCommand]
        public void EndTimeSelection()
        {
            if (!NewDay.AfterMidnight && NewDay.EndTime < NewDay.StartTime)
            {
                alertService.ShowAlert("End time must be later than Start time",
                    "If the concerts are finishing after midnigth, please tick the box");
            }
        }
        [RelayCommand]
        public void AddDay()
        {
            if (NewDay.AfterMidnight)
            {
                NewDay.EndDateTime = NewDay.StartDateTime.Date.AddDays(1) + NewDay.EndTime;
            }
            else if (NewDay.EndTime > NewDay.StartTime)
            {
                NewDay.EndDateTime = NewDay.StartDateTime.Date + NewDay.EndTime;
            }
            else
            {
                alertService.ShowAlert("Please enter a valid End time",
                    "End time must be later than Start time");
                return;
            }

            NewDay.StartDateTime = NewDay.StartDateTime.Date + NewDay.StartTime;

            // Save selected date before modifying NewDay
            DateTime selectedDate = NewDay.StartDateTime.Date;
            // Add the new day to the list
            Days.Add(new Day
            {
                StartDateTime = NewDay.StartDateTime,
                EndDateTime = NewDay.EndDateTime,
                AfterMidnight = NewDay.AfterMidnight
            });

            NewDay = new Day
            {
                MinStartDateTime = selectedDate.AddDays(1),
                MaxStartDateTime = selectedDate.AddDays(1)
            };

            OnPropertyChanged(nameof(NewDay));
        }
        [RelayCommand]
        public void DeleteDay()
        {
            if (Days.Count > 0)
            {
                DateTime lastDay = Days[^1].StartDateTime.Date;
                Days.Remove(Days[^1]);

                if (Days.Count == 0)
                {
                    NewDay = new Day
                    {
                        MinStartDateTime = DateTime.Today,
                        MaxStartDateTime = DateTime.MaxValue
                    };
                }
                else
                {
                    NewDay = new Day
                    {
                        MinStartDateTime = lastDay,
                        MaxStartDateTime = lastDay
                    };
                }
                OnPropertyChanged(nameof(NewDay));
            }
        }
        [RelayCommand]
        public async Task Save()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                foreach (var stage in Stages)
                {
                    if (string.IsNullOrEmpty(NewFestival.Name) || Days.Count == 0 || string.IsNullOrEmpty(stage.Name))
                    {
                        await alertService.ShowAlert("Please enter all mandatory fields",
                            "Enter festival name and add al leats one day");
                        return;
                    }
                }
                await databaseService.AddFestivalAsync(NewFestival);

                foreach (var stage in Stages)
                {
                    await databaseService.AddStageAsync(stage, NewFestival.Id);
                }

                DateTime start = DateTime.MaxValue;
                DateTime end = DateTime.MinValue;
                foreach (var day in Days)
                {
                    await databaseService.AddDayAsync(day, NewFestival.Id);
                    if (day.StartDateTime < start)
                    {
                        start = day.StartDateTime;
                    }
                    if (day.StartDateTime > end)
                    {
                        end = day.StartDateTime;
                    }
                }
                NewFestival.StartDate = start;
                NewFestival.EndDate = end;
                await databaseService.UpdateFestivalAsync(NewFestival);

                await Shell.Current.GoToAsync($"/{nameof(EditFestivalPage)}", true,
                    new Dictionary<string, object>
                    {
                        { "FestivalId", NewFestival.Id }
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
