using SQLite;

namespace FestivalPlannerApp.Models
{
    [Table("concerts")]
    public class Concert
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int FestivalId { get; set; }
        [Indexed]
        public int StageId { get; set; }
        [Indexed]
        public int DayId { get; set; }
        public string? ArtistName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
