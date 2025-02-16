namespace FestivalPlannerApp.Models
{
    public class User
    {
        public string Country { get; set; } = string.Empty;
        public string Display_name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ExplicitContent Explicit_content { get; set; } = new ExplicitContent();
        public ExternalUrls External_urls { get; set; } = new ExternalUrls();
        public Followers Followers { get; set; } = new Followers();
        public string Href { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<Image> Images { get; set; } = [];
        public string Product { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
    }
}
