namespace FestivalPlannerApp.Models;

public class FollowedArtistsResult
{
    public string Href { get; set; } = string.Empty;
    public int Limit { get; set; }
    public string Next { get; set; } = string.Empty;
    public Cursors Cursors { get; set; } = new();
    public int Total { get; set; }
    public List<Artist> Items { get; set;} = new();
}
