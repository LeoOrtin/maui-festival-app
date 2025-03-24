namespace FestivalPlannerApp.Models;

public class ScheduleItem
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Dictionary<int, string?> StageEvents { get; set; } = []; // stageId, concertName
}
