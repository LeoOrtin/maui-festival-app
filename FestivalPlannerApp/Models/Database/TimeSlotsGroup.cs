namespace FestivalPlannerApp.Models;

public class TimeSlotsGroup : List<TimeSpan>
{
    public TimeSlotsGroup(Day day)
    {
        AddRange(day.TimeSlots);
    }
}
