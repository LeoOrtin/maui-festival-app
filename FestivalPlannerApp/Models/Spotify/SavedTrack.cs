namespace FestivalPlannerApp.Models;

public class SavedTrack
{
    public string Added_at { get; set; } = string.Empty;
    public Track Track { get; set; } = new();
}
