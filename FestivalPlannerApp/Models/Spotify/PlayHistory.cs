namespace FestivalPlannerApp.Models;

public class PlayHistory
{
    public Track Track { get; set; } = new();
    public string Played_at { get; set; } = string.Empty;
    public Context Context { get; set; } = new();
}
