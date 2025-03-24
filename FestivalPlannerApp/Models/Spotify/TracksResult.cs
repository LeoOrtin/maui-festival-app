namespace FestivalPlannerApp.Models;

public class TracksResult
{
    public string Href { get; set; } = string.Empty;
    public int Limit { get; set; }
    public string Next { get; set; } = string.Empty;
    public int Offset { get; set; }
    public string Previous { get; set; } = string.Empty;
    public int Total { get; set; }
    public List<Track> Items { get; set; } = [];
}
