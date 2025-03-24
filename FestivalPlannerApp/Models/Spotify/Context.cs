namespace FestivalPlannerApp.Models;

public class Context
{
    public string Type { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public ExternalUrls External_urls { get; set; } = new();
    public string Uri { get; set; } = string.Empty;
}
