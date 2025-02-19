using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;
using System.Collections.ObjectModel;

namespace FestivalPlannerApp.ViewModels
{
    public partial class NewFestivalViewModel : ObservableObject
    {
        private readonly IAlertService _alertService;
        private readonly IDatabaseService _databaseService;
        [ObservableProperty]
        public partial int StagesStepperValue { get; set; }
        [ObservableProperty]
        public partial int DaysStepperValue { get; set; }
        [ObservableProperty]
        public partial ObservableCollection<Stage> Stages { get; set; }
        [ObservableProperty]
        public partial Day NewDay { get; set; }
        [ObservableProperty]
        public partial ObservableCollection<Day> Days { get; set; }
        [ObservableProperty]
        public partial Festival NewFestival { get; set; }
        public NewFestivalViewModel(IAlertService alertService, IDatabaseService databaseService)
        {
            _alertService = alertService;
            _databaseService = databaseService;
            Stages = new ObservableCollection<Stage>
            { 
                new Stage()
            };
            NewDay = new Day();
            Days = new ObservableCollection<Day>();
            NewFestival = new Festival();
        }
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
        public void DaysStepper()
        {
            if (DaysStepperValue < Days.Count)
            {
                Days.RemoveAt(Days.Count - 1);
            }
            else
            {
                Days.Add(new Day { });
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
                _alertService.ShowAlert("End time must be later than Start time",
                    "If the concerts are finishing after midnigth, please tick the box");
            }
        }
        [RelayCommand]
        public void AddDay()
        {
            if (NewDay.AfterMidnight)
            {
                NewDay.EndDateTime = NewDay.StartDateTime.Date.AddDays(1) + NewDay.EndTime;
                Days.Add(NewDay);
            }
            else if (NewDay.EndTime > NewDay.StartTime)
            {
                NewDay.EndDateTime = NewDay.StartDateTime.Date + NewDay.EndTime;
                Days.Add(NewDay);
            }
            else
            {
                _alertService.ShowAlert("Please enter a valid End time",
                    "End time must be later than Start time");
            }
        }
        [RelayCommand]
        public async Task Save()
        {
            if(string.IsNullOrEmpty(NewFestival.Name))
            {
                await _alertService.ShowAlert("Please enter festival name", "");
            }
            else
            {
                NewFestival.Stages = Stages.ToList();
                NewFestival.Days = Days.ToList();
                await _databaseService.AddFestival(NewFestival);
                await Shell.Current.GoToAsync($"/{nameof(EditFestivalPage)}", true,
                    new Dictionary<string, object>
                    {
                        { "FestivalId", NewFestival.Id   }
                    });
            }
        }
        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
    }
}
