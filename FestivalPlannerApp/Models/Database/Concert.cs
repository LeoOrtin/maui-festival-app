using SQLite;

namespace FestivalPlannerApp.Models
{
    [Table("Concerts")]
    public class Concert
    {
        [PrimaryKey]
        public int Id { get; set; }
        [Indexed]
        public int FestivalId { get; set; }
        public string? Stage { get; set; }
        public string? ArtistName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
