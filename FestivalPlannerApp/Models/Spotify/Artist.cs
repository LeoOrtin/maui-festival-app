namespace FestivalPlannerApp.Models;

public class Artist
{
    public ExternalUrls External_urls { get; set; } = new ExternalUrls();
    public Followers Followers { get; set; } = new Followers();
    public List<string> Genres { get; set; } = [];
    public string Href { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public List<Image> Images { get; set; } = [];
    public string Name { get; set; } = string.Empty;
    public int Popularity { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}
