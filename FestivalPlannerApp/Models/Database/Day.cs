using CommunityToolkit.Mvvm.ComponentModel;
using FestivalPlannerApp.ViewModels;
using SQLite;
using System.Collections.ObjectModel;

namespace FestivalPlannerApp.Models;

[Table("days")]
public partial class Day : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed]
    public int FestivalID { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool AfterMidnight { get; set; }
    public DateTime MinStartDateTime { get; set; } = DateTime.Today;
    public DateTime MaxStartDateTime { get; set; } = DateTime.MaxValue;

    //private EditFestivalViewModel? _viewModel;
    private ObservableCollection<TimeSpan>? _timeSlots;
    public ObservableCollection<TimeSpan> TimeSlots
    {
        get
        {
            if (_timeSlots == null)
            {
                var times = new ObservableCollection<TimeSpan>();
                var current = StartDateTime.TimeOfDay;
                var end = AfterMidnight ? TimeSpan.FromHours(24) : EndDateTime.TimeOfDay;

                while (current <= end)
                {
                    times.Add(current);
                    current = current.Add(TimeSpan.FromMinutes(30));
                    if (end == TimeSpan.FromHours(24) && current == TimeSpan.FromHours(24))
                    {
                        current = TimeSpan.Zero;
                        end = EndDateTime.TimeOfDay;
                    }
                }
                _timeSlots = times;
            }
            return _timeSlots;
        }
    }

    //public IEnumerable<ScheduleItem> FilteredSchedule
    //{
    //    get
    //    {
    //        if (_viewModel == null)
    //            return Enumerable.Empty<ScheduleItem>();

    //        return _viewModel?.Schedule?
    //            .Where(s => s.Date.Date == StartDateTime.Date)
    //            ?? Enumerable.Empty<ScheduleItem>();
    //    }
    //}
    //// Inject ViewModel after SQLite loads the object
    //public void InjectViewModel(EditFestivalViewModel viewModel)
    //{
    //    if (_viewModel != null)
    //    {
    //        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
    //    }

    //    _viewModel = viewModel;
    //    _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    //}

    //private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(_viewModel.Schedule))
    //    {
    //        OnPropertyChanged(nameof(FilteredSchedule)); // Notify UI of changes
    //    }
    //}
}
