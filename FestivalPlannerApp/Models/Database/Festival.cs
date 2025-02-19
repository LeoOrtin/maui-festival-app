using SQLite;

namespace FestivalPlannerApp.Models
{
    [Table("Festivals")]
    public class Festival
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Stage>? Stages { get; set; }
        public List<Day>? Days { get; set; }
    }
}
