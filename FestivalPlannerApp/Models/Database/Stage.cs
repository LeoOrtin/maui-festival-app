using SQLite;

namespace FestivalPlannerApp.Models
{
    [Table("stages")]
    public class Stage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Indexed]
        public int FestivalId { get; set; }
    }
}