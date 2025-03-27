namespace FestivalPlannerApp.Models;

public class ConcertGroup : List<Concert>
{
    public string DayTitle { get; }
    public ConcertGroup(Day day, IEnumerable<Concert> concerts)
    {
        DayTitle = day.StartDateTime.ToString("ddd, dd MMM yyyy");
        AddRange(concerts.Where(c => c.DayId == day.Id));
    }
}
