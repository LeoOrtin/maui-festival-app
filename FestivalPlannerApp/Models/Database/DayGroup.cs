using System.Collections.ObjectModel;

namespace FestivalPlannerApp.Models;

public partial class DayGroup : List<StageSchedule>
{
    public DateTime DayDate { get; }
    public string DayTitle { get; }

    public DayGroup(Day day, IEnumerable<Stage> stages, IEnumerable<Concert> concerts)
    {
        DayDate = day.StartDateTime;
        DayTitle = day.StartDateTime.ToString("ddd, dd MMM yyyy");

        // Create schedule for each stage
        foreach (var stage in stages)
        {
            Add(new StageSchedule(stage, day, concerts));
        }
    }
}
