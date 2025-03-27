using SQLite;

namespace FestivalPlannerApp.Models;

[Table("festivals")]
public class Festival
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [Indexed]
    public string? UserId { get; set; } 
}
