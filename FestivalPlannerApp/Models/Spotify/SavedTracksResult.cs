namespace FestivalPlannerApp.Models;

public class SavedTracksResult
{
    public string Href { get; set; } = string.Empty;
    public int Limits { get; set; }
    public string Next {  get; set; } = string.Empty;
    public int Offset { get; set; }
    public string Previous { get; set; } = string.Empty;
    public int Total { get; set; }
    public List<SavedTrack> Items { get; set; } = [];
}
