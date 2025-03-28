
using System.Collections.ObjectModel;

namespace FestivalPlannerApp.Models;
public record ArtistTimeSlot(TimeSpan Time, string ArtistName);
public class StageSchedule
{
    public Stage Stage { get; }
    public Day Day { get; }
    public ObservableCollection<ArtistTimeSlot> ArtistTimeSlots { get; } = new();   
    public StageSchedule(Stage stage, Day day, IEnumerable<Concert> concerts)
    {
        Stage = stage;
        Day = day;

        // Calculate time slots with concerts
        var current = day.StartDateTime.TimeOfDay;
        var end = day.AfterMidnight ? TimeSpan.FromHours(24) : day.EndDateTime.TimeOfDay;

        while (current <= end)
        {
            var concert = concerts.FirstOrDefault(c =>
                c.StageId == stage.Id &&
                c.DayId == day.Id &&
                c.StartTime <= current &&
                (c.EndTime > current || c.EndTime == TimeSpan.Zero));

            ArtistTimeSlots.Add(new ArtistTimeSlot(current, concert?.ArtistName ?? string.Empty));

            current = current.Add(TimeSpan.FromMinutes(30));
            if (end == TimeSpan.FromHours(24) && current == TimeSpan.FromHours(24))
            {
                current = TimeSpan.Zero;
                end = day.EndDateTime.TimeOfDay;
            }
        }
    }
}
