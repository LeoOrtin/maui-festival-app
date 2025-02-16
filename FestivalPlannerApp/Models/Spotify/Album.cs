namespace FestivalPlannerApp.Models
{
    public class Album
    {
        public string Album_type { get; set; } = String.Empty;
        public int Total_tracks { get; set; }
        public List<string> Available_markets { get; set; } = [];
        public ExternalUrls External_urls { get; set; } = new();
        public string Href { get; set; } = String.Empty;
        public string Id { get; set; } = String.Empty;
        public List<Image> Images { get; set; } = [];
        public string Name { get; set; } = String.Empty;
        public string Release_date { get; set; } = String.Empty;
        public string Release_date_precision { get; set; } = String.Empty;
        public Restrictions Restrictions { get; set; } = new();
        public string Type { get; set; } = String.Empty;
        public string Uri { get; set; } = String.Empty;
        public List<Artist> Artists { get; set; } = [];
    }
}
