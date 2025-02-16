namespace FestivalPlannerApp.Models
{
    public class Track
    {
        public Album Album { get; set; } = new();
        public List<Artist> Artists { get; set; } = [];
        public List<string> Available_markets { get; set; } = [];
        public int Disc_number { get; set; }
        public int Duration_ms { get; set; }
        public bool Explicit { get; set; }
        public ExternalIds External_ids { get; set; } = new();
        public ExternalUrls External_urls { get; set; } = new();
        public string Href { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public bool Is_playable { get; set; }
        public LinkedFrom Linked_from { get; set; } = new();
        public Restrictions Restrictions { get; set; } = new();
        public string Name { get; set; } = string.Empty;
        public int Popularity { get; set; }
        public string Preview_url { get; set; } = string.Empty;
        public int Track_number { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
        public bool Is_local { get; set; }

    }
}
